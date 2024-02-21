using System.Collections.ObjectModel;
using System.Windows;

namespace Exercise1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IList<Movie> GetDummyMovies()
        {
            var movies = new List<Movie>
            {
                new Movie
                {
                    Title = "The Godfather",
                    Director = "Francis Ford Coppola",
                    ReleaseYear = 1972
                },
                new Movie
                {
                    Title = "The Shawshank Redemption",
                    Director = "Frank Darabont",
                    ReleaseYear = 1994
                }
            };
            return movies;
        }
    }
}