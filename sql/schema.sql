-- XpressLearn Database Schema

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Role NVARCHAR(20) DEFAULT 'Student' CHECK (Role IN ('Admin','Instructor','Student')),
    Bio NVARCHAR(500),
    ProfileImageUrl NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000),
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId),
    InstructorId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    ThumbnailUrl NVARCHAR(500),
    Price DECIMAL(10,2) DEFAULT 0,
    Level NVARCHAR(20) DEFAULT 'Beginner' CHECK (Level IN ('Beginner','Intermediate','Advanced')),
    IsPublished BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);

CREATE TABLE Attempts (
    AttemptId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    CourseId INT NOT NULL FOREIGN KEY REFERENCES Courses(CourseId),
    Score DECIMAL(5,2),
    MaxScore DECIMAL(5,2) DEFAULT 100,
    StartedAt DATETIME2 DEFAULT GETDATE(),
    CompletedAt DATETIME2 NULL,
    Status NVARCHAR(20) DEFAULT 'InProgress' CHECK (Status IN ('InProgress','Completed','Abandoned')),
    Notes NVARCHAR(1000)
);
GO

-- usp_GetAllCourses
CREATE PROCEDURE usp_GetAllCourses
AS BEGIN
    SELECT c.CourseId, c.Title, c.Description, c.CategoryId, cat.Name AS CategoryName,
           c.InstructorId, u.FirstName + ' ' + u.LastName AS InstructorName,
           c.ThumbnailUrl, c.Price, c.Level, c.IsPublished, c.CreatedAt, c.UpdatedAt
    FROM Courses c
    JOIN Categories cat ON c.CategoryId = cat.CategoryId
    JOIN Users u ON c.InstructorId = u.UserId
END
GO

-- usp_GetCourseById
CREATE PROCEDURE usp_GetCourseById @CourseId INT
AS BEGIN
    SELECT c.CourseId, c.Title, c.Description, c.CategoryId, cat.Name AS CategoryName,
           c.InstructorId, u.FirstName + ' ' + u.LastName AS InstructorName,
           c.ThumbnailUrl, c.Price, c.Level, c.IsPublished, c.CreatedAt, c.UpdatedAt
    FROM Courses c
    JOIN Categories cat ON c.CategoryId = cat.CategoryId
    JOIN Users u ON c.InstructorId = u.UserId
    WHERE c.CourseId = @CourseId
END
GO

-- usp_CreateCourse
CREATE PROCEDURE usp_CreateCourse
    @Title NVARCHAR(200), @Description NVARCHAR(2000), @CategoryId INT, @InstructorId INT,
    @ThumbnailUrl NVARCHAR(500), @Price DECIMAL(10,2), @Level NVARCHAR(20)
AS BEGIN
    INSERT INTO Courses (Title, Description, CategoryId, InstructorId, ThumbnailUrl, Price, Level)
    VALUES (@Title, @Description, @CategoryId, @InstructorId, @ThumbnailUrl, @Price, @Level);
    SELECT SCOPE_IDENTITY();
END
GO

-- usp_UpdateCourse
CREATE PROCEDURE usp_UpdateCourse
    @CourseId INT, @Title NVARCHAR(200), @Description NVARCHAR(2000), @CategoryId INT,
    @ThumbnailUrl NVARCHAR(500), @Price DECIMAL(10,2), @Level NVARCHAR(20), @IsPublished BIT
AS BEGIN
    UPDATE Courses SET Title=@Title, Description=@Description, CategoryId=@CategoryId,
        ThumbnailUrl=@ThumbnailUrl, Price=@Price, Level=@Level, IsPublished=@IsPublished,
        UpdatedAt=GETDATE()
    WHERE CourseId=@CourseId
END
GO

-- usp_DeleteCourse
CREATE PROCEDURE usp_DeleteCourse @CourseId INT
AS BEGIN
    DELETE FROM Courses WHERE CourseId = @CourseId
END
GO

-- usp_GetLeaderboard
CREATE PROCEDURE usp_GetLeaderboard
AS BEGIN
    WITH RankedScores AS (
        SELECT cat.CategoryId, cat.Name AS CategoryName,
               u.UserId, u.Username, u.FirstName, u.LastName,
               MAX(a.Score) AS BestScore,
               ROW_NUMBER() OVER (PARTITION BY cat.CategoryId ORDER BY MAX(a.Score) DESC) AS RowNum
        FROM Attempts a
        JOIN Courses c ON a.CourseId = c.CourseId
        JOIN Categories cat ON c.CategoryId = cat.CategoryId
        JOIN Users u ON a.UserId = u.UserId
        WHERE a.Status = 'Completed'
        GROUP BY cat.CategoryId, cat.Name, u.UserId, u.Username, u.FirstName, u.LastName
    )
    SELECT CategoryId, CategoryName, UserId, Username, FirstName, LastName, BestScore
    FROM RankedScores
    WHERE RowNum = 1
    ORDER BY CategoryName
END
GO

-- usp_GetAllUsers
CREATE PROCEDURE usp_GetAllUsers
AS BEGIN
    SELECT UserId, Username, Email, FirstName, LastName, Role, Bio, ProfileImageUrl, CreatedAt, IsActive
    FROM Users
END
GO

-- usp_GetUserById
CREATE PROCEDURE usp_GetUserById @UserId INT
AS BEGIN
    SELECT UserId, Username, Email, FirstName, LastName, Role, Bio, ProfileImageUrl, CreatedAt, IsActive
    FROM Users WHERE UserId = @UserId
END
GO

-- usp_GetAttemptsByCourse
CREATE PROCEDURE usp_GetAttemptsByCourse @CourseId INT
AS BEGIN
    SELECT a.AttemptId, a.UserId, u.Username, a.CourseId, a.Score, a.MaxScore,
           a.StartedAt, a.CompletedAt, a.Status, a.Notes
    FROM Attempts a JOIN Users u ON a.UserId = u.UserId
    WHERE a.CourseId = @CourseId
END
GO

-- usp_GetAttemptsByUser
CREATE PROCEDURE usp_GetAttemptsByUser @UserId INT
AS BEGIN
    SELECT a.AttemptId, a.UserId, a.CourseId, c.Title AS CourseTitle,
           a.Score, a.MaxScore, a.StartedAt, a.CompletedAt, a.Status, a.Notes
    FROM Attempts a JOIN Courses c ON a.CourseId = c.CourseId
    WHERE a.UserId = @UserId
END
GO

-- usp_CreateAttempt
CREATE PROCEDURE usp_CreateAttempt
    @UserId INT, @CourseId INT, @Score DECIMAL(5,2), @MaxScore DECIMAL(5,2),
    @Status NVARCHAR(20), @Notes NVARCHAR(1000)
AS BEGIN
    INSERT INTO Attempts (UserId, CourseId, Score, MaxScore, Status, Notes, CompletedAt)
    VALUES (@UserId, @CourseId, @Score, @MaxScore, @Status, @Notes,
            CASE WHEN @Status = 'Completed' THEN GETDATE() ELSE NULL END);
    SELECT SCOPE_IDENTITY();
END
GO

-- usp_GetAllCategories
CREATE PROCEDURE usp_GetAllCategories
AS BEGIN
    SELECT CategoryId, Name, Description, CreatedAt FROM Categories ORDER BY Name
END
GO

-- usp_GetInstructors
CREATE PROCEDURE usp_GetInstructors
AS BEGIN
    SELECT UserId, Username, FirstName, LastName FROM Users WHERE Role = 'Instructor' ORDER BY FirstName, LastName
END
GO
