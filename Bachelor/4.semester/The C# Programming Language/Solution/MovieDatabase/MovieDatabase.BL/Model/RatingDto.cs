using System;

namespace MovieDatabase.BL.Model
{
    public class RatingDto : DtoBase
    {
        public string Text { get; set; }
        public short Number { get; set; }
        public Guid MovieId { get; set; }
        public string MovieName { get; set; }

        #region Compare override
        public override bool Equals(object x)
        {
            if (ReferenceEquals(this, x))
                return true;

            if (x is null || !GetType().Equals(x.GetType()))
                return false;

            var rDto = (RatingDto)x;
            //beware, sequences can be null -> exception, check that
            return Id.Equals(rDto.Id) &&
                   string.Equals(Text, rDto.Text) &&
                   Number.Equals(rDto.Number) &&
                   MovieId.Equals(rDto.MovieId) &&
                   string.Equals(MovieName, rDto.MovieName);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Number.GetHashCode();
                hashCode = (hashCode * 397) ^ (MovieId != null ? MovieId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MovieName != null ? MovieName.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
