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
            if (this.ShowDeadlineDialog(deadline) == true)
            {
                this.SaveDeadlinesToJson();
            }
            else
            {
                this.LoadDeadlinesFromJson();
            }
        }

        private bool? ShowDeadlineDialog(Deadline deadline)
        {
            DeadlineDialog deadlineDialog = new DeadlineDialog(deadline);
            deadlineDialog.Owner = App.Current.MainWindow;
            return deadlineDialog.ShowDialog();            
        }

        private void AddDeadline()
        {
            Deadline deadline = new Deadline("", DateTime.Now);
            if (this.ShowDeadlineDialog(deadline) == true)
            {
                if (!deadline.Time.HasValue)
                {
                    MessageBox.Show("Please select a valid date and time for the deadline.", "Invalid Deadline");
                }
                else if (string.IsNullOrEmpty(deadline.Name))
                {
                    MessageBox.Show("Please enter a name for the deadline", "No Name for the deadline");
                }
                else
                {
                    this.deadlines.Add(deadline);
                    this.SaveDeadlinesToJson();
                }
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
