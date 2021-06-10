using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using DAL;
using DAL.Contexts;
using DAL.Factories;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    class Program
    {
        private const int Id = 12;
        static async Task Main(string[] args)
        {
            var serviceProvider = GetServiceProvider();
            
            // Uncomment to test EfCore repository
            await TestEfCoreRepository(serviceProvider);

            using var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
            // Uncomment to test connectedRepository repository
            var connectedRepository = new AdoConnectedRepository<Customer>(serviceProvider.GetService<IDbReaderMapperFactory>(), unitOfWork);
            await TestRepository(connectedRepository, serviceProvider, unitOfWork);
            
            // Uncomment to test disconnectedRepository repository
            var disconnectedRepository =
                new AdoDisconnectedRepository<Customer>(unitOfWork);
            TestRepository(disconnectedRepository, serviceProvider, unitOfWork);
            
            // Uncomment to test dapperRepository repository
            var dapperRepository =
                new DapperRepository<Customer>(unitOfWork);
            await TestRepository(dapperRepository, serviceProvider, unitOfWork);
        }

        private static async Task TestRepository(IRepository<Customer> repository, ServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            Func<Task<IEnumerable<Customer>>> getAll = repository.GetAll;
            Console.WriteLine("Before Insert");
            DisplayAll(await getAll());
            await repository.Insert(new Customer(){Id = Id, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Insert");
            DisplayAll(await getAll());
            Console.WriteLine("Before Update");
            DisplayAll(await getAll());
            await repository.Update(new Customer(){Id = Id, FirstName = "sss", LastName = "sss", Phone = "22"});
            Console.WriteLine("After Update");
            DisplayAll(await getAll());
            var r = await repository.GetByKey(new {Id = Id});
            Console.WriteLine($"get by key {Id}");
            DisplayAll(new []{r});
            Console.WriteLine("Before Delete");
            DisplayAll(await getAll());
            await repository.Delete(new {Id = Id});
            Console.WriteLine("After Delete");
            DisplayAll(await getAll());
            unitOfWork.Save();
            Console.WriteLine("Begin Transaction Insert");
            await repository.Insert(new Customer(){Id = Id, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Transaction Insert");
            DisplayAll(await getAll());
            Console.WriteLine("After Transaction Insert Rollback");
            unitOfWork.Rollback();
            DisplayAll(await getAll());
        }

        private static async Task TestEfCoreRepository(IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetService<IRepository<Customer>>();
            Func<Task<IEnumerable<Customer>>> getAll = repository.GetAll;
            Console.WriteLine("Before Insert");
            DisplayAll(await getAll());
            await repository.Insert(new Customer(){Id = Id, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Insert");
            DisplayAll(await getAll());
            Console.WriteLine("Before Update");
            DisplayAll(await getAll());
            await repository.Update(new Customer(){Id = Id, FirstName = "sss", LastName = "sss", Phone = "22"});
            Console.WriteLine("After Update");
            DisplayAll(await getAll());
            var r = await repository.GetByKey(Id);
            Console.WriteLine($"get by key {Id}");
            DisplayAll(new []{r});
            Console.WriteLine("Before Delete");
            DisplayAll(await getAll());
            await repository.Delete(Id);
            Console.WriteLine("After Delete");
            DisplayAll(await getAll());
        }

        private static void DisplayAll(IEnumerable<Customer> customers)
        {
            foreach (var c in customers)
            {
                Console.WriteLine($"{c.Id}, {c.FirstName}, {c.LastName}, {c.Phone}");
            }
        }

        private static ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName+"/ConsoleApp","appsettings.json"))
                .Build();
            
            var connectionString = configuration.GetConnectionString("SqlDeliveryDB");
            
            services.AddScoped<IUnitOfWork, UnitOfWork>(x => new UnitOfWork(connectionString))
                .AddTransient(typeof(IRepository<>), typeof(EfCoreRepository<>))
                .AddSingleton<IDbReaderMapperFactory, DbReaderMapperFactory>()
                .AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString))
                .AddSingleton(configuration);
        }
    }
}
