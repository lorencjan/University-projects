using System;
using MovieDatabase.DAL.Entities;
using MovieDatabase.DAL.Enums;
using MovieDatabase.DAL.Seeds;
using MovieDatabase.BL.Model;
using MovieDatabase.BL.Mapping;
using AutoMapper;
using Xunit;

namespace MovieDatabase.BL.Tests
{
    public class RatingMapperTests
    {
        private Rating _rating;
        private RatingDto _ratingDto;
        private Mapper _mapper;

        public RatingMapperTests()
        {
            _rating = new Rating()
            {
                Id = new Guid("8ecf6824-ab94-4e39-b81d-223afd4cbe50"),
                Text = "You can watch this movie in 1997, you can watch it again in 2004 or 2009 or you can watch it in 2015 or 2020, and this movie will get you EVERY TIME. Titanic has made itself FOREVER a timeless classic! I just saw it today (2015) and I was crying my eyeballs out JUST like the first time I saw it back in 1998. This is a movie that is SO touching, SO precise in the making of the boat, the acting and the storyline is BRILLIANT! And the preciseness of the ship makes it even more outstanding! Kate Winslet and Leonardo Dicaprio definitely created a timeless classic that can be watched time and time again and will never get old. This movie will always continue to be a beautiful, painful & tragic movie. 10/10 stars for this masterpiece!",
                Number = 10,
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                Movie = new Movie
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
                }
            };
            _ratingDto = new RatingDto()
            {
                Id = new Guid("8ecf6824-ab94-4e39-b81d-223afd4cbe50"),
                Text = "You can watch this movie in 1997, you can watch it again in 2004 or 2009 or you can watch it in 2015 or 2020, and this movie will get you EVERY TIME. Titanic has made itself FOREVER a timeless classic! I just saw it today (2015) and I was crying my eyeballs out JUST like the first time I saw it back in 1998. This is a movie that is SO touching, SO precise in the making of the boat, the acting and the storyline is BRILLIANT! And the preciseness of the ship makes it even more outstanding! Kate Winslet and Leonardo Dicaprio definitely created a timeless classic that can be watched time and time again and will never get old. This movie will always continue to be a beautiful, painful & tragic movie. 10/10 stars for this masterpiece!",
                Number = 10,
                MovieId = new Guid("ef5fb3cc-f682-4794-84ad-9041c3bf09c5"),
                MovieName = "Titanic"
            };
            _mapper = DtoMapper.CreateMapper();
        }

        [Fact]
        public void Rating2RatingDtoPass()
        {
            var ratingDto = _mapper.Map<Rating, RatingDto>(_rating);
            Assert.Equal(ratingDto, _ratingDto);
        }
        [Fact]
        public void RatingDto2RatingPass()
        {
            var rating = _mapper.Map<RatingDto, Rating>(_ratingDto);
            Assert.Equal(rating, _rating);
        }
        [Fact]
        public void Rating2RatingDtoFail()
        {
            _rating.MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792");
            _rating.Movie = null;
            var ratingDto = _mapper.Map<Rating, RatingDto>(_rating);
            Assert.NotEqual(ratingDto, _ratingDto);
        }
        [Fact]
        public void RatingDto2RatingFail()
        {
            _ratingDto.MovieId = new Guid("558cdffd-2c5e-47f8-b1bd-1140c6388792");
            var rating = _mapper.Map<RatingDto, Rating>(_ratingDto);
            Assert.NotEqual(rating, _rating);
        }

        [Fact]
        public void Rating2RatingListDtoInList()
        {
            var expectedRatingListDto = new RatingListDto()
            {
                Id = _rating.Id,
                DisplayName = $"{_rating.Movie.OriginalName} - {_rating.Number}/10"
            };
            var mappedRatingListDto = _mapper.Map<Rating, RatingListDto>(_rating);
            Assert.Equal(expectedRatingListDto, mappedRatingListDto);
        }
    }
}
