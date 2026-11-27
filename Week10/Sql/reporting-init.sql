-- Task 10.4 — Reporting service's PRE-EXISTING schema.
-- EF DB First scaffolds C# classes FROM this DB — the DB is authored here,
-- by hand, exactly as "a database owned by another team" would be.

IF DB_ID('ReportingDb') IS NULL
    CREATE DATABASE ReportingDb;
GO

USE ReportingDb;
GO

IF OBJECT_ID('dbo.Enrollments', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Enrollments (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        StudentName NVARCHAR(200) NOT NULL,
        CourseCode  NVARCHAR(10)  NOT NULL,
        EnrolledOn  DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Enrollments)
BEGIN
    INSERT INTO dbo.Enrollments (StudentName, CourseCode, EnrolledOn) VALUES
        ('Elakkia', 'CS101', '2026-01-20'),
        ('Elakkia', 'CS201', '2026-01-20'),
        ('Ramya',   'CS101', '2026-01-21'),
        ('Arjun',   'CS301', '2026-01-22'),
        ('Arjun',   'CS201', '2026-01-22');
END
GO

-- Task 10.4 — the view Reporting reads through EF's HasNoKey().ToView(...).
CREATE OR ALTER VIEW dbo.vw_EnrollmentSummary AS
    SELECT CourseCode, COUNT(*) AS StudentCount
    FROM dbo.Enrollments
    GROUP BY CourseCode;
GO
