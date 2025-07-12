using PropertyChanged;
using Syncfusion.Maui.Calendar;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class DatePickerVM : INotifyPropertyChanged
    {
        public DatePickerVM()
        {
            SelectionChangedCommand = new Command<CalendarSelectionChangedEventArgs>(SelectionChanged);
        }

        private string startDate = string.Empty;
        private string endDate = string.Empty;

        public string StartDate
        {
            get => startDate;
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EndDate
        {
            get => endDate;
            set
            {
                if (endDate != value)
                {
                    endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SelectionChangedCommand { get; }

        private void SelectionChanged(CalendarSelectionChangedEventArgs args)
        {
            if (args.NewValue is CalendarDateRange range)
            {
                StartDate = range.StartDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                EndDate = range.EndDate?.ToString("yyyy-MM-dd") ?? string.Empty;
            }
            else if (args.NewValue is DateTime singleDate)
            {
                StartDate = singleDate.ToString("yyyy-MM-dd");
                EndDate = singleDate.ToString("yyyy-MM-dd");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
