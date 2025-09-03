using System.Data;
using System.Data.Common;
using Dapper;
using RoletaBrindes.Domain.Models;
using RoletaBrindes.Infrastructure.Data;
using RoletaBrindes.Infrastructure.Repositories.Interfaces;

namespace RoletaBrindes.Infrastructure.Repositories;

public class GiftRepository(IConnectionFactory f) : IGiftRepository
{
    public async Task<IReadOnlyList<Gift>> ListActiveAsync(IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        // FOR UPDATE só se estiver dentro de transação
        var sql = tx is null
            ? "SELECT * FROM gifts WHERE is_active AND stock > 0 AND weight > 0 ORDER BY id"
            : "SELECT * FROM gifts WHERE is_active AND stock > 0 AND weight > 0 ORDER BY id FOR UPDATE";
        var list = await conn.QueryAsync<Gift>(sql, transaction: tx);
        return list.ToList();
    }

    public async Task<IReadOnlyList<Gift>> ListAllAsync(IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        // FOR UPDATE só se estiver dentro de transação
        var sql = tx is null
            ? "SELECT * FROM gifts ORDER BY id"
            : "SELECT * FROM gifts ORDER BY id FOR UPDATE";
        var list = await conn.QueryAsync<Gift>(sql, transaction: tx);
        return list.ToList();
    }
    
    public async Task<Gift?> GetByIdAsync(int id, IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        return await conn.QueryFirstOrDefaultAsync<Gift>("SELECT * FROM gifts WHERE id=@id", new { id }, tx);
    }

    public async Task<int> CreateAsync(Gift g, IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        var sql = "INSERT INTO gifts(name,stock,weight,is_active) VALUES(@Name,@Stock,@Weight,TRUE) RETURNING id";
        return await conn.ExecuteScalarAsync<int>(sql, g, tx);
    }

    public async Task<int> UpdateAsync(Gift g, IDbTransaction? tx = null)
    {
        var conn = tx?.Connection ?? f.NewConnection();
        if (conn.State != ConnectionState.Open)
        {
            if (conn is DbConnection dbc) await dbc.OpenAsync();
            else conn.Open();
        }
        var sql = @"UPDATE gifts SET name=@Name, stock=@Stock, weight=@Weight, is_active = (stock > 0 AND weight > 0)
                    WHERE id=@Id";
        return await conn.ExecuteAsync(sql, g, tx);
    }
}