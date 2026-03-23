using System.Data;
using Dapper;
using XpressLearn.Application.Interfaces;
using XpressLearn.Domain.Entities;

namespace XpressLearn.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly IDbConnection _db;

    public CourseRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        return await _db.QueryAsync<Course>("usp_GetAllCourses", commandType: CommandType.StoredProcedure);
    }

    public async Task<Course?> GetCourseByIdAsync(int courseId)
    {
        return await _db.QueryFirstOrDefaultAsync<Course>(
            "usp_GetCourseById",
            new { CourseId = courseId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateCourseAsync(Course course)
    {
        var result = await _db.QueryFirstOrDefaultAsync<int>(
            "usp_CreateCourse",
            new
            {
                course.Title,
                course.Description,
                course.CategoryId,
                course.InstructorId,
                course.ThumbnailUrl,
                course.Price,
                course.Level
            },
            commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task UpdateCourseAsync(Course course)
    {
        await _db.ExecuteAsync(
            "usp_UpdateCourse",
            new
            {
                course.CourseId,
                course.Title,
                course.Description,
                course.CategoryId,
                course.ThumbnailUrl,
                course.Price,
                course.Level,
                course.IsPublished
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteCourseAsync(int courseId)
    {
        await _db.ExecuteAsync(
            "usp_DeleteCourse",
            new { CourseId = courseId },
            commandType: CommandType.StoredProcedure);
    }
}
