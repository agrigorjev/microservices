using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Mandara.TradeApiService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Dac;
using Testcontainers.MsSql;

namespace Mandara.TradeApiService.UnitTests;

[TestClass]
public class TestDbInitializer
{
    public static string ConnectionString => _dbContainer!.GetConnectionString();
    public const string DatabaseName = "MandaraProducts";

    private readonly static string _dbPassword = $"test{Guid.NewGuid().ToString("d").Substring(1, 8)}";
    private const string BacPacFilePath = @"TestDatabase/MandaraProductsTest.bacpac";

    private static MsSqlContainer? _dbContainer;

    [AssemblyInitialize]
    public static void Init()
    {
        var testcontainersBuilder = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .WithAutoRemove(true);

        _dbContainer = testcontainersBuilder.Build();
        _dbContainer.StartAsync().Wait();

        DacServices dacServices = new(_dbContainer.GetConnectionString());
        using BacPackage bacPac = BacPackage.Load(Path.Combine(AppContext.BaseDirectory, BacPacFilePath));
        dacServices.ImportBacpac(bacPac, DatabaseName);
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        _dbContainer!.StopAsync().Wait();
    }

    public static MandaraEntities CreateContext()
    {
        var connString = ConnectionString.Replace("database=master",
                                       $"database={DatabaseName}",
                                       StringComparison.InvariantCultureIgnoreCase);

        var contextOptionsBuilder = new DbContextOptionsBuilder<MandaraEntities>();
        contextOptionsBuilder.UseSqlServer(connectionString: connString);

        return new MandaraEntities(contextOptionsBuilder.Options);
    }
}
