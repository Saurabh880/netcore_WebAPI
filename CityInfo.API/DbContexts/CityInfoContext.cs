using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;
using System.ComponentModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> CityInfos { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //we add the appsettings.json file to the configuration sources and then build it

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") // Specify the configuration file to l
                .Build();   //// Build the configuration object, making it ready to retrieve values


            //The GetSection method is used to access a specific section within the configuration file.
            //we are accessing the "ConnectionStrings" section which contains our database connection strings.
            var configSection = configuration.GetSection("ConnectionStrings");

            //Retrieve the connection string value using its key ("SQLServerConnection").
            // The indexer [] is used to access the value corresponding to the "SQLServerConnection" key within the section.
            var ConnectionStrings = configSection["CityInfoDBContextConnectionString"] ?? null;

            optionsBuilder.UseSqlite(ConnectionStrings);
            base.OnConfiguring(optionsBuilder);
        }
        //this can be used to manually construct our model, which is useful if the conventions we used up until now wouldn't be sufficient 
        //or if you prefer to be a little bit more explicit. And it can also be used to provide data for seeding the database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New York City")
                {
                    Id = 1,
                    
                    Description = "The one with that big park."
                },
                new City("Antwerp")
                {
                    Id = 2,
                     
                    Description = "The one with the cathedral that was never really finished."
                },
                new City("Paris")
                {
                    Id = 3,
                    
                    Description = "The one with that big tower."
                });
            modelBuilder.Entity<PointOfInterest>(
                ).HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The most visited urban park in the United States."
                },
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "A 102-story skyscraper located in Midtown Manhattan."
                },
                new PointOfInterest("Cathedral of Our Lady")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "A Gothic style cathedral, conceived by architects Jan and Pieter Appelmans."
                },
                new PointOfInterest("Antwerp Central Station")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "The the finest example of railway architecture in Belgium."
                },
                new PointOfInterest("Eiffel Tower")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
