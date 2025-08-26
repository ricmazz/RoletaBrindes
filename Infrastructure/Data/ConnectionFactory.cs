using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace RoletaBrindes.Infrastructure.Data;

public interface IConnectionFactory
{
    NpgsqlConnection NewConnection();
}

public class PgConnectionFactory(IConfiguration cfg) : IConnectionFactory
{
    private readonly string _cs = cfg.GetConnectionString("Default")!;
    public NpgsqlConnection NewConnection() => new NpgsqlConnection(_cs);
}