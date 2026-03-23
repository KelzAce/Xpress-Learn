# XpressLearn LMS

A modern Learning Management System built with ASP.NET Core 8 MVC, Dapper, and SQL Server.

## Features

- **Course Management** – Full CRUD (Create, Read, Update, Delete) for courses with thumbnail image upload
- **Leaderboard** – Category-based leaderboard showing top scorer per category, loaded via async JavaScript fetch
- **Categories** – 10 subject categories (Technology, Business, Design, Marketing, Music, Photography, Health & Fitness, Personal Development, Language, Science)
- **User Roles** – Admin, Instructor, Student roles with role-based data
- **Bootstrap 5** – Responsive UI with tables, badges, progress bars, cards

## Tech Stack

| Technology | Version |
|---|---|
| ASP.NET Core MVC | 8.0 |
| Dapper | 2.1.35 |
| SQL Server | 2019+ |
| Bootstrap | 5.x (via CDN) |
| C# | 12 |

## Project Structure

```
XpressLearn/
├── Controllers/
│   ├── CoursesController.cs      # CRUD for courses + file upload
│   ├── DashboardController.cs    # Leaderboard data endpoint
│   └── HomeController.cs        # Home and error pages
├── Models/
│   ├── User.cs
│   ├── Category.cs
│   ├── Course.cs
│   ├── Attempt.cs
│   ├── LeaderboardEntry.cs
│   └── CourseCreateViewModel.cs
├── Repositories/
│   ├── ICourseRepository.cs / CourseRepository.cs
│   ├── IAttemptRepository.cs / AttemptRepository.cs
│   └── ILeaderboardRepository.cs / LeaderboardRepository.cs
├── Views/
│   ├── Courses/ (Index, Create, Edit)
│   ├── Dashboard/ (Leaderboard)
│   └── Shared/ (_Layout, _ValidationScriptsPartial)
├── wwwroot/uploads/              # Uploaded course thumbnails
├── Program.cs                    # DI setup, middleware pipeline
└── appsettings.json

sql/
├── schema.sql    # Table DDL + 12 stored procedures
└── seed.sql      # Sample data (10 categories, 50 users, 500 courses, 500 attempts)
```

## Database Setup

1. Create a SQL Server database named `XpressLearnDB`
2. Run `sql/schema.sql` to create tables and stored procedures
3. Run `sql/seed.sql` to insert sample data

```sql
-- In SSMS or sqlcmd:
CREATE DATABASE XpressLearnDB;
GO
USE XpressLearnDB;
GO
-- Then run schema.sql and seed.sql
```

## Configuration

Update `appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=XpressLearnDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Running the Application

```bash
cd XpressLearn
dotnet run
```

Navigate to `https://localhost:5001` (or the port shown in terminal).

## Stored Procedures

| Procedure | Description |
|---|---|
| `usp_GetAllCourses` | Returns all courses with category and instructor names |
| `usp_GetCourseById` | Returns a single course by ID |
| `usp_CreateCourse` | Inserts a new course, returns new ID |
| `usp_UpdateCourse` | Updates an existing course |
| `usp_DeleteCourse` | Deletes a course by ID |
| `usp_GetLeaderboard` | Returns top scorer per category (completed attempts) |
| `usp_GetAllUsers` | Returns all users |
| `usp_GetUserById` | Returns a single user by ID |
| `usp_GetAttemptsByCourse` | Returns all attempts for a course |
| `usp_GetAttemptsByUser` | Returns all attempts by a user |
| `usp_CreateAttempt` | Inserts a new attempt, returns new ID |
| `usp_GetAllCategories` | Returns all categories ordered by name |

## Seed Data

| Entity | Count |
|---|---|
| Categories | 10 |
| Users | 50 (2 Admins, 10 Instructors, 38 Students) |
| Courses | 500 (50 per category, all published) |
| Attempts | 500 (90% Completed, 10% InProgress) |
