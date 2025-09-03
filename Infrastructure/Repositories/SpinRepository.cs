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
    
    public async Task<IReadOnlyList<Spin>> ListAllAsync(IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        // FOR UPDATE só se estiver dentro de transação
        var sql = tx is null
            ? "SELECT s.id, p.name as Participant_Name, p.phone as Participant_Phone, g.name as Gift_Name, s.won, s.created_at  FROM spins s inner join participants p on p.id = s.participant_id  inner join gifts g on g.id = s.gift_id  ORDER BY s.id"
            : "SELECT s.id, p.name as Participant_Name, p.phone as Participant_Phone, g.name as Gift_Name, s.won, s.created_at  FROM spins s inner join participants p on p.id = s.participant_id  inner join gifts g on g.id = s.gift_id  ORDER BY s.id FOR UPDATE";
        var list = await conn.QueryAsync<Spin>(sql, transaction: tx);
        return list.ToList();
    }

    public async Task<bool> DeleteAsync(int spinId, IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        var sql = @"DELETE FROM spins WHERE id=@Id";
        var affectedRows = await conn.ExecuteAsync(sql, new { Id = spinId }, tx);
        return affectedRows > 0;
    }
}