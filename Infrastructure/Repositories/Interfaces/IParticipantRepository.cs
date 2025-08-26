using System.Data;

namespace RoletaBrindes.Infrastructure.Repositories.Interfaces;

public interface IParticipantRepository
{
    Task<int> UpsertByPhoneAsync(string name, string phone, IDbTransaction? tx = null);
}