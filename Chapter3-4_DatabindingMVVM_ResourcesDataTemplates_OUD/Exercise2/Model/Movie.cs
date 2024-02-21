using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Exercise2.Model
{
    public class Movie : INotifyPropertyChanged
    {
        private int _rating;
        public string Title { get; set; }
        public string OpeningCrawl { get; set; }

        public int Rating
        {
            get => _rating;
            set
            {
                if (value < 1)
                {
                    _rating = 1;
                }
                else if (value > 5)
                {
                    _rating = 5;
                }
                else
                {
                    _rating = value;
                }
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Movie()
        {
            Title = string.Empty;
            OpeningCrawl = string.Empty;
            Rating = 1;
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
