using System;
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
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName+"/ConsoleApp","appsettings.json"))
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddTransient(typeof(IRepository<>), typeof(DapperRepository<>))
                .AddSingleton<IDbReaderMapperFactory, DbReaderMapperFactory>()
                .AddSingleton(configuration)
                .BuildServiceProvider();
            
            var repository = serviceProvider.GetService<IRepository<Customer>>();
            Console.WriteLine("Before Insert");
            DisplayAll(repository);
            repository.Insert(new Customer(){Id = 12, FirstName = "ggg", LastName = "ggg", Phone = "11"});
            Console.WriteLine("After Insert");
            DisplayAll(repository);
            Console.WriteLine("Before Update");
            DisplayAll(repository);
            repository.Update(new Customer(){Id = 12, FirstName = "sss", LastName = "sss", Phone = "22"});
            Console.WriteLine("After Update");
            DisplayAll(repository);
            var r = repository.GetByKey(new {Id = 12}).ToList();
            Console.WriteLine("get by key 12");
            DisplayCustomer(r.Single());
            Console.WriteLine("Before Delete");
            DisplayAll(repository);
            repository.Delete(new {Id = 12});
            Console.WriteLine("After Delete");
            DisplayAll(repository);
        }

        private static void DisplayAll(IRepository<Customer> repository)
        {
            foreach (var c in repository.GetAll())
            {
                DisplayCustomer(c);
            }
        }

        private static void DisplayCustomer(Customer c)
        {
            Console.WriteLine($"{c.Id}, {c.FirstName}, {c.LastName}, {c.Phone}");
        }
    }
}
