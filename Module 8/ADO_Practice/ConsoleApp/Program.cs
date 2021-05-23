using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
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
                .AddTransient(typeof(IRepository<>), typeof(AdoConnectedRepository<>))
                .AddSingleton<IDbReaderMapperFactory, DbReaderMapperFactory>()
                .AddSingleton(configuration)
                .BuildServiceProvider();
            
            using var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
            var repository = serviceProvider.GetService<IRepository<Customer>>();
            // repository.Insert(new Customer(){Id = 11, FirstName = "ggg", LastName = "ggg"});
            // repository.Update(new Customer(){Id = 11, FirstName = "sss", LastName = "sss"});
            // unitOfWork.Rollback();
            // unitOfWork.CommitTransaction();
            // repository.GetByKey(new {Id = 11});
            // repository.Delete(new {Id = 11});
            // unitOfWork.Save();
            // unitOfWork.Save();
            foreach (var c in repository.GetAll())
            {
                Console.WriteLine($"{c.Id}, {c.FirstName}, {c.LastName}, {c.Phone}");
            }
        }
    }
}
