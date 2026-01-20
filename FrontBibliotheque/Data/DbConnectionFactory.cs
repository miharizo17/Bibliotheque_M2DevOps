using Npgsql;
using Microsoft.Extensions.Configuration;

public class DbConnectionFactory
{
    private readonly IConfiguration _config;

    public DbConnectionFactory(IConfiguration config)
    {
        _config = config;
    }

    public NpgsqlConnection Create()
    {
        return new NpgsqlConnection(
            _config.GetConnectionString("DefaultConnection")
        );
    }
}
