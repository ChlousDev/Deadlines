﻿using Deadlines;
using System;
using System.ComponentModel;
using System.Timers;

namespace Deadlines
{
    public class Deadline: INotifyPropertyChanged
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan TimeRemaining => Time - DateTime.Now;



        public Deadline(string name, DateTime time)
        {
            this.Name = name;

            //Time down to minutes part
            this.Time = time.Date;
            this.Time = this.Time.AddHours(time.Hour);
            this.Time = this.Time.AddMinutes(time.Minute);
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