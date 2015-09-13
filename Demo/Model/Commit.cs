using System;
using System.ComponentModel;

namespace Demo.Model
{
    public class Commit : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Message { get; set; }
        public string User { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string Hash { get; set; }
        public int FilesCount { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
