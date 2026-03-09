USE [EmployeeDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @InsertedEmployees INT = 0;
DECLARE @UpdatedEmployees INT = 0;
DECLARE @InsertedReviews INT = 0;
DECLARE @UpdatedReviews INT = 0;
DECLARE @DeletedReviews INT = 0;

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

;WITH ReviewSlots AS
(
    SELECT * FROM (VALUES (1), (2), (3)) AS rs(Slot)
),
SeedReviews AS
(
    SELECT
        e.Id AS EmployeeId,
        rs.Slot AS ReviewOrdinal,
        CASE
            WHEN rs.Slot = 1 THEN CASE WHEN e.Id % 2 = 0 THEN 4 ELSE 5 END
            WHEN rs.Slot = 2 THEN CASE WHEN e.Id % 5 IN (0, 1) THEN 3 ELSE 4 END
            ELSE 5
        END AS Rate,
        CONCAT(
            'Valutazione seed #',
            rs.Slot,
            ' per ',
            e.FirstName,
            ' ',
            e.LastName,
            ' (',
            e.Email,
            ').'
        ) AS Comment
    FROM dbo.Employee e
    CROSS JOIN ReviewSlots rs
    WHERE e.Email LIKE 'employee___@azienda.it'
      AND (
            (e.Id % 4 = 0 AND rs.Slot = 1)
         OR (e.Id % 4 = 1 AND rs.Slot IN (1, 2))
         OR (e.Id % 4 = 2 AND rs.Slot IN (1, 2, 3))
      )
),
ExistingReviews AS
(
    SELECT
        r.Id,
        r.EmployeeId,
        ROW_NUMBER() OVER (PARTITION BY r.EmployeeId ORDER BY r.Id) AS ReviewOrdinal
    FROM dbo.Review r
    INNER JOIN dbo.Employee e ON e.Id = r.EmployeeId
    WHERE e.Email LIKE 'employee___@azienda.it'
),
ReviewTarget AS
(
    SELECT
        er.Id AS ExistingReviewId,
        sr.Rate,
        sr.Comment
    FROM SeedReviews sr
    INNER JOIN ExistingReviews er
        ON er.EmployeeId = sr.EmployeeId
       AND er.ReviewOrdinal = sr.ReviewOrdinal
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

;WITH ReviewSlots AS
(
    SELECT * FROM (VALUES (1), (2), (3)) AS rs(Slot)
),
SeedReviews AS
(
    SELECT
        e.Id AS EmployeeId,
        rs.Slot AS ReviewOrdinal,
        CASE
            WHEN rs.Slot = 1 THEN CASE WHEN e.Id % 2 = 0 THEN 4 ELSE 5 END
            WHEN rs.Slot = 2 THEN CASE WHEN e.Id % 5 IN (0, 1) THEN 3 ELSE 4 END
            ELSE 5
        END AS Rate,
        CONCAT(
            'Valutazione seed #',
            rs.Slot,
            ' per ',
            e.FirstName,
            ' ',
            e.LastName,
            ' (',
            e.Email,
            ').'
        ) AS Comment
    FROM dbo.Employee e
    CROSS JOIN ReviewSlots rs
    WHERE e.Email LIKE 'employee___@azienda.it'
      AND (
            (e.Id % 4 = 0 AND rs.Slot = 1)
         OR (e.Id % 4 = 1 AND rs.Slot IN (1, 2))
         OR (e.Id % 4 = 2 AND rs.Slot IN (1, 2, 3))
      )
),
ExistingReviews AS
(
    SELECT
        r.Id,
        r.EmployeeId,
        ROW_NUMBER() OVER (PARTITION BY r.EmployeeId ORDER BY r.Id) AS ReviewOrdinal
    FROM dbo.Review r
    INNER JOIN dbo.Employee e ON e.Id = r.EmployeeId
    WHERE e.Email LIKE 'employee___@azienda.it'
)
INSERT INTO dbo.Review (Rate, Comment, EmployeeId)
SELECT sr.Rate, sr.Comment, sr.EmployeeId
FROM SeedReviews sr
LEFT JOIN ExistingReviews er
    ON er.EmployeeId = sr.EmployeeId
   AND er.ReviewOrdinal = sr.ReviewOrdinal
WHERE er.Id IS NULL;

SET @InsertedReviews = @@ROWCOUNT;

;WITH ReviewSlots AS
(
    SELECT * FROM (VALUES (1), (2), (3)) AS rs(Slot)
),
DesiredReviewOrdinals AS
(
    SELECT
        e.Id AS EmployeeId,
        rs.Slot AS ReviewOrdinal
    FROM dbo.Employee e
    CROSS JOIN ReviewSlots rs
    WHERE e.Email LIKE 'employee___@azienda.it'
      AND (
            (e.Id % 4 = 0 AND rs.Slot = 1)
         OR (e.Id % 4 = 1 AND rs.Slot IN (1, 2))
         OR (e.Id % 4 = 2 AND rs.Slot IN (1, 2, 3))
      )
),
ExistingReviews AS
(
    SELECT
        r.Id,
        r.EmployeeId,
        ROW_NUMBER() OVER (PARTITION BY r.EmployeeId ORDER BY r.Id) AS ReviewOrdinal
    FROM dbo.Review r
    INNER JOIN dbo.Employee e ON e.Id = r.EmployeeId
    WHERE e.Email LIKE 'employee___@azienda.it'
)
DELETE r
FROM dbo.Review r
INNER JOIN ExistingReviews er ON er.Id = r.Id
LEFT JOIN DesiredReviewOrdinals d
    ON d.EmployeeId = er.EmployeeId
   AND d.ReviewOrdinal = er.ReviewOrdinal
WHERE d.EmployeeId IS NULL;

SET @DeletedReviews = @@ROWCOUNT;

COMMIT TRANSACTION;

PRINT CONCAT('Employee aggiornati: ', @UpdatedEmployees, ', inseriti: ', @InsertedEmployees);
PRINT CONCAT('Review aggiornate: ', @UpdatedReviews, ', inserite: ', @InsertedReviews, ', eliminate: ', @DeletedReviews);
