-- Tạo database nếu chưa có
CREATE DATABASE SIMS_System;
GO
USE SIMS_System;
GO

-- Bảng Users (quản lý tài khoản đăng nhập)
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL -- 'Admin' hoặc 'Student'
);

-- Bảng Students (thông tin sinh viên)
CREATE TABLE Students (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Bảng Courses (quản lý khóa học)
CREATE TABLE Courses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);

-- Bảng StudentCourses (gán khóa học cho sinh viên)
CREATE TABLE StudentCourses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE,
    FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE CASCADE
);

-- Tạo tài khoản Admin mặc định
INSERT INTO Users (Username, PasswordHash, Role)
VALUES ('admin', 'admin123', 'Admin'); -- PasswordHash nên mã hóa, ví dụ dùng SHA256
