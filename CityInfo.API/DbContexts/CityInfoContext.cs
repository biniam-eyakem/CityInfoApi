using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options) :
             base(options)
        {
        }
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

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
                    Description = "The one with cathedral that was never really finished."
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one with that big tower."
                });

            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                    new PointOfInterest("Central Park")
                    {
                        Id = 1,
                        CityId = 1,
                        Description = "The most visited urban park in United States."
                    },
                    new PointOfInterest("Empire Building")
                    {
                        Id = 2,
                        CityId = 1,
                        Description = "A 102-store skyscraper located in midtown Manhattan."
                    },
                    new PointOfInterest("Cathedral")
                    {
                        Id = 3,
                        CityId = 2,
                        Description = "A Gothic style cathedral, conceived by architects Jan & Pieter Appelmans"
                    },
                    new PointOfInterest("Antwerp Central Station")
                    {
                        Id = 4,
                        CityId = 2,
                        Description = "The finest example of railway architecture in Belgium"
                    },
                    new PointOfInterest("Effel Tower")
                    {
                        Id = 5,
                        CityId = 3,
                        Description = "A wrought iron lattice tower on the Champs de Mars, named after engineer Gustave Effel."
                    },
                    new PointOfInterest("The Louvre")
                    {
                        Id = 6,
                        CityId = 3,
                        Description = "The world's largest museum."
                    }
                );

            base.OnModelCreating(modelBuilder);
        }

        /*
            // optional configuration using DbContext configuration event OnConiguring
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite("ConnectionString");
                base.OnConfiguring(optionsBuilder);
            }

        */
    }
}
