using Deadlines;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;

namespace Deadlines
{
    public class Deadline : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(nameof(Deadline.Name));

                }
            }
        }

        private DateTime? time;
        public DateTime? Time
        {
            get
            {
                return this.time;
            }
            set
            {
                if (this.time != value)
                {
                    this.time = value;
                    this.OnPropertyChanged(nameof(Deadline.Time));

                }
            }
        }

        [JsonIgnore]
        public string TimeRemaining
        {
            get
            {
                TimeSpan timeRemaining = this.Time.HasValue ? this.Time.Value - DateTime.Now : TimeSpan.Zero;
                if (timeRemaining < TimeSpan.Zero)
                {
                    timeRemaining = TimeSpan.Zero;
                }
                string timeRemainingString = timeRemaining.ToString(@"%d' days 'hh\:mm\:ss");
                if (timeRemaining.Days < 1)
                {
                    {
                        timeRemainingString = timeRemaining.ToString(@"hh\:mm\:ss");
                    }
                }
                return timeRemainingString;
            }
        }

        [JsonIgnore]
        public List<Color> ColorItemsSource
        {
            get
            {
                return new List<Color>() { new Color("None", "#00000000"), new Color("Blue", "#FF7799FF"), new Color("Red", "#FFFF9999"), new Color("Green", "#FF88CC88"), new Color("Yellow", "#FFFFFF88"), new Color("Purple", "#FFCC99CC"), new Color("Teal", "#FF33DDDD") };
            }
        }

        private string color;
        public string Color
        {
            get
            {
                return this.color;
            }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.OnPropertyChanged(nameof(Deadline.Color));

                }
            }
        }

        public Deadline(string name, DateTime time)
        {
            this.Name = name;
           
            //Time down to minutes part
            this.Time = time.Date;
            this.Time = this.Time.Value.AddHours(time.Hour);
            this.Time = this.Time.Value.AddMinutes(time.Minute);

            this.Color = "#00000000";
        }

        public void UpdateTimeRemaining()
        {
            // Notify UI that TimeRemaining property has changed
            App.Current.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(TimeRemaining));
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}