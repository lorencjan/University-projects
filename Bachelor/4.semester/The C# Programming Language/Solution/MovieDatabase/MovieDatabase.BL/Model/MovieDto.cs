using System;
using System.Linq;
using System.Collections.Generic;
using MovieDatabase.DAL.Enums;

namespace MovieDatabase.BL.Model
{
    public class MovieDto : DtoBase
    {
        public string OriginalName { get; set; }
        public string CzechName { get; set; }
        public Genre Genre { get; set; }
        public byte[] TitlePhoto { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }

        public List<PersonListDto> Actors { get; set; }
        public List<PersonListDto> Directors { get; set; }
        public List<RatingDto> Ratings { get; set; }

        #region Compare override
        public override bool Equals(object x)
        {
            if (ReferenceEquals(this, x))
                return true;

            if (x is null || !GetType().Equals(x.GetType()))
                return false;

            var mDto = (MovieDto)x;
            //beware, sequences can be null -> exception, check that
            return Id.Equals(mDto.Id) &&
                   string.Equals(OriginalName, mDto.OriginalName) &&
                   string.Equals(CzechName, mDto.CzechName) &&
                   Genre.Equals(mDto.Genre) &&
                   (TitlePhoto == mDto.TitlePhoto || TitlePhoto != null && mDto.TitlePhoto != null && TitlePhoto.SequenceEqual(mDto.TitlePhoto)) &&
                   string.Equals(Country, mDto.Country) &&
                   Year.Equals(mDto.Year) &&
                   Duration.Equals(mDto.Duration) &&
                   string.Equals(Description, mDto.Description) &&
                   (Actors == mDto.Actors || Actors != null && mDto.Actors != null && Actors.OrderBy(m => m.Id).SequenceEqual(mDto.Actors.OrderBy(m => m.Id))) &&
                   (Directors == mDto.Directors || Directors != null && mDto.Directors != null && Directors.OrderBy(m => m.Id).SequenceEqual(mDto.Directors.OrderBy(m => m.Id))) &&
                   (Ratings == mDto.Ratings || Ratings != null && mDto.Ratings != null && Ratings.OrderBy(m => m.Id).SequenceEqual(mDto.Ratings.OrderBy(m => m.Id)));
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
