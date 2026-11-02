-- ============================================================
-- Week 7 – Task 7.4: Stored Procedures
-- ============================================================
USE StudentDb;
GO

-- ----------------------------------------------------------
-- usp_GetStudentById
-- ----------------------------------------------------------
CREATE OR ALTER PROCEDURE usp_GetStudentById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Age, RollNumber, Email, InternalNotes, EnrolledOn
    FROM   Students
    WHERE  Id = @Id;
END
GO

-- ----------------------------------------------------------
-- usp_InsertStudent
-- ----------------------------------------------------------
CREATE OR ALTER PROCEDURE usp_InsertStudent
    @Name          NVARCHAR(200),
    @Age           INT,
    @RollNumber    NVARCHAR(50),
    @Email         NVARCHAR(200),
    @InternalNotes NVARCHAR(MAX),
    @EnrolledOn    DATE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Students (Name, Age, RollNumber, Email, InternalNotes, EnrolledOn)
    VALUES (@Name, @Age, @RollNumber, @Email, @InternalNotes, @EnrolledOn);
    SELECT SCOPE_IDENTITY() AS NewId;
END
GO
