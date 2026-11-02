-- ============================================================
-- Week 7 – Task 7.1: ER Diagram realized as SQL DDL
-- Database: StudentDb  (≥3NF)
--
-- Tables:
--   Users      (Id PK, Username UNIQUE, PasswordHash, Role)
--   Teachers   (Id PK, Name, Email UNIQUE, Designation)
--   Students   (Id PK, Name, Age, RollNumber UNIQUE,
--               Email UNIQUE, InternalNotes, EnrolledOn,
--               TeacherId FK → Teachers)
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StudentDb')
    CREATE DATABASE StudentDb;
GO

USE StudentDb;
GO

-- ----------------------------------------------------------
-- Users
-- ----------------------------------------------------------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE Users (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Role         NVARCHAR(50)  NOT NULL DEFAULT 'Student',
        CONSTRAINT UQ_Users_Username UNIQUE (Username)
    );

    INSERT INTO Users (Username, PasswordHash, Role) VALUES
        ('admin',   'AQAAAAIAAYagAAAAEH5uYb7sNBpexmqAkG4pzwJzI1hXUfA==', 'Admin'),
        ('elakkia', 'AQAAAAIAAYagAAAAEH5uYb7sNBpexmqAkG4pzwJzI1hXUfA==', 'Student');
END
GO

-- ----------------------------------------------------------
-- Teachers
-- ----------------------------------------------------------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_NAME = 'Teachers')
BEGIN
    CREATE TABLE Teachers (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(200) NOT NULL,
        Email       NVARCHAR(200) NOT NULL,
        Designation NVARCHAR(100),
        CONSTRAINT UQ_Teachers_Email UNIQUE (Email)
    );

    INSERT INTO Teachers (Name, Email, Designation) VALUES
        ('Dr. Priya',  'priya@college.edu',  'Professor'),
        ('Mr. Arjun',  'arjun@college.edu',  'Lecturer');
END
GO

-- ----------------------------------------------------------
-- Students
-- ----------------------------------------------------------
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_NAME = 'Students')
BEGIN
    CREATE TABLE Students (
        Id            INT IDENTITY(1,1) PRIMARY KEY,
        Name          NVARCHAR(200) NOT NULL,
        Age           INT           NOT NULL CHECK (Age BETWEEN 5 AND 100),
        RollNumber    NVARCHAR(50)  NOT NULL,
        Email         NVARCHAR(200),
        InternalNotes NVARCHAR(MAX),
        EnrolledOn    DATE,
        TeacherId     INT           NULL REFERENCES Teachers(Id),
        CONSTRAINT UQ_Students_RollNumber UNIQUE (RollNumber),
        CONSTRAINT UQ_Students_Email     UNIQUE (Email)
    );

    INSERT INTO Students (Name, Age, RollNumber, Email, EnrolledOn) VALUES
        ('Elakkia', 21, 'R001', 'elakkia@email.com', '2024-01-15'),
        ('Ramya',   22, 'R002', 'ramya@email.com',   '2024-01-15'),
        ('Arjun',   23, 'R003', 'arjun@email.com',   '2024-01-16');
END
GO
