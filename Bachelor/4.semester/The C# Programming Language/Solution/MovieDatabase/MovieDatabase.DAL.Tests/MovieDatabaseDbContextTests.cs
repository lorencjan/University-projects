using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;

namespace MovieDatabase.DAL.Tests
{
    public class MovieDatabaseDbContextTests : IClassFixture<MovieDatabaseDbContextTestsFixture>
    {
        public MovieDatabaseDbContextTests(MovieDatabaseDbContextTestsFixture testContext)
        {
            _testContext = testContext;
            _testMovie = new Movie
            {
                //Id = new Guid("ab5c1699-afc6-4744-a824-1ec45c674a6a"),
                OriginalName = "Pulp Fiction",
                CzechName = "Pulp Fiction",
                Genre = Genre.Drama,
                Country = "USA",
                Year = 1994,
                Duration = 154,
                Description = "Starring John Travolta, Samuel L. Jackson, Bruce Willis, Tim Roth, Ving Rhames, and Uma Thurman, it tells several stories of criminal Los Angeles. The title refers to the pulp magazines and hardboiled crime novels popular during the mid-20th century, known for their graphic violence and punchy dialogue.",
            };
            _testActor = new Person
            {
                //Id = new Guid("f10fa054-5ebd-4143-9c5a-8414404bb0c7"),
                FirstName = "Miroslav",
                LastName = "Donutil",
                Age = 69, //Nice
                Photo = Seeds.SeedImages.BradPitt,
                Country = "Czech Republic",
            };
            _testDirector = new Person
            {
                //Id = new Guid("5b3371b2-28f7-4e7b-8e50-ffda2958fc37"),
                FirstName = "Někdo",
                LastName = "Testovaný",
                Age = 48,
                Photo = Seeds.SeedImages.JamesCameron,
                Country = "Austria",
            };
            _testRating = new Rating
            {
                //Id = new Guid("caa63420-d2b0-480e-846c-235415f4b3b3"),
                Number = 7,
                Text = "Nějakej testovací text ratingu"
            };
        }

        private readonly MovieDatabaseDbContextTestsFixture _testContext;
        private Movie _testMovie;
        private Person _testActor;
        private Person _testDirector;
        private Rating _testRating;

        #region Tests for atomic operations CRUD (Create, Read, Update, Remove)
        //Testing Movie Entity
        [Fact]
        public void AddMovieTest()
        {
            //Act
            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            //Assert
            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var retrievedMovie = dbx.Movies.First(entity => entity.Id == _testMovie.Id);
                Assert.Equal(_testMovie, retrievedMovie);
            }
        }

        [Fact]
        public void RemoveMovieTest()
        {
            //Act
            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();
            _testContext.MovieDatabaseDbContextSUT.Movies.Remove(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            //Assert
            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                Assert.Throws<InvalidOperationException>(() => dbx.Movies.First(entity => entity.Id == _testMovie.Id));
            }
        }

        [Fact]
        public void UpdateMovieTest()
        {
            //Act
            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            _testMovie.CzechName = "Pulpová Fikce";

            _testContext.MovieDatabaseDbContextSUT.Movies.Update(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            //Assert
            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var retrievedMovie = dbx.Movies.First(entity => entity.Id == _testMovie.Id);
                Assert.Equal(_testMovie, retrievedMovie);
                Assert.Equal("Pulpová Fikce", retrievedMovie.CzechName);
            }
        }

        //Testing Person Entity
        [Fact]
        public void AddPersonTest()
        {
            _testContext.MovieDatabaseDbContextSUT.People.Add(_testActor);
            _testContext.MovieDatabaseDbContextSUT.People.Add(_testDirector);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var retrievedActor = dbx.People.Find(_testActor.Id);
                Assert.Equal(_testActor, retrievedActor);
                var retrievedDirector = dbx.People.Find(_testDirector.Id);
                Assert.Equal(_testDirector, retrievedDirector);
            }
        }

        [Fact]
        public void RemovePersonTest()
        {
            _testContext.MovieDatabaseDbContextSUT.People.Add(_testActor);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();
            _testContext.MovieDatabaseDbContextSUT.People.Remove(_testActor);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            //Assert
            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                Assert.Throws<InvalidOperationException>(() => dbx.People.First(entity => entity.Id == _testActor.Id));
            }
        }

        [Fact]
        public void UpdatePersonTest()
        {
            _testContext.MovieDatabaseDbContextSUT.People.Add(_testActor);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            _testActor.Age = 42;

            _testContext.MovieDatabaseDbContextSUT.People.Update(_testActor);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            //Assert
            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var retrievedActor = dbx.People.Find(_testActor.Id);
                Assert.Equal(42, retrievedActor.Age);
                Assert.Equal(_testActor, retrievedActor);
            }
        }

        //Testing Rating Entity
        [Fact]
        public void AddRatingTest()
        {
            _testContext.MovieDatabaseDbContextSUT.Ratings.Add(_testRating);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var retr = dbx.Ratings.Find(_testRating.Id);
                Assert.Equal(_testRating, retr);
            }
        }

        [Fact]
        public void RemoveRatingTest()
        {
            _testContext.MovieDatabaseDbContextSUT.Ratings.Add(_testRating);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();
            _testContext.MovieDatabaseDbContextSUT.Ratings.Remove(_testRating);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                Assert.Throws<InvalidOperationException>(() => dbx.Ratings.First(entity => entity.Id == _testRating.Id));
            }
        }

        [Fact]
        public void UpdateRatingTest()
        {
            _testContext.MovieDatabaseDbContextSUT.Ratings.Add(_testRating);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            _testRating.Text = "Changed text";

            _testContext.MovieDatabaseDbContextSUT.Ratings.Update(_testRating);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var retr = dbx.Ratings.Find(_testRating.Id);
                Assert.Equal("Changed text", retr.Text);
                Assert.Equal(_testRating, retr);
            }
        }
        #endregion

        #region Tests for table relations

        [Fact]
        public void AddMovieWithRating()
        {
            _testRating.MovieId = _testMovie.Id;
            _testMovie.Ratings = new List<Rating> { _testRating };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using(var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var movie = dbx.Movies
                    .Include(m => m.Ratings)
                    .First(m => m.Id == _testMovie.Id);

                var rating = dbx.Ratings.First(r => r.Id == _testRating.Id);

                Assert.Equal(rating, _testRating);
                Assert.Equal(movie, _testMovie);
            }
        }

        [Fact]
        public void RemoveRatingFromMovie()
        {
            _testRating.MovieId = _testMovie.Id;
            _testMovie.Ratings = new List<Rating> { _testRating };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();
            _testContext.MovieDatabaseDbContextSUT.Ratings.Remove(_testRating);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var movie = dbx.Movies
                    .Include(m => m.Ratings)
                    .First(m => m.Id == _testMovie.Id);

                Assert.Throws<InvalidOperationException>(() => dbx.Ratings.First(r => r.Id == _testRating.Id));

                Assert.Equal(movie, _testMovie);
            }
        }

        [Fact]
        public void RemoveMovieWithRating()
        {
            _testRating.MovieId = _testMovie.Id;
            _testMovie.Ratings = new List<Rating> { _testRating };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var movie = dbx.Movies
                    .Include(m => m.Ratings)
                    .First(m => m.Id == _testMovie.Id);

                var rating = dbx.Ratings.First(r => r.Id == _testRating.Id);

                Assert.Equal(movie, _testMovie);
                Assert.Equal(rating, _testRating);
            }

            //Should remove both the rating and the movie
            _testContext.MovieDatabaseDbContextSUT.Movies.Remove(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                //Neither of those should contain any elements
                Assert.Throws<InvalidOperationException>(() => dbx.Movies.First(m => m.Id == _testMovie.Id));
                Assert.Throws<InvalidOperationException>(() => dbx.Ratings.First(r => r.Id == _testRating.Id));
            }
        }

        [Fact]
        public void AddMovieWithActorAndDirector()
        {
            _testMovie.Actors = new List<MovieActor>
            {
                new MovieActor
                {
                    Person = _testActor,
                    Movie = _testMovie
                }
            };
            _testMovie.Directors = new List<MovieDirector>
            {
                new MovieDirector
                {
                    Person = _testDirector,
                    Movie = _testMovie
                }
            };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var movie = dbx.Movies
                    .Include(m => m.Actors).ThenInclude(p => p.Person)
                    .Include(m => m.Directors).ThenInclude(p => p.Person)
                    .First(m => m.Id == _testMovie.Id);

                var actor = dbx.MovieActor.First(m => m.PersonId == _testActor.Id);
                var director = dbx.MovieDirector.First(m => m.PersonId == _testDirector.Id);

                Assert.Equal(movie, _testMovie);
                Assert.Equal(actor.PersonId, _testActor.Id);
                Assert.Equal(director.PersonId, _testDirector.Id);
            }
        }

        [Fact]
        public void RemoveActorFromMovie()
        {
            var a = new MovieActor
            {
                Person = _testActor,
                Movie = _testMovie
            };
            _testMovie.Actors = new List<MovieActor>{ a };

            var d = new MovieDirector
            {
                Person = _testDirector,
                Movie = _testMovie
            };
            _testMovie.Directors = new List<MovieDirector> { d };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();
            _testContext.MovieDatabaseDbContextSUT.MovieActor.Remove(a);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var movie = dbx.Movies
                    .Include(m => m.Actors).ThenInclude(p => p.Person)
                    .Include(m => m.Directors).ThenInclude(p => p.Person)
                    .First(m => m.Id == _testMovie.Id);

                Assert.Throws<InvalidOperationException>(() => dbx.MovieActor.First(m => m.PersonId == _testActor.Id));

                var director = dbx.MovieDirector.First(m => m.PersonId == _testDirector.Id);

                Assert.Equal(movie, _testMovie);
                Assert.Empty(movie.Actors);
                Assert.Equal(director.PersonId, _testDirector.Id);
            }
        }

        [Fact]
        public void RemoveDirectorFromMovie()
        {
            var a = new MovieActor
            {
                Person = _testActor,
                Movie = _testMovie
            };
            _testMovie.Actors = new List<MovieActor> { a };

            var d = new MovieDirector
            {
                Person = _testDirector,
                Movie = _testMovie
            };
            _testMovie.Directors = new List<MovieDirector> { d };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();
            _testContext.MovieDatabaseDbContextSUT.MovieDirector.Remove(d);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var movie = dbx.Movies
                    .Include(m => m.Actors).ThenInclude(p => p.Person)
                    .Include(m => m.Directors).ThenInclude(p => p.Person)
                    .First(m => m.Id == _testMovie.Id);

                Assert.Throws<InvalidOperationException>(() => dbx.MovieDirector.First(m => m.PersonId == _testDirector.Id));

                var actor = dbx.MovieActor.First(m => m.PersonId == _testActor.Id);

                Assert.Equal(movie, _testMovie);
                Assert.Empty(movie.Directors);
                Assert.Equal(actor.PersonId, _testActor.Id);
            }
        }

        [Fact]
        public void RemoveMovieWithDirectorAndActor()
        {
            var a = new MovieActor
            {
                Person = _testActor,
                Movie = _testMovie
            };
            _testMovie.Actors = new List<MovieActor> { a };

            var d = new MovieDirector
            {
                Person = _testDirector,
                Movie = _testMovie
            };
            _testMovie.Directors = new List<MovieDirector> { d };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();
            _testContext.MovieDatabaseDbContextSUT.Movies.Remove(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                Assert.Throws<InvalidOperationException>(() => dbx.Movies.First(m => m.Id == _testMovie.Id));
                Assert.Throws<InvalidOperationException>(() => dbx.MovieActor.First(m => m.PersonId == _testActor.Id));
                Assert.Throws<InvalidOperationException>(() => dbx.MovieDirector.First(m => m.PersonId == _testDirector.Id));
            }
        }

        [Fact]
        public void SwapActorAndDirectorOfMovieWithUpdate()
        {
            var a = new MovieActor
            {
                Person = _testActor,
                Movie = _testMovie
            };
            _testMovie.Actors = new List<MovieActor> { a };

            var d = new MovieDirector
            {
                Person = _testDirector,
                Movie = _testMovie
            };
            _testMovie.Directors = new List<MovieDirector> { d };

            var ad = new MovieDirector
            {
                Person = _testActor,
                Movie = _testMovie
            };

            var da = new MovieActor
            {
                Person = _testDirector,
                Movie = _testMovie
            };

            _testContext.MovieDatabaseDbContextSUT.Movies.Add(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            _testMovie.Actors.Remove(a);
            _testMovie.Actors.Add(da);
            _testMovie.Directors.Remove(d);
            _testMovie.Directors.Add(ad);

            _testContext.MovieDatabaseDbContextSUT.Movies.Update(_testMovie);
            _testContext.MovieDatabaseDbContextSUT.SaveChanges();

            using (var dbx = _testContext.CreateMovieDatabaseDbContext())
            {
                var movie = dbx.Movies
                    .Include(m => m.Actors).ThenInclude(p => p.Person)
                    .Include(m => m.Directors).ThenInclude(p => p.Person)
                    .First(m => m.Id == _testMovie.Id);

                var actor = dbx.MovieActor.First(m => m.PersonId == _testDirector.Id);
                var director = dbx.MovieDirector.First(m => m.PersonId == _testActor.Id);

                Assert.Equal(movie, _testMovie);
                Assert.Equal(actor, da);
                Assert.Equal(director, ad);
            }
        }
        #endregion
    }
}