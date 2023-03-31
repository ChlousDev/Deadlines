using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            this.timer = new Timer(1000);
            this.timer.Elapsed += Timer_Elapsed;
            this.timer.Start();

            this.LoadDeadlinesToJson();

            this.DeleteDeadlineCommand = new RelayCommand<object>(DeleteDeadline);
            this.AddDeadlineCommand = new RelayCommand(AddDeadline);
        }

        private void SaveDeadlinesToJson()
        {
            string json = JsonConvert.SerializeObject(this.deadlines);
            File.WriteAllText("deadlines.json", json);
        }

        private void LoadDeadlinesToJson()
        {
            // load the deadlines from the JSON file
            if (File.Exists("deadlines.json"))
            {
                string json = File.ReadAllText("deadlines.json");
                this.deadlines = JsonConvert.DeserializeObject<ObservableCollection<Deadline>>(json);
            }

            // create a new empty list if the file doesn't exist
            if (this.deadlines == null)
            {
                this.deadlines = new ObservableCollection<Deadline>();
            }

            this.Deadlines = new CollectionViewSource();
            this.Deadlines.Source = this.deadlines;
            this.Deadlines.SortDescriptions.Add(new SortDescription(nameof(Deadline.Time), ListSortDirection.Ascending));
        }

        private void DeleteDeadline(object parameter)
        {
            Deadline deadline = (Deadline)parameter;
            this.deadlines.Remove(deadline);
            this.SaveDeadlinesToJson();
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
            foreach(Deadline deadline in this.deadlines)
            {
                deadline.UpdateTimeRemaining();
            }
        }
    }
}
