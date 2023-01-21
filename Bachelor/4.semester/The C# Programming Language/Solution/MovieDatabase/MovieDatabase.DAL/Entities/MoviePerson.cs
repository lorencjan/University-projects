using System;

namespace MovieDatabase.DAL.Entities
{
    public abstract class MoviePerson
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; }

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        public override bool Equals(object obj)
        {
            MoviePerson mp = (MoviePerson)obj;
            return PersonId.Equals(mp.PersonId) && MovieId.Equals(mp.MovieId);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PersonId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Person != null ? Person.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MovieId.GetHashCode();
                hashCode = (hashCode * 397) ^ (Movie != null ? Movie.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
