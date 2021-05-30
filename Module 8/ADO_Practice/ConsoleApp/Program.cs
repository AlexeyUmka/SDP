using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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
        static void Main(string[] args)
        {
            var serviceProvider = GetServiceProvider();
            
            // Uncomment to test EfCore repository
            // TestEfCoreRepository(serviceProvider);
            
            // Uncomment to test connectedRepository repository
            // var connectedRepository = new AdoConnectedRepository<Customer>(serviceProvider.GetService<IDbReaderMapperFactory>(),serviceProvider.GetService<IUnitOfWork>());
            // TestRepository(connectedRepository, serviceProvider);
            
            // Uncomment to test disconnectedRepository repository
            // var disconnectedRepository =
            //     new AdoDisconnectedRepository<Customer>(serviceProvider.GetService<IUnitOfWork>());
            // TestRepository(disconnectedRepository, serviceProvider);
            
            // Uncomment to test dapperRepository repository
            // var dapperRepository =
            //     new DapperRepository<Customer>(serviceProvider.GetService<IUnitOfWork>());
            // TestRepository(dapperRepository, serviceProvider);
        }

        private static void TestRepository(IRepository<Customer> repository, ServiceProvider serviceProvider)
        {
            using var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
            Func<IEnumerable<Customer>> getAll = repository.GetAll;
            Console.WriteLine("Before Insert");
            DisplayAll(getAll());
            repository.Insert(new Customer(){Id = Id, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Insert");
            DisplayAll(getAll());
            Console.WriteLine("Before Update");
            DisplayAll(getAll());
            repository.Update(new Customer(){Id = Id, FirstName = "sss", LastName = "sss", Phone = "22"});
            Console.WriteLine("After Update");
            DisplayAll(getAll());
            var r = repository.GetByKey(new {Id = Id});
            Console.WriteLine($"get by key {Id}");
            DisplayAll(new []{r});
            Console.WriteLine("Before Delete");
            DisplayAll(getAll());
            repository.Delete(new {Id = Id});
            Console.WriteLine("After Delete");
            DisplayAll(getAll());
            unitOfWork.Save();
            Console.WriteLine("Begin Transaction Insert");
            repository.Insert(new Customer(){Id = Id, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Transaction Insert");
            DisplayAll(getAll());
            Console.WriteLine("After Transaction Insert Rollback");
            unitOfWork.Rollback();
            DisplayAll(getAll());
        }

        private static void TestEfCoreRepository(IServiceProvider serviceProvider)
        {
            using var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
            var repository = serviceProvider.GetService<IRepository<Customer>>();
            Func<IEnumerable<Customer>> getAll = repository.GetAll;
            Console.WriteLine("Before Insert");
            DisplayAll(getAll());
            repository.Insert(new Customer(){Id = Id, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Insert");
            DisplayAll(getAll());
            Console.WriteLine("Before Update");
            DisplayAll(getAll());
            repository.Update(new Customer(){Id = Id, FirstName = "sss", LastName = "sss", Phone = "22"});
            Console.WriteLine("After Update");
            DisplayAll(getAll());
            var r = repository.GetByKey(Id);
            Console.WriteLine($"get by key {Id}");
            DisplayAll(new []{r});
            Console.WriteLine("Before Delete");
            DisplayAll(getAll());
            repository.Delete(Id);
            Console.WriteLine("After Delete");
            DisplayAll(getAll());
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
