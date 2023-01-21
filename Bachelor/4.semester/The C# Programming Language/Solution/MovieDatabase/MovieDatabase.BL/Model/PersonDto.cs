using System;
using System.Linq;
using System.Collections.Generic;

namespace MovieDatabase.BL.Model
{
    public class PersonDto : DtoBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public short Age { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }

        public List<MovieListDto> MoviesPlayedIn { get; set; }
        public List<MovieListDto> MoviesDirected { get; set; }

        #region Compare override
        public override bool Equals(object x)
        {
            if (ReferenceEquals(this, x))
                return true;

            if (x is null || !GetType().Equals(x.GetType()))
                return false;

            var pDto = (PersonDto)x;
            //beware, sequences can be null -> exception, check that
            return Id.Equals(pDto.Id) &&
                   string.Equals(FirstName, pDto.FirstName) &&
                   string.Equals(LastName, pDto.LastName) &&
                   Age.Equals(pDto.Age) &&
                   (Photo == pDto.Photo || Photo != null && pDto.Photo != null && Photo.SequenceEqual(pDto.Photo)) &&
                   string.Equals(Country, pDto.Country) &&
                   (MoviesPlayedIn == pDto.MoviesPlayedIn || MoviesPlayedIn != null && pDto.MoviesPlayedIn != null && MoviesPlayedIn.OrderBy(m => m.Id).SequenceEqual(pDto.MoviesPlayedIn.OrderBy(m => m.Id))) &&
                   (MoviesDirected == pDto.MoviesDirected || MoviesDirected != null && pDto.MoviesDirected != null && MoviesDirected.OrderBy(m => m.Id).SequenceEqual(pDto.MoviesDirected.OrderBy(m => m.Id)));
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
