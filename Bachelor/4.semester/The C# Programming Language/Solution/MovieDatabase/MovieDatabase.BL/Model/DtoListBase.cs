namespace MovieDatabase.BL.Model
{
    public abstract class DtoListBase : DtoBase
    {
        public string DisplayName { get; set; }

        #region Compare
        public override bool Equals(object x)
        {
            if (ReferenceEquals(this, x))
                return true;

            if (x is null || !GetType().Equals(x.GetType()))
                return false;

            var lDto = (DtoListBase)x;
            return Id.Equals(lDto.Id) && string.Equals(DisplayName, lDto.DisplayName);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
