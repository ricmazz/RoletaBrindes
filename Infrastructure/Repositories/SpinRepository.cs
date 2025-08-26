using System.Data;
using System.Data.Common;
using Dapper;
using RoletaBrindes.Domain.Models;
using RoletaBrindes.Infrastructure.Data;
using RoletaBrindes.Infrastructure.Repositories.Interfaces;

namespace RoletaBrindes.Infrastructure.Repositories;

public class SpinRepository(IConnectionFactory f) : ISpinRepository
{
    public async Task<int> InsertAsync(Spin s, IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        var sql = @"INSERT INTO spins(participant_id, gift_id, won) VALUES(@Participant_Id,@Gift_Id,@Won) RETURNING id";
        return await conn.ExecuteScalarAsync<int>(sql, s, tx);
    }
}