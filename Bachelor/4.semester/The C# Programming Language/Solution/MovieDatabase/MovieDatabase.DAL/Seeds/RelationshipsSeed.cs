using System;
using Microsoft.EntityFrameworkCore;
using MovieDatabase.DAL.Entities;

namespace MovieDatabase.DAL.Seeds
{
    public class RelationshipsSeed
    {
        private static readonly MovieActor[] _movieActor = new MovieActor[]
        {
            new MovieActor
            {
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                PersonId = new Guid("6a6399f6-3a1c-4d60-b2d1-9f5b1b2494b5")
            },
            new MovieActor
            {
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                PersonId = new Guid("f9008976-566d-40ab-b2bc-137bab9fb64d")
            },
            new MovieActor
            {
                MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                PersonId = new Guid("74fa3424-b2e8-470d-92eb-011b75290055")
            },
            new MovieActor
            {
                MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                PersonId = new Guid("dccb08fd-1460-4f20-a26a-7eb8dc6ba5e5")
            },
            new MovieActor
            {
                MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                PersonId = new Guid("0b6ebbbd-30f3-4041-8d8f-9fa7441bb03d")
            },
            new MovieActor
            {
                MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                PersonId = new Guid("0fb32496-f0bb-46f6-a577-476d033225ff")
            },
            new MovieActor
            {
                MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                PersonId = new Guid("74fa3424-b2e8-470d-92eb-011b75290055")
            },
            new MovieActor
            {
                MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                PersonId = new Guid("805c86f2-ab1f-4848-87de-fc5745be91ee")
            },
            new MovieActor
            {
                MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                PersonId = new Guid("6a6399f6-3a1c-4d60-b2d1-9f5b1b2494b5")
            }
        };
        private static readonly MovieDirector[] _movieDirector = new MovieDirector[]
        {
            new MovieDirector
            {
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                PersonId = new Guid("9f64adab-623b-4125-8e03-f64a4bc7edb0")
            },
            new MovieDirector
            {
                MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                PersonId = new Guid("4a95bfcc-0882-4a68-a008-114bbbaf2ba1")
            },
            new MovieDirector
            {
                MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                PersonId = new Guid("e47f9e0d-35b3-47a6-9417-1180546da691")
            }
        };

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieActor>().HasData(_movieActor);
            modelBuilder.Entity<MovieDirector>().HasData(_movieDirector);
        }
    }
}
