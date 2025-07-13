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
        private DateTime? _startDate;
        private DateTime? _endDate;

        public DatePickerVM()
        {
            SelectionChangedCommand = new Command<CalendarSelectionChangedEventArgs>(SelectionChanged);
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsDateRangeValid));
                    OnPropertyChanged(nameof(FilterSummary));
                }
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsDateRangeValid));
                    OnPropertyChanged(nameof(FilterSummary));
                }
            }
        }

        public bool IsDateRangeValid => StartDate.HasValue && EndDate.HasValue && StartDate <= EndDate;

        public string FilterSummary
        {
            get
            {
                if (!StartDate.HasValue || !EndDate.HasValue)
                    return "Select date range to filter transactions";

                if (StartDate.Value.Date == EndDate.Value.Date)
                    return $"Filtering transactions for {StartDate.Value:MMM dd, yyyy}";

                return $"Filtering transactions from {StartDate.Value:MMM dd} to {EndDate.Value:MMM dd, yyyy}";
            }
        }

        public ICommand SelectionChangedCommand { get; }

        private void SelectionChanged(CalendarSelectionChangedEventArgs args)
        {
            try
            {
                if (args.NewValue is CalendarDateRange range)
                {
                    StartDate = range.StartDate;
                    EndDate = range.EndDate ?? range.StartDate;
                }
                else if (args.NewValue is DateTime singleDate)
                {
                    StartDate = singleDate;
                    EndDate = singleDate;
                }

                Debug.WriteLine($"Date range selected: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SelectionChanged: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
