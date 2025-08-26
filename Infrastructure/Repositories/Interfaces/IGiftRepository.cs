using System.Data;
using RoletaBrindes.Domain.Models;

namespace RoletaBrindes.Infrastructure.Repositories.Interfaces;

public interface IGiftRepository
{
    Task<IReadOnlyList<Gift>> ListActiveAsync(IDbTransaction? tx = null);
    Task<Gift?> GetByIdAsync(int id, IDbTransaction? tx = null);
    Task<int> CreateAsync(Gift g, IDbTransaction? tx = null);
    Task<int> UpdateAsync(Gift g, IDbTransaction? tx = null);
}