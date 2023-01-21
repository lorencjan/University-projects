using System.Collections.Generic;
using MovieDatabase.App.Resources.Img;

namespace MovieDatabase.App.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        public byte[] TitlePicture { get; set; }
        public byte[] ListIndent { get; set; }
        public List<string> AppPossibilities { get; set; }
        public HomePageViewModel()
        {
            TitlePicture = Images.TitlePicture;
            ListIndent = Images.ListIndent;
            AppPossibilities = new List<string>()
            {
                "View information about your favourite movies",
                "Add new movies to the database or edit them",
                "See the movie critiques or write a rating yourself",
                "Find out more about the actors and directors of the movies",
                "Modify or create the records of the actors/directors in the same ways as the movies"
            };
        }
    }
}
