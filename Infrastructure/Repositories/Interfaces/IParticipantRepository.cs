using System.Data;
using RoletaBrindes.Domain.Models;

namespace RoletaBrindes.Infrastructure.Repositories.Interfaces;

public interface IParticipantRepository
{
    Task<int> UpsertByPhoneAsync(string name, string phone, IDbTransaction? tx = null);
    
    Task<bool> HasUserWithPhoneAsync(string phone, IDbTransaction? tx = null);
    Task<IReadOnlyList<Participant>> ListAllAsync(IDbTransaction? tx = null);
}