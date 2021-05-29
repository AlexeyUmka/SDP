using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DAL;
using DAL.Factories;
using DAL.Interfaces;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = GetServiceProvider();
            
            // Uncomment to test EfCore repository
            // TestEfCoreRepository(serviceProvider);
            
            // Uncomment to test connectedRepository repository
            // var connectedRepository = new AdoConnectedRepository<Customer>(serviceProvider.GetService<IDbReaderMapperFactory>(),serviceProvider.GetService<IUnitOfWork>());
            // TestRepository(connectedRepository);
            
            // Uncomment to test disconnectedRepository repository
            // var disconnectedRepository =
            //     new AdoDisconnectedRepository<Customer>(serviceProvider.GetService<IUnitOfWork>());
            // TestRepository(disconnectedRepository);
            
            // Uncomment to test dapperRepository repository
            // var dapperRepository =
            //     new DapperRepository<Customer>(serviceProvider.GetService<IUnitOfWork>());
            // TestRepository(dapperRepository);
        }

        private static void TestRepository(IRepository<Customer> repository)
        {
            Func<IEnumerable<Customer>> getAll = repository.GetAll;
            Console.WriteLine("Before Insert");
            DisplayAll(getAll());
            repository.Insert(new Customer(){Id = 12, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Insert");
            DisplayAll(getAll());
            Console.WriteLine("Before Update");
            DisplayAll(getAll());
            repository.Update(new Customer(){Id = 12, FirstName = "sss", LastName = "sss", Phone = "22"});
            Console.WriteLine("After Update");
            DisplayAll(getAll());
            var r = repository.GetByKey(new {Id = 12});
            Console.WriteLine("get by key 12");
            DisplayAll(new []{r});
            Console.WriteLine("Before Delete");
            DisplayAll(getAll());
            repository.Delete(new {Id = 12});
            Console.WriteLine("After Delete");
            DisplayAll(getAll());
        }

        private static void TestEfCoreRepository(IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetService<IRepository<Customer>>();
            Func<IEnumerable<Customer>> getAll = repository.GetAll;
            Console.WriteLine("Before Insert");
            DisplayAll(getAll());
            repository.Insert(new Customer(){Id = 12, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Insert");
            DisplayAll(getAll());
            Console.WriteLine("Before Update");
            DisplayAll(getAll());
            repository.Update(new Customer(){Id = 12, FirstName = "sss", LastName = "sss", Phone = "22"});
            Console.WriteLine("After Update");
            DisplayAll(getAll());
            var r = repository.GetByKey(12);
            Console.WriteLine("get by key 12");
            DisplayAll(new []{r});
            Console.WriteLine("Before Delete");
            DisplayAll(getAll());
            repository.Delete(12);
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
            services.AddScoped<IUnitOfWork, UnitOfWork>(x => new UnitOfWork(configuration.GetConnectionString("SqlDeliveryDB")))
                .AddTransient(typeof(IRepository<>), typeof(EfCoreRepository<>))
                .AddSingleton<IDbReaderMapperFactory, DbReaderMapperFactory>()
                .AddSingleton(configuration);
        }
    }
}
