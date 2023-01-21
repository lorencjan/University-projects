using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MovieDatabase.DAL.Enums;

namespace MovieDatabase.DAL.Entities
{
    public class Movie : EntityBase
    {
        [Column("Name")]
        public string OriginalName { get; set; }
        [Column("Czech Translation")]
        public string CzechName { get; set; }
        public Genre Genre { get; set; }
        [Column("Title Photo")]
        public byte[] TitlePhoto { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }

        public ICollection<MovieActor> Actors { get; set; }
        public ICollection<MovieDirector> Directors { get; set; }
        public ICollection<Rating> Ratings { get; set; }

        #region Compare override
        public override bool Equals(object x)
        {
            if (ReferenceEquals(this, x))
                return true;

            if (x is null || !GetType().Equals(x.GetType()))
                return false;

            var movie = (Movie)x;
            //beware, sequences can be null -> exception, check that
            return Id.Equals(movie.Id) &&
                   string.Equals(OriginalName, movie.OriginalName) &&
                   string.Equals(CzechName, movie.CzechName) &&
                   Genre.Equals(movie.Genre) &&
                   (TitlePhoto == movie.TitlePhoto || TitlePhoto != null && movie.TitlePhoto != null && TitlePhoto.SequenceEqual(movie.TitlePhoto)) &&
                   string.Equals(Country, movie.Country) &&
                   Year.Equals(movie.Year) &&
                   Duration.Equals(movie.Duration) &&
                   string.Equals(Description, movie.Description) &&
                   (Actors == movie.Actors || Actors != null && movie.Actors != null && Actors.OrderBy(m => m.PersonId).SequenceEqual(movie.Actors.OrderBy(m => m.PersonId))) &&
                   (Directors == movie.Directors || Directors != null && movie.Directors != null && Directors.OrderBy(m => m.PersonId).SequenceEqual(movie.Directors.OrderBy(m => m.PersonId))) &&
                   (Ratings == movie.Ratings || Ratings != null && movie.Ratings != null && Ratings.OrderBy(m => m.Id).SequenceEqual(movie.Ratings.OrderBy(m => m.Id)));
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (OriginalName != null ? OriginalName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CzechName != null ? CzechName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Genre.GetHashCode();
                hashCode = (hashCode * 397) ^ (TitlePhoto != null ? TitlePhoto.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Year.GetHashCode();
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Actors != null ? Actors.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Directors != null ? Directors.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Ratings != null ? Ratings.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
