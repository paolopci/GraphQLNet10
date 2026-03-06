USE [EmployeeDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @InsertedEmployees INT = 0;
DECLARE @UpdatedEmployees INT = 0;
DECLARE @InsertedReviews INT = 0;
DECLARE @UpdatedReviews INT = 0;

;WITH Numbers AS
(
    SELECT TOP (100)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
    FROM sys.all_objects
),
FirstNames AS
(
    SELECT *
    FROM (VALUES
        (1, 'Luca'), (2, 'Marco'), (3, 'Giulia'), (4, 'Francesca'), (5, 'Alessandro'),
        (6, 'Chiara'), (7, 'Matteo'), (8, 'Sara'), (9, 'Davide'), (10, 'Elena'),
        (11, 'Simone'), (12, 'Federica'), (13, 'Andrea'), (14, 'Valentina'), (15, 'Stefano'),
        (16, 'Martina'), (17, 'Gabriele'), (18, 'Noemi'), (19, 'Riccardo'), (20, 'Alessia')
    ) AS f(Id, Value)
),
LastNames AS
(
    SELECT *
    FROM (VALUES
        (1, 'Rossi'), (2, 'Bianchi'), (3, 'Romano'), (4, 'Greco'), (5, 'Bruno'),
        (6, 'Gallo'), (7, 'Conti'), (8, 'DeLuca'), (9, 'Mancini'), (10, 'Costa'),
        (11, 'Giordano'), (12, 'Rinaldi'), (13, 'Moretti'), (14, 'Barbieri'), (15, 'Ferrari'),
        (16, 'Marini'), (17, 'Lombardi'), (18, 'Parisi'), (19, 'Caruso'), (20, 'Santoro'),
        (21, 'Esposito'), (22, 'Ricci'), (23, 'Leone'), (24, 'Pellegrini'), (25, 'Serra')
    ) AS l(Id, Value)
),
SeedData AS
(
    SELECT
        n.N,
        fn.Value AS FirstName,
        ln.Value AS LastName,
        CONCAT('employee', RIGHT(CONCAT('000', CAST(n.N AS varchar(3))), 3), '@azienda.it') AS Email
    FROM Numbers n
    INNER JOIN FirstNames fn ON fn.Id = ((n.N - 1) % 20) + 1
    INNER JOIN LastNames ln ON ln.Id = ((n.N - 1) % 25) + 1
)
UPDATE e
SET
    e.FirstName = s.FirstName,
    e.LastName = s.LastName
FROM dbo.Employee e
INNER JOIN SeedData s ON s.Email = e.Email
WHERE e.FirstName <> s.FirstName
   OR e.LastName <> s.LastName;

SET @UpdatedEmployees = @@ROWCOUNT;

;WITH Numbers AS
(
    SELECT TOP (100)
        ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS N
    FROM sys.all_objects
),
FirstNames AS
(
    SELECT *
    FROM (VALUES
        (1, 'Luca'), (2, 'Marco'), (3, 'Giulia'), (4, 'Francesca'), (5, 'Alessandro'),
        (6, 'Chiara'), (7, 'Matteo'), (8, 'Sara'), (9, 'Davide'), (10, 'Elena'),
        (11, 'Simone'), (12, 'Federica'), (13, 'Andrea'), (14, 'Valentina'), (15, 'Stefano'),
        (16, 'Martina'), (17, 'Gabriele'), (18, 'Noemi'), (19, 'Riccardo'), (20, 'Alessia')
    ) AS f(Id, Value)
),
LastNames AS
(
    SELECT *
    FROM (VALUES
        (1, 'Rossi'), (2, 'Bianchi'), (3, 'Romano'), (4, 'Greco'), (5, 'Bruno'),
        (6, 'Gallo'), (7, 'Conti'), (8, 'DeLuca'), (9, 'Mancini'), (10, 'Costa'),
        (11, 'Giordano'), (12, 'Rinaldi'), (13, 'Moretti'), (14, 'Barbieri'), (15, 'Ferrari'),
        (16, 'Marini'), (17, 'Lombardi'), (18, 'Parisi'), (19, 'Caruso'), (20, 'Santoro'),
        (21, 'Esposito'), (22, 'Ricci'), (23, 'Leone'), (24, 'Pellegrini'), (25, 'Serra')
    ) AS l(Id, Value)
),
SeedData AS
(
    SELECT
        n.N,
        fn.Value AS FirstName,
        ln.Value AS LastName,
        CONCAT('employee', RIGHT(CONCAT('000', CAST(n.N AS varchar(3))), 3), '@azienda.it') AS Email
    FROM Numbers n
    INNER JOIN FirstNames fn ON fn.Id = ((n.N - 1) % 20) + 1
    INNER JOIN LastNames ln ON ln.Id = ((n.N - 1) % 25) + 1
)
INSERT INTO dbo.Employee (FirstName, LastName, Email)
SELECT s.FirstName, s.LastName, s.Email
FROM SeedData s
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Employee e
    WHERE e.Email = s.Email
);

SET @InsertedEmployees = @@ROWCOUNT;

;WITH SeedReviews AS
(
    SELECT
        e.Id AS EmployeeId,
        CASE WHEN e.Id % 2 = 0 THEN 4 ELSE 5 END AS Rate,
        CONCAT('Valutazione annuale seed per ', e.FirstName, ' ', e.LastName, ' (', e.Email, ').') AS Comment
    FROM dbo.Employee e
    WHERE e.Email LIKE 'employee___@azienda.it'
),
ReviewTarget AS
(
    SELECT
        sr.EmployeeId,
        sr.Rate,
        sr.Comment,
        (
            SELECT MIN(r.Id)
            FROM dbo.Review r
            WHERE r.EmployeeId = sr.EmployeeId
        ) AS ExistingReviewId
    FROM SeedReviews sr
)
UPDATE r
SET
    r.Rate = rt.Rate,
    r.Comment = rt.Comment
FROM dbo.Review r
INNER JOIN ReviewTarget rt ON r.Id = rt.ExistingReviewId
WHERE r.Rate <> rt.Rate
   OR r.Comment <> rt.Comment;

SET @UpdatedReviews = @@ROWCOUNT;

;WITH SeedReviews AS
(
    SELECT
        e.Id AS EmployeeId,
        CASE WHEN e.Id % 2 = 0 THEN 4 ELSE 5 END AS Rate,
        CONCAT('Valutazione annuale seed per ', e.FirstName, ' ', e.LastName, ' (', e.Email, ').') AS Comment
    FROM dbo.Employee e
    WHERE e.Email LIKE 'employee___@azienda.it'
)
INSERT INTO dbo.Review (Rate, Comment, EmployeeId)
SELECT sr.Rate, sr.Comment, sr.EmployeeId
FROM SeedReviews sr
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Review r
    WHERE r.EmployeeId = sr.EmployeeId
);

SET @InsertedReviews = @@ROWCOUNT;

COMMIT TRANSACTION;

PRINT CONCAT('Employee aggiornati: ', @UpdatedEmployees, ', inseriti: ', @InsertedEmployees);
PRINT CONCAT('Review aggiornate: ', @UpdatedReviews, ', inserite: ', @InsertedReviews);
