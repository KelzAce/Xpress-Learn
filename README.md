# XpressLearn LMS API

A Learning Management System Web API built with ASP.NET Core 8, Dapper, SQL Server, and Swagger, following **Clean Architecture** principles.

## Features

- **Course Management** – Full CRUD REST API for courses with thumbnail file upload
- **Attempts Tracking** – Track user progress, scores, and timestamps
- **Leaderboard** – Analytics endpoint showing top scorer per course category
- **Users & Categories** – Endpoints for users, instructors, and course categories
- **Swagger UI** – Interactive API documentation available at the root URL
- **Dapper + Stored Procedures** – All database interactions use stored procedures
- **Clean Architecture** – Separated into Domain, Application, Infrastructure, and API layers

## Tech Stack

| Technology | Version |
|---|---|
| ASP.NET Core Web API | 8.0 |
| Swagger (Swashbuckle) | 6.6.2 |
| Dapper | 2.1.35 |
| SQL Server | 2019+ |
| C# | 12 |

## Architecture

The project follows **Clean Architecture** with four layers:

| Layer | Project | Responsibility |
|---|---|---|
| **Domain** | `XpressLearn.Domain` | Entity models (no external dependencies) |
| **Application** | `XpressLearn.Application` | Repository interfaces, application-level DTOs |
| **Infrastructure** | `XpressLearn.Infrastructure` | Dapper repository implementations, database access |
| **API** | `XpressLearn.API` | Controllers, API request DTOs, configuration |

**Dependency flow:** API → Infrastructure → Application → Domain

## Project Structure

```
XpressLearn.slnx
src/
├── XpressLearn.Domain/
│   └── Entities/
│       ├── Course.cs
│       ├── User.cs
│       ├── Category.cs
│       ├── Attempt.cs
│       └── LeaderboardEntry.cs
├── XpressLearn.Application/
│   ├── Interfaces/
│   │   ├── ICourseRepository.cs
│   │   ├── IAttemptRepository.cs
│   │   ├── ILeaderboardRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   └── IUserRepository.cs
│   └── DTOs/
│       └── CreateAttemptRequest.cs
├── XpressLearn.Infrastructure/
│   ├── Repositories/
│   │   ├── CourseRepository.cs
│   │   ├── AttemptRepository.cs
│   │   ├── LeaderboardRepository.cs
│   │   ├── CategoryRepository.cs
│   │   └── UserRepository.cs
│   └── DependencyInjection.cs
└── XpressLearn.API/
    ├── Controllers/
    │   ├── CoursesController.cs
    │   ├── AttemptsController.cs
    │   ├── LeaderboardController.cs
    │   ├── CategoriesController.cs
    │   └── UsersController.cs
    ├── DTOs/
    │   ├── CreateCourseRequest.cs
    │   └── UpdateCourseRequest.cs
    ├── wwwroot/uploads/
    ├── Program.cs
    └── appsettings.json

sql/
├── schema.sql    # Table DDL + 12 stored procedures
└── seed.sql      # Sample data (10 categories, 50 users, 500 courses, 500 attempts)
```

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/courses` | Get all courses |
| `GET` | `/api/courses/{id}` | Get course by ID |
| `POST` | `/api/courses` | Create course (multipart/form-data) |
| `PUT` | `/api/courses/{id}` | Update course (multipart/form-data) |
| `DELETE` | `/api/courses/{id}` | Delete course |
| `GET` | `/api/attempts/course/{courseId}` | Get attempts by course |
| `GET` | `/api/attempts/user/{userId}` | Get attempts by user |
| `POST` | `/api/attempts` | Create attempt |
| `GET` | `/api/leaderboard` | Get leaderboard (top scorer per category) |
| `GET` | `/api/categories` | Get all categories |
| `GET` | `/api/users` | Get all users |
| `GET` | `/api/users/{id}` | Get user by ID |
| `GET` | `/api/users/instructors` | Get all instructors |

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

Update `src/XpressLearn.API/appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=XpressLearnDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Running the Application

```bash
dotnet build XpressLearn.slnx
dotnet run --project src/XpressLearn.API
```

Navigate to `https://localhost:5001` (or the port shown in terminal) to open the Swagger UI.

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
