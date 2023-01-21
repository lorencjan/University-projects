using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieDatabase.DAL.Entities
{
    public class Person : EntityBase
    {
        [Column("First Name")]
        public string FirstName { get; set; }
        [Column("Last Name")]
        public string LastName { get; set; }
        public short Age { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }

        [Column("Movies Played In")]
        public ICollection<MovieActor> MoviesPlayedIn { get; set; }
        [Column("Movies Directed")]
        public ICollection<MovieDirector> MoviesDirected { get; set; }

        #region Compare override
        public override bool Equals(object x)
        {
            if (ReferenceEquals(this, x))
                return true;

            if (x is null || !GetType().Equals(x.GetType()))
                return false;

            var person = (Person)x;
            //beware, sequences can be null -> exception, check that
            return Id.Equals(person.Id) &&
                   string.Equals(FirstName, person.FirstName) &&
                   string.Equals(LastName, person.LastName) &&
                   Age.Equals(person.Age) &&
                   (Photo == person.Photo || Photo != null && person.Photo != null && Photo.SequenceEqual(person.Photo)) &&
                   string.Equals(Country, person.Country) &&
                   (MoviesPlayedIn == person.MoviesPlayedIn || MoviesPlayedIn != null && person.MoviesPlayedIn != null && MoviesPlayedIn.OrderBy(m => m.MovieId).SequenceEqual(person.MoviesPlayedIn.OrderBy(m => m.MovieId))) &&
                   (MoviesDirected == person.MoviesDirected || MoviesDirected != null && person.MoviesDirected != null && MoviesDirected.OrderBy(m => m.MovieId).SequenceEqual(person.MoviesDirected.OrderBy(m => m.MovieId)));
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Age.GetHashCode();
                hashCode = (hashCode * 397) ^ (Photo != null ? Photo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MoviesPlayedIn != null ? MoviesPlayedIn.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MoviesDirected != null ? MoviesDirected.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
