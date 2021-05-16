using System;
using System.IO;
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
            Console.WriteLine("Hello World!");
            // Build configuration
            //setup our DI
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName+"/ConsoleApp","appsettings.json"))
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddTransient<IRepository<Customer>, AdoConnectedRepository<Customer>>()
                .AddSingleton<IDbReaderMapperFactory, DbReaderMapperFactory>()
                .AddSingleton(configuration)
                .BuildServiceProvider();

            var repository = serviceProvider.GetService<IRepository<Customer>>();
            foreach (var customer in repository.GetAll())
            {
                Console.WriteLine(customer.FirstName);
            }
        }
    }
}
