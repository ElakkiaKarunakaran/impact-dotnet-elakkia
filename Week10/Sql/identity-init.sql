-- Task 10.2 — Identity service schema (ADO.NET, stored procedures only).
-- Mounted into the sqlserver container and run once on first startup
-- (see docker-compose.yml init-db service).

IF DB_ID('IdentityDb') IS NULL
    CREATE DATABASE IdentityDb;
GO

USE IdentityDb;
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(500) NOT NULL,
        PasswordSalt NVARCHAR(500) NOT NULL,
        Role         NVARCHAR(50)  NOT NULL DEFAULT 'Student'
    );
END
GO

-- Task 10.2 — parameterized access only ever goes through these two procs.
CREATE OR ALTER PROCEDURE dbo.usp_FindUserByUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Username, PasswordHash, PasswordSalt, Role
    FROM dbo.Users
    WHERE Username = @Username;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_InsertUser
    @Username     NVARCHAR(100),
    @PasswordHash NVARCHAR(500),
    @PasswordSalt NVARCHAR(500),
    @Role         NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Users (Username, PasswordHash, PasswordSalt, Role)
    VALUES (@Username, @PasswordHash, @PasswordSalt, @Role);
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- Seed two demo accounts; password for both is "password123".
-- Hashes below are placeholders — the app re-hashes on /register, so for a
-- truly clean demo, register fresh accounts instead of relying on these rows.
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
    EXEC dbo.usp_InsertUser @Username='admin', @PasswordHash='SEED_PLACEHOLDER', @PasswordSalt='SEED_PLACEHOLDER', @Role='Teacher';
GO
