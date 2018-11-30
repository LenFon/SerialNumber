using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.EntityFramework.Conventions;
using Orleans.Providers.EntityFramework.Extensions;
using SerialNumber.EntityFrameworkCore;
using SerialNumber.Grains;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SerialNumber.SiloHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Server";
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.Read();
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "SerialNumberApp";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(SerialNumber.Grains.SerialNumberService).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .AddEfGrainStorage<SerialNumberDbContext>("ef")
                .ConfigureServices(services =>
                {
                    services.AddDbContext<SerialNumberDbContext>((sp, options) =>
                    {
                        var connectionString = "Server=(LocalDb)\\MSSQLLocalDB;DataBase=SerialNumberDb;User ID=sa;Password=1";
                        options.UseSqlServer(connectionString);
                    });
                    services.ConfigureGrainStorageOptions<SerialNumberDbContext, SerialNumberService, EntityFrameworkCore.SerialNumber>(options =>
                    {
                        options.UseKey(entity => entity.Name);
                        options.ConfigureIsPersisted(entity => entity.Number > 1);
                    });

                });

            var host = builder.Build();

            using (var scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<SerialNumberDbContext>();

                await db.Database.MigrateAsync();
            }

            await host.StartAsync();
            return host;
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SerialNumberDbContext>
    {
        public SerialNumberDbContext CreateDbContext(string[] args)
        {
            var builder = new SiloHostBuilder()
               .UseLocalhostClustering()
               .Configure<ClusterOptions>(options =>
               {
                   options.ClusterId = "dev";
                   options.ServiceId = "SerialNumberApp";
               })
               .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
               .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(SerialNumber.Grains.SerialNumberService).Assembly).WithReferences())
               .ConfigureLogging(logging => logging.AddConsole())
               .AddEfGrainStorage<SerialNumberDbContext>("ef")
               .ConfigureServices(services =>
               {
                   services.AddDbContext<SerialNumberDbContext>((sp, options) =>
                   {
                       var connectionString = "Server=(LocalDb)\\MSSQLLocalDB;DataBase=SerialNumberDb;User ID=sa;Password=1";
                       options.UseSqlServer(connectionString);
                   });
                   //services.ConfigureGrainStorageOptions<SerialNumberDbContext, SerialNumberService, EntityFrameworkCore.SerialNumber>(options =>
                   //{
                   //    options.UseKey(entity => entity.Name);
                   //});

               });

            var host = builder.Build();

            return host.Services.GetService<SerialNumberDbContext>();
        }
    }
}
