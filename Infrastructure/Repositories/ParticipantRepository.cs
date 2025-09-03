using System.Data;
using System.Data.Common;
using RoletaBrindes.Infrastructure.Data;
using Dapper;
using RoletaBrindes.Domain.Models;
using RoletaBrindes.Infrastructure.Repositories.Interfaces;

namespace RoletaBrindes.Infrastructure.Repositories;

public class ParticipantRepository(IConnectionFactory f) : IParticipantRepository
{
    public async Task<int> UpsertByPhoneAsync(string name, string phone, IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        var sql = @"INSERT INTO participants(name, phone) VALUES(@name,@phone)
                    ON CONFLICT (phone) DO UPDATE SET name = EXCLUDED.name
                    RETURNING id";
        return await conn.ExecuteScalarAsync<int>(sql, new { name, phone }, tx);
    }
    
    public async Task<bool> HasUserWithPhoneAsync(string phone, IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        var sql = @"SELECT EXISTS(SELECT 1 FROM participants WHERE phone = @phone)";
        return await conn.QuerySingleAsync<bool>(sql, new { phone }, tx);
    }
    
    public async Task<IReadOnlyList<Participant>> ListAllAsync(IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        // FOR UPDATE só se estiver dentro de transação
        var sql = tx is null
            ? "SELECT * FROM participants ORDER BY id"
            : "SELECT * FROM participants ORDER BY id FOR UPDATE";
        var list = await conn.QueryAsync<Participant>(sql, transaction: tx);
        return list.ToList();
    }
}