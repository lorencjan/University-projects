using System;
using MovieDatabase.BL.Model;
using MovieDatabase.DAL;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Factories;
using MovieDatabase.DAL.Seeds;
using MovieDatabase.DAL.Enums;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class RepositoryTests
    {
        private readonly MovieRepositoryFixture _movieRepository;
        private readonly PersonRepositoryFixture _personRepository;
        private readonly RatingRepositoryFixture _ratingRepository;
        public RepositoryTests() 
        {
            MovieDatabaseInMemoryDbContextFactory factory = new MovieDatabaseInMemoryDbContextFactory();
            
            #region Database seeding          
            MovieDatabaseDbContext dbContext = factory.CreateDbContext();
            dbContext.Database.EnsureDeleted();
            Movie[] movies = new Movie[]{
                new Movie
                {
                    Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                    OriginalName = "Titanic",
                    CzechName = "Titanic",
                    Genre = Genre.Catasthrophic,
                    TitlePhoto = SeedImages.Titanic,
                    Country = "USA",
                    Year = 1997,
                    Duration = 194,
                    Description = "Titanic is a 1997 American epic romance and disaster film directed, written, co-produced, and co-edited by James Cameron. Incorporating both historical and fictionalized aspects, the film is based on accounts of the sinking of the RMS Titanic, and stars Leonardo DiCaprio and Kate Winslet as members of different social classes who fall in love aboard the ship during its ill-fated maiden voyage. Cameron's inspiration for the film came from his fascination with shipwrecks; he felt a love story interspersed with the human loss would be essential to convey the emotional impact of the disaster. Production began in 1995, when Cameron shot footage of the actual Titanic wreck. The modern scenes on the research vessel were shot on board the Akademik Mstislav Keldysh, which Cameron had used as a base when filming the wreck. Scale models, computer-generated imagery, and a reconstruction of the Titanic built at Baja Studios were used to re-create the sinking. The film was co-financed by Paramount Pictures and 20th Century Fox; the former handled distribution in North America while the latter released the film internationally. It was the most expensive film ever made at the time, with a production budget of $200 million."
                },
                new Movie
                {
                    Id = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6"),
                    OriginalName = "Troy",
                    CzechName = "Trója",
                    Genre = Genre.Historical,
                    TitlePhoto = SeedImages.Troy,
                    Country = "USA",
                    Year = 2004,
                    Duration = 157,
                    Description = "Troy is a 2004 epic historical war drama film directed by Wolfgang Petersen and written by David Benioff. Produced by units in Malta, Mexico and Britain's Shepperton Studios, the film features an ensemble cast led by Brad Pitt, Eric Bana, and Orlando Bloom. It is loosely based on Homer's Iliad in its narration of the entire story of the decade-long Trojan War—condensed into little more than a couple of weeks, rather than just the quarrel between Achilles and Agamemnon in the ninth year. Achilles leads his Myrmidons along with the rest of the Greek army invading the historical city of Troy, defended by Hector's Trojan army. The end of the film (the sack of Troy) is not taken from the Iliad, but rather from Quintus Smyrnaeus's Posthomerica as the Iliad concludes with Hector's death and funeral. Troy made over $497 million worldwide, temporarily placing it in the #60 spot of top box office hits of all time. It received a nomination for the Academy Award for Best Costume Design and was the 8th highest-grossing film of 2004."
                },
                new Movie
                {
                    Id = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792"),
                    OriginalName = "Once Upon a Time in Hollywood",
                    CzechName = "Tenkrát v Hollywoodu",
                    Genre = Genre.Comedy,
                    TitlePhoto = SeedImages.OnceUponATimeInHollywood,
                    Country = "USA",
                    Year = 2019,
                    Duration = 161,
                    Description = "Once Upon a Time in Hollywood is a 2019 comedy-drama film written and directed by Quentin Tarantino. Produced by Columbia Pictures, Bona Film Group, Heyday Films, and Visiona Romantica and distributed by Sony Pictures Releasing, it is a co-production between the United States and the United Kingdom. It features a large ensemble cast led by Leonardo DiCaprio, Brad Pitt, and Margot Robbie. Set in 1969 Los Angeles, the film follows an actor and his stunt double, as they navigate the changing film industry, and features multiple storylines in a modern fairy tale tribute to the final moments of Hollywood's golden age."
                } 
            };
            Person[] people = new Person[]
            {
                new Person
                {
                    Id = new Guid("9f64adab-623b-4125-8e03-f64a4bc7edb0"),
                    FirstName = "James",
                    LastName = "Cameron",
                    Age = 65,
                    Photo = SeedImages.JamesCameron,
                    Country = "Canada"
                },
                new Person
                {
                    Id = new Guid("6a6399f6-3a1c-4d60-b2d1-9f5b1b2494b5"),
                    FirstName = "Leonardo",
                    LastName = "DiCaprio",
                    Age = 45,
                    Photo = SeedImages.LeonardoDiCaprio,
                    Country = "USA"
                },
                new Person
                {
                    Id = new Guid("f9008976-566d-40ab-b2bc-137bab9fb64d"),
                    FirstName = "Kate",
                    LastName = "Winslet",
                    Age = 44,
                    Photo = SeedImages.KateWinslet,
                    Country = "Great Britain"
                }
            };
            Rating[] ratings = new Rating[]
            {
                new Rating
                {
                    Id = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                    Number = 9,
                    Text = "But it's an inspiring tale of men(!) at war in ancient times. The movie, albeit long, moves along a good pace, with mercifully brief romantic and philosophical breaks between the combat scenes. This movie is action, with more than a little thought put into accurately presenting the realities of the tactics used in Greek warfare. Troy is also to be congratulated for not over-armoring the cast like previous Hollywood productions and staying true to the lightness of armor prevalent during the historical period. Lovers of Homer and Greek mythology may be disappointed but keep in mind this film is about the Trojan War, not the Iliad. This war is epic in scale and isn't about poetry. Still, it would be great if Sean Bean were given the opportunity to play Odysseus again. Although not on screen much in Troy, his performance is edgy and true to the legends of the cunning king of Ithaca.",
                    MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6")
                },
                new Rating
                {
                    Id = new Guid("29f9ee6e-abbb-4cb7-a7f4-48b5adb713b9"),
                    Number = 10,
                    Text = "The sequence between Hector and Achilles is the best choreographed fight scene amongst epic fight movies. Cons for Jaime Abregana Jr. The harmony of the deathliest attacks of the heroes, usage of the camera plans with minimum keeps the sequence to watch in breathless. Until the warning of the shocking rise of the back music with the first death injury of Hector, you completely forget to inhale. If you don't care the out of the plan insertions of the castle balcony people (King/father, wife, Helene etc) the whole fight sequence is planned and played very well. The rest of the cast is good enough with a special note for Peter O'Toole and Brian Cox. Their lines are well delivered and their characters are believable.",
                    MovieId = new Guid("96792e8b-4311-486c-94ee-8a6f8f92d3f6")
                },
                new Rating
                {
                    Id = new Guid("cfbdd89d-6620-4c19-841f-ebd6ef5c4825"),
                    Number = 6,
                    Text = "Cinematography - great, acting - great. Music- reused already written works. Plot - none. Just almost 3+ hours worth of some random scenes stiched together without leading anywhere. Watched it twice. Has not changed my opinion. By the time it ended I did not care anymore, I just wanted to pee!!! After sitting there for almost 3 hours. What was the point of this movie? Still wondering. One of the Tarantino's worst (if not worst!).",
                    MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792")
                }
            };
            foreach(Movie movie in movies)
            {
                dbContext.Add(movie);
            }
            foreach(Person person in people)
            {
                dbContext.Add(person);
            }
            foreach(Rating rating in ratings)
            {
                dbContext.Add(rating);
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
            #endregion

            _movieRepository = new MovieRepositoryFixture(factory);
            _personRepository = new PersonRepositoryFixture(factory);
            _ratingRepository = new RatingRepositoryFixture(factory);
        }

        #region Movie Atomic tests
        [Fact]
        public void MovieInsertAndDelete()
        {
            MovieDto movie= new MovieDto
            {
                OriginalName = "Testovací",
                CzechName = "Film",
                Description = "nějaký film", 
                Country = "Czech Republic",
            }; 
            movie = _movieRepository.InsertOrUpdate(movie);
            Assert.False(movie.Id.Equals(Guid.Empty));
            _movieRepository.Delete(movie.Id);
            Assert.Null(_movieRepository.GetById(movie.Id));
        }

        [Fact]
        public void MovieUpdate()
        {
            MovieDto movie = _movieRepository.GetAll()[0];
            movie.OriginalName = "Test";
            string insertFirstName = movie.OriginalName;
            movie = _movieRepository.InsertOrUpdate(movie);
            Assert.True(insertFirstName == _movieRepository.GetById(movie.Id).OriginalName);
            _movieRepository.Delete(movie.Id);
        }

        [Fact]
        public void MovieGetByNonExistentId()
        {
            var entity = _movieRepository.GetById(new Guid());
            Assert.Null(entity);
        }

        [Fact]
        public void MovieGetByExistentId()
        {
            Guid movieId = _movieRepository.GetAll()[0].Id;
            Assert.NotNull(_movieRepository.GetById(movieId));
        }

        [Fact]
        public void MovieGetAll()
        {
            var movies = _movieRepository.GetAll();
            Assert.True(movies.Count == 3);
        }

        [Fact]
        public void MovieGetAllAsList()
        {
            var movies = _movieRepository.GetAll();
            Assert.True(movies.Count == 3);
        }

        [Fact]
        public void MovieDeleteNonExistent()
        {
            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    _movieRepository.Delete(new Guid());
                });
        }
        #endregion

        #region Person Atomic Tests
        [Fact]
        public void PersonInsertAndDelete()
        {
            PersonDto person= new PersonDto
            {
                FirstName = "Miroslav",
                LastName = "Donutil",
                Age = 69, 
                Country = "Czech Republic",
            }; 
            person = _personRepository.InsertOrUpdate(person);
            Assert.False(person.Id.Equals(Guid.Empty));
            _personRepository.Delete(person.Id);
            Assert.Null(_personRepository.GetById(person.Id));
        }

        [Fact]
        public void PersonUpdate()
        {
            PersonDto person = _personRepository.GetAll()[0];
            person.FirstName = "Arno";
            string insertFirstName = person.FirstName;
            person = _personRepository.InsertOrUpdate(person);
            Assert.True(insertFirstName == _personRepository.GetById(person.Id).FirstName);
            _personRepository.Delete(person.Id);
        }

        [Fact]
        public void PersonGetByNonExistentId()
        {
            var entity = _personRepository.GetById(new Guid());
            Assert.Null(entity);
        }

        [Fact]
        public void PersonGetByExistentId()
        {
            Guid personId = _personRepository.GetAll()[0].Id;
            Assert.NotNull(_personRepository.GetById(personId));
        }

        [Fact]
        public void PersonGetAll()
        {
            var persons = _personRepository.GetAll();
            Assert.True(persons.Count == 3);
        }

        [Fact]
        public void PersonGetAllAsList()
        {
            var persons = _personRepository.GetAll();
            Assert.True(persons.Count == 3);
        }

        [Fact]
        public void PersonDeleteNonExistent()
        {
            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    _personRepository.Delete(new Guid());
                });
        }
        #endregion

        #region Rating Atomic Tests
        [Fact]
        public void RatingInsertAndDelete()
        {
            RatingDto rating= new RatingDto
            {
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                Number = 8,
                Text = "Doufal jsem že se všichni utopí. 8/10 protože se neutopili všichni."
            }; 
            rating = _ratingRepository.InsertOrUpdate(rating);
            Assert.False(rating.Id.Equals(Guid.Empty));

            _ratingRepository.Delete(rating.Id);
            Assert.Null(_ratingRepository.GetById(rating.Id));
        }

        [Fact]
        public void RatingUpdate()
        {
            RatingDto rating = _ratingRepository.GetAll()[0];
            rating.Text = "Still better love story then Twilight";
            string insertText = rating.Text;
            rating = _ratingRepository.InsertOrUpdate(rating);
            Assert.True(insertText == _ratingRepository.GetById(rating.Id).Text);
            _ratingRepository.Delete(rating.Id);
        }

        [Fact]
        public void RatingGetByNonExistentId()
        {
            var entity = _ratingRepository.GetById(new Guid());
            Assert.Null(entity);
        }

        [Fact]
        public void RatingGetByExistentId()
        {
            Guid ratingId = _ratingRepository.GetAll()[0].Id;
            Assert.NotNull(_ratingRepository.GetById(ratingId));
        }

        [Fact]
        public void RatingGetAll()
        {
            var ratings = _ratingRepository.GetAll();
            Assert.True(ratings.Count == 3);
        }

        [Fact]
        public void RatingGetAllAsList()
        {
            var ratings = _ratingRepository.GetAll();
            Assert.True(ratings.Count == 3);
        }

        [Fact]
        public void RatingDeleteNonExistent()
        {
            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    _ratingRepository.Delete(new Guid());
                });
        }
        #endregion

        #region Relationships tests
        [Fact]
        public void MovieAndRatingAddAndRemove()
        {
            MovieDto movie = _movieRepository.GetAll()[0];
            RatingDto rating = new RatingDto()
            {
                MovieId = movie.Id,
                Number = 4,
                Text = "some sample text"
            };
            rating = _ratingRepository.InsertOrUpdate(rating);
            
            movie = _movieRepository.GetById(movie.Id);
            Assert.True(movie.Ratings.Exists(r=>r.Id.Equals(rating.Id)));
            
            _ratingRepository.Delete(rating.Id);
            
            movie = _movieRepository.GetById(movie.Id);
            Assert.False(movie.Ratings.Exists(r=>r.Id.Equals(rating.Id)));
        }

       [Fact]
        public void ChangeRatingMovie()
        {
            RatingDto rating = _ratingRepository.GetAll()[0];
            MovieDto movie = _movieRepository.GetAll().Find(m=>!(m.Id.Equals(rating.MovieId)));

            rating.MovieId = movie.Id;
            rating = _ratingRepository.InsertOrUpdate(rating);

            movie = _movieRepository.GetById(movie.Id);
            Assert.True(movie.Ratings.Exists(r=>r.Id.Equals(rating.Id)));
        }

        [Fact]
        public void RatingMovieSynchroniseTables()
        {
            MovieDto movie = _movieRepository.GetAll()[0];
            RatingDto rating1 = (new RatingDto()
            {
                MovieId = movie.Id,
                Number = 8,
                Text = "TestText"
            });
            RatingDto rating2 = (new RatingDto()
            {
                Number = 5,
                Text = "Text"
            });

            rating1 = _ratingRepository.InsertOrUpdate(rating1);
            movie.Ratings.Add(rating2);
            movie = _movieRepository.InsertOrUpdate(movie);
            
            movie.Ratings.Find(r=>r.Number==rating2.Number&&r.Text==rating2.Text).Text = "testing text";
            movie.Ratings.Remove(rating1);

            movie = _movieRepository.InsertOrUpdate(movie);
            Assert.False(movie.Ratings.Exists(r=>(r.Number==rating1.Number&&r.Text==rating1.Text)||(r.Number==rating2.Number&&r.Text==rating2.Text)));
        }

        [Fact]
        public void MoviesAndActorsAddAndRemove()
        {
            PersonListDto personList = _personRepository.GetAllAsList()[0];
            MovieListDto movieList = _movieRepository.GetAllAsList()[0];
            MovieDto movie = _movieRepository.GetById(movieList.Id);

            movie.Actors.Add(personList);
            movie = _movieRepository.InsertOrUpdate(movie);
            
            PersonDto person = _personRepository.GetById(personList.Id);
            Assert.True(person.MoviesPlayedIn.Exists(m=>m.Id.Equals(movie.Id)));
            
            person.MoviesPlayedIn.Remove(movieList);
            person = _personRepository.InsertOrUpdate(person);

            movie = _movieRepository.GetById(movie.Id);
            Assert.False(movie.Actors.Exists(p=>p.Id.Equals(person.Id)));
        }
        #endregion

    }
}
