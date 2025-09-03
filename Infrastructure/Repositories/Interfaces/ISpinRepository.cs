using System.Data;
using RoletaBrindes.Domain.Models;

namespace RoletaBrindes.Infrastructure.Repositories.Interfaces;

public interface ISpinRepository
{
    Task<int> InsertAsync(Spin s, IDbTransaction? tx = null);
    Task<IReadOnlyList<Spin>> ListAllAsync(IDbTransaction? tx = null);
    Task<bool> DeleteAsync(int spinId, IDbTransaction? tx = null);
}