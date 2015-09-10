using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Demo.Model;

namespace Demo.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Commit> _commits;

        public ObservableCollection<Commit> Commits
        {
            get { return _commits; }
            set
            {
                if (Equals(value, _commits)) return;
                _commits = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            this.Commits = new ObservableCollection<Commit>();

            for (int i = 0; i < 10; i++)
            {
                this.Commits.Add(new Commit
                {
                    User = "John Doe",
                    Date = new DateTime(2013, 7, 14),
                    Email = "jdoe@domain.com",
                    FilesCount = 10,
                    Hash = "b5e7a1",
                    Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam quis interdum arcu."
                });

                this.Commits.Add(new Commit
                {
                    User = "John Doe",
                    Date = new DateTime(2013, 7, 9),
                    Email = "jdoe@domain.com",
                    FilesCount = 20,
                    Hash = "0c47a0",
                    Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam quis interdum arcu."
                });
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}