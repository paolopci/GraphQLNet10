using EmployeeManagementGraphQL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementGraphQL.Data.Repositories;

public class ReviewRepository
{
    private readonly EntityDatabaseContext _context;

    public ReviewRepository(EntityDatabaseContext context)
    {
        _context = context;
    }

    public List<Review> GetAllReviews()
    {
        return [.. _context.ReviewEntity
            .AsNoTracking()
            .Include(r => r.Employee)];
    }

    public Review? GetReviewById(int id)
    {
        return _context.ReviewEntity
            .AsNoTracking()
            .Include(r => r.Employee)
            .FirstOrDefault(r => r.Id == id);
    }

    public List<Review> GetReviewsByEmployeeId(int employeeId)
    {
        return [.. _context.ReviewEntity
            .AsNoTracking()
            .Include(r => r.Employee)
            .Where(r => r.EmployeeId == employeeId)];
    }

    public bool EmployeeExists(int employeeId)
    {
        return _context.EmployeeEntity.Any(e => e.Id == employeeId);
    }

    public Review AddReview(Review review)
    {
        _context.ReviewEntity.Add(review);
        _context.SaveChanges();
        return review;
    }

    public Review? UpdateReview(int id, Review review)
    {
        var existingReview = _context.ReviewEntity.FirstOrDefault(r => r.Id == id);
        if (existingReview is null)
        {
            return null;
        }

        existingReview.Rate = review.Rate;
        existingReview.Comment = review.Comment;
        existingReview.EmployeeId = review.EmployeeId;

        _context.SaveChanges();

        return _context.ReviewEntity
            .AsNoTracking()
            .Include(r => r.Employee)
            .FirstOrDefault(r => r.Id == id);
    }

    public bool DeleteReview(int id)
    {
        var review = _context.ReviewEntity.Find(id);
        if (review is null)
        {
            return false;
        }

        _context.ReviewEntity.Remove(review);
        _context.SaveChanges();
        return true;
    }
}
