USE [EmployeeDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @InsertedEmployees INT = 0;
DECLARE @InsertedReviews INT = 0;

;WITH SeedData (FirstName, LastName, Email) AS
(
    SELECT *
    FROM (VALUES
        ('Luca', 'Rossi', 'luca.rossi@azienda.it'),
        ('Marco', 'Bianchi', 'marco.bianchi@azienda.it'),
        ('Giulia', 'Romano', 'giulia.romano@azienda.it'),
        ('Francesca', 'Greco', 'francesca.greco@azienda.it'),
        ('Alessandro', 'Bruno', 'alessandro.bruno@azienda.it'),
        ('Chiara', 'Gallo', 'chiara.gallo@azienda.it'),
        ('Matteo', 'Conti', 'matteo.conti@azienda.it'),
        ('Sara', 'DeLuca', 'sara.deluca@azienda.it'),
        ('Davide', 'Mancini', 'davide.mancini@azienda.it'),
        ('Elena', 'Costa', 'elena.costa@azienda.it'),
        ('Simone', 'Giordano', 'simone.giordano@azienda.it'),
        ('Federica', 'Rinaldi', 'federica.rinaldi@azienda.it'),
        ('Andrea', 'Moretti', 'andrea.moretti@azienda.it'),
        ('Valentina', 'Barbieri', 'valentina.barbieri@azienda.it'),
        ('Stefano', 'Ferrari', 'stefano.ferrari@azienda.it'),
        ('Martina', 'Marini', 'martina.marini@azienda.it'),
        ('Gabriele', 'Lombardi', 'gabriele.lombardi@azienda.it'),
        ('Noemi', 'Parisi', 'noemi.parisi@azienda.it'),
        ('Riccardo', 'Caruso', 'riccardo.caruso@azienda.it'),
        ('Alessia', 'Santoro', 'alessia.santoro@azienda.it')
    ) AS v (FirstName, LastName, Email)
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

;WITH SeedReviews (EmployeeEmail, Rate, Comment) AS
(
    SELECT *
    FROM (VALUES
        ('luca.rossi@azienda.it', 5, 'Professionale e puntuale.'),
        ('marco.bianchi@azienda.it', 4, 'Ottima capacita di problem solving.'),
        ('giulia.romano@azienda.it', 5, 'Collaborativa e precisa.'),
        ('francesca.greco@azienda.it', 4, 'Consegne sempre nei tempi.'),
        ('alessandro.bruno@azienda.it', 5, 'Leadership tecnica eccellente.'),
        ('chiara.gallo@azienda.it', 4, 'Comunicazione chiara con il team.'),
        ('matteo.conti@azienda.it', 5, 'Qualita del codice molto alta.'),
        ('sara.deluca@azienda.it', 4, 'Affidabile nelle attivita critiche.'),
        ('davide.mancini@azienda.it', 5, 'Proattivo nel miglioramento continuo.'),
        ('elena.costa@azienda.it', 4, 'Buona gestione delle priorita.'),
        ('simone.giordano@azienda.it', 5, 'Supporta efficacemente i colleghi.'),
        ('federica.rinaldi@azienda.it', 4, 'Attenta ai dettagli funzionali.'),
        ('andrea.moretti@azienda.it', 5, 'Eccellente autonomia operativa.'),
        ('valentina.barbieri@azienda.it', 4, 'Ottimo contributo alle retrospettive.'),
        ('stefano.ferrari@azienda.it', 5, 'Performance costante e solida.'),
        ('martina.marini@azienda.it', 4, 'Buona qualita della documentazione.'),
        ('gabriele.lombardi@azienda.it', 5, 'Grande affidabilita in produzione.'),
        ('noemi.parisi@azienda.it', 4, 'Approccio pragmatico ai problemi.'),
        ('riccardo.caruso@azienda.it', 5, 'Spirito di iniziativa elevato.'),
        ('alessia.santoro@azienda.it', 4, 'Ottima collaborazione cross-team.')
    ) AS v (EmployeeEmail, Rate, Comment)
)
INSERT INTO dbo.Review (Rate, Comment, EmployeeId)
SELECT sr.Rate, sr.Comment, e.Id
FROM SeedReviews sr
INNER JOIN dbo.Employee e ON e.Email = sr.EmployeeEmail
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.Review r
    WHERE r.EmployeeId = e.Id
      AND r.Rate = sr.Rate
      AND r.Comment = sr.Comment
);

SET @InsertedReviews = @@ROWCOUNT;

COMMIT TRANSACTION;

PRINT CONCAT('Inserimento Employee completato. Nuove righe: ', @InsertedEmployees);
PRINT CONCAT('Inserimento Review completato. Nuove righe: ', @InsertedReviews);
