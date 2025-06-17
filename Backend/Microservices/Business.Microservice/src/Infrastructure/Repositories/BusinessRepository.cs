using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BusinessRepository : Repository<Business>, IBusinessRepository
{
    private readonly BusinessRestaurantContext _context;

    public BusinessRepository(BusinessRestaurantContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Business?> GetByIdAsync(Guid id)
    {
        return await _context.Businesses
            .Include(b => b.BusinessRestaurants)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Business?> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Businesses
            .Include(b => b.BusinessRestaurants)
            .FirstOrDefaultAsync(b => b.OwnerId == ownerId);
    }

    public async Task<IEnumerable<Business>> GetAllAsync()
    {
        return await _context.Businesses
            .Include(b => b.BusinessRestaurants)
            .ToListAsync();
    }

    public async Task<IEnumerable<Business>> GetByOwnerIdListAsync(IEnumerable<string> ownerIds)
    {
        return await _context.Businesses
            .Include(b => b.BusinessRestaurants)
            .Where(b => ownerIds.Contains(b.OwnerId))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Businesses.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> ExistsByOwnerIdAsync(string ownerId)
    {
        return await _context.Businesses.AnyAsync(b => b.OwnerId == ownerId);
    }

    public async Task<Business> AddAsync(Business entity)
    {
        var result = await _context.Businesses.AddAsync(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Business> UpdateAsync(Business entity)
    {
        _context.Businesses.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Business entity)
    {
        _context.Businesses.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Business>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _context.Businesses
            .Include(b => b.BusinessRestaurants)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Businesses.CountAsync();
    }
} 