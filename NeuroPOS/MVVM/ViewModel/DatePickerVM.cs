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

        public bool IsDateRangeValid
        {
            get
            {
                try
                {
                    // At least one date must be selected
                    if (!StartDate.HasValue && !EndDate.HasValue)
                        return false;

                    // If both dates are selected, ensure StartDate <= EndDate
                    if (StartDate.HasValue && EndDate.HasValue)
                        return StartDate <= EndDate;

                    // If only one date is selected, it's valid
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in IsDateRangeValid: {ex.Message}");
                    return false;
                }
            }
        }

        public string FilterSummary
        {
            get
            {
                try
                {
                    if (!StartDate.HasValue && !EndDate.HasValue)
                        return "Select date range to filter transactions";

                    // If only one date is selected, show it as a single date
                    if (StartDate.HasValue && !EndDate.HasValue)
                        return $"Filtering transactions for {StartDate.Value:MMM dd, yyyy}";

                    if (!StartDate.HasValue && EndDate.HasValue)
                        return $"Filtering transactions for {EndDate.Value:MMM dd, yyyy}";

                    // Both dates are selected
                    if (StartDate.HasValue && EndDate.HasValue)
                    {
                        if (StartDate.Value.Date == EndDate.Value.Date)
                            return $"Filtering transactions for {StartDate.Value:MMM dd, yyyy}";

                        return $"Filtering transactions from {StartDate.Value:MMM dd} to {EndDate.Value:MMM dd, yyyy}";
                    }

                    return "Select date range to filter transactions";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in FilterSummary: {ex.Message}");
                    return "Select date range to filter transactions";
                }
            }
        }

        public ICommand SelectionChangedCommand { get; }

        // Method to manually set a single date (for testing/debugging)
        public void SetSingleDate(DateTime date)
        {
            try
            {
                StartDate = date;
                EndDate = date;
                Debug.WriteLine($"Manually set single date: {date:yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting single date: {ex.Message}");
            }
        }

        private void SelectionChanged(CalendarSelectionChangedEventArgs args)
        {
            try
            {
                Debug.WriteLine($"SelectionChanged called with NewValue type: {args.NewValue?.GetType().Name}");
                Debug.WriteLine($"NewValue: {args.NewValue}");

                if (args.NewValue is CalendarDateRange range)
                {
                    Debug.WriteLine($"CalendarDateRange - StartDate: {range.StartDate}, EndDate: {range.EndDate}");
                    StartDate = range.StartDate;
                    // If EndDate is null, set it to StartDate to ensure we have a valid range
                    EndDate = range.EndDate ?? range.StartDate;
                }
                else if (args.NewValue is DateTime singleDate)
                {
                    Debug.WriteLine($"DateTime singleDate: {singleDate}");
                    StartDate = singleDate;
                    EndDate = singleDate;
                }
                else if (args.NewValue is null)
                {
                    Debug.WriteLine("NewValue is null - clearing dates");
                    StartDate = null;
                    EndDate = null;
                }
                else
                {
                    Debug.WriteLine($"Unknown NewValue type: {args.NewValue.GetType().Name}");
                    // Try to handle as a single date if possible
                    if (args.NewValue is DateTime date)
                    {
                        StartDate = date;
                        EndDate = date;
                    }
                }

                // Ensure both dates are set if at least one is selected
                if (StartDate.HasValue && !EndDate.HasValue)
                {
                    Debug.WriteLine($"Setting EndDate to StartDate: {StartDate.Value}");
                    EndDate = StartDate.Value;
                }
                else if (!StartDate.HasValue && EndDate.HasValue)
                {
                    Debug.WriteLine($"Setting StartDate to EndDate: {EndDate.Value}");
                    StartDate = EndDate.Value;
                }

                Debug.WriteLine($"Final date range: StartDate={StartDate}, EndDate={EndDate}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SelectionChanged: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
