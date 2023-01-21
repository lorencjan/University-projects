using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieDatabase.DAL.Entities
{
    public class Rating : EntityBase
    {
        public string Text { get; set; }
        [Range(0, 10)]
        public short Number { get; set; }

        //Many to one relationship with Movie entity
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }

        #region Compare override
        public override bool Equals(object x)
        {
            if (ReferenceEquals(this, x))
                return true;

            if (x is null || !GetType().Equals(x.GetType()))
                return false;

            var rating = (Rating)x;
            //beware, sequences can be null -> exception, check that
            return Id.Equals(rating.Id) &&
                   string.Equals(Text, rating.Text) &&
                   Number.Equals(rating.Number) &&
                   MovieId.Equals(rating.MovieId);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Number.GetHashCode();
                hashCode = (hashCode * 397) ^ (MovieId != null ? MovieId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Movie != null ? Movie.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
