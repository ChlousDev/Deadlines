using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Deadlines
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Timer timer;

        private ObservableCollection<Deadline> deadlines;
        public CollectionViewSource Deadlines { get; private set; }
        public ICommand DeleteDeadlineCommand { get; private set; }
        public ICommand AddDeadlineCommand { get; private set; }
        public ICommand EditDeadlineCommand { get; private set; }

        private string newDeadlineName;
        public string NewDeadlineName
        {
            get
            {
                return this.newDeadlineName;
            }
            set
            {
                if (this.newDeadlineName != value)
                {
                    this.newDeadlineName = value; ;
                    this.OnPropertyChanged(nameof(MainViewModel.NewDeadlineName));
                }
            }
        }

        public DateTime? newDeadlineDateTime { get; set; }
        public DateTime? NewDeadlineDateTime
        {
            get
            {
                return this.newDeadlineDateTime;
            }
            set
            {
                if (this.newDeadlineDateTime != value)
                {
                    this.newDeadlineDateTime = value; ;
                    this.OnPropertyChanged(nameof(MainViewModel.NewDeadlineDateTime));
                }
            }
        }

        public MainViewModel()
        {
            this.deadlines = new ObservableCollection<Deadline>();
            this.Deadlines = new CollectionViewSource();
            this.Deadlines.Source = this.deadlines;
            this.Deadlines.SortDescriptions.Add(new SortDescription(nameof(Deadline.Time), ListSortDirection.Ascending));
            this.LoadDeadlinesFromJson();

            this.timer = new Timer(1000);
            this.timer.Elapsed += Timer_Elapsed;
            this.timer.Start();

            this.DeleteDeadlineCommand = new RelayCommand<object>(DeleteDeadline);
            this.EditDeadlineCommand = new RelayCommand<object>(EditDeadline);
            this.AddDeadlineCommand = new RelayCommand(AddDeadline);
        }

        private void SaveDeadlinesToJson()
        {
            string json = JsonConvert.SerializeObject(this.deadlines.ToList());
            File.WriteAllText("deadlines.json", json);
        }

        private void LoadDeadlinesFromJson()
        {
            List<Deadline> deadlines = new List<Deadline>();
            // load the deadlines from the JSON file
            if (File.Exists("deadlines.json"))
            {
                string json = File.ReadAllText("deadlines.json");
                deadlines = JsonConvert.DeserializeObject<List<Deadline>>(json);
            }

            this.deadlines.Clear();
            foreach(Deadline deadline in deadlines)
            {
                this.deadlines.Add(deadline);
            }
        }

        private void DeleteDeadline(object parameter)
        {
            Deadline deadline = (Deadline)parameter;
            this.deadlines.Remove(deadline);
            this.SaveDeadlinesToJson();
        }

        private void EditDeadline(object parameter)
        {
            Deadline deadline = (Deadline)parameter;
            DeadlineDialog deadlineDialog = new DeadlineDialog(deadline);
            deadlineDialog.Owner = App.Current.MainWindow;
            if (deadlineDialog.ShowDialog() == true)
            {
                this.SaveDeadlinesToJson();
            }
            else
            {
                this.LoadDeadlinesFromJson();
            }
        }

        private void AddDeadline()
        {
            if (!this.NewDeadlineDateTime.HasValue)
            {
                MessageBox.Show("Please select a valid date and time for the deadline.", "Invalid Deadline");
            }
            else if (string.IsNullOrEmpty(this.NewDeadlineName))
            {
                MessageBox.Show("Please enter a name for the deadline", "No Name for the deadline");
            }
            else
            {
                // create a new Deadline object and add it to the list
                Deadline deadline = new Deadline(this.NewDeadlineName, this.NewDeadlineDateTime.Value);
                this.deadlines.Add(deadline);

                // save the deadlines to the JSON file
                this.SaveDeadlinesToJson();

                // reset the UI
                this.NewDeadlineName = "";
                this.NewDeadlineDateTime = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (Deadline deadline in this.deadlines)
                {
                    deadline.UpdateTimeRemaining();
                }
            }
            catch { }
        }
    }
}
