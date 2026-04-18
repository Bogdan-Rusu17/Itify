using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Specifications;

public sealed class DeviceProjectionSpec : Specification<Device, DeviceRecord>
{
    public DeviceProjectionSpec(bool orderByCreatedAt = false) =>
        Query.Include(d => d.Category)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new()
            {
                Id = d.Id,
                Name = d.Name,
                CategoryId = d.CategoryId,
                CategoryName = d.Category.Name,
                SerialNumber = d.SerialNumber,
                Status = d.Status,
                PurchaseDate = d.PurchaseDate,
            });

    public DeviceProjectionSpec(Guid id) : this() =>
        Query.Where(d => d.Id == id);

    public DeviceProjectionSpec(string? searchExpr) : this(true)
    {
        if (string.IsNullOrWhiteSpace(searchExpr))
        {
            return;
        }

        var engineSearchExpr = $"%{searchExpr.Replace(" ", "%")}%";
        Query.Where(d => EF.Functions.Like(d.Name, engineSearchExpr));
    }
}