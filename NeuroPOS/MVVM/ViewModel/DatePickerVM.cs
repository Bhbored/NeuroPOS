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


        #region private fields
        private DateTime? _startDate;
        private DateTime? _endDate;
        private string _entityType = "items";
        #endregion


        public DatePickerVM(string entityType = "items")
        {
            _entityType = entityType;
            SelectionChangedCommand = new Command<CalendarSelectionChangedEventArgs>(SelectionChanged);
        }
        #region Properties
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
        #endregion

        #region Methods
        public bool IsDateRangeValid
        {
            get
            {
                try
                {
                    if (!StartDate.HasValue && !EndDate.HasValue)
                        return false;
                    if (StartDate.HasValue && EndDate.HasValue)
                        return StartDate <= EndDate;
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
                        return $"Select date range to filter {_entityType}";
                    if (StartDate.HasValue && !EndDate.HasValue)
                        return $"Filtering {_entityType} for {StartDate.Value:MMM dd, yyyy}";
                    if (!StartDate.HasValue && EndDate.HasValue)
                        return $"Filtering {_entityType} for {EndDate.Value:MMM dd, yyyy}";
                    if (StartDate.HasValue && EndDate.HasValue)
                    {
                        if (StartDate.Value.Date == EndDate.Value.Date)
                            return $"Filtering {_entityType} for {StartDate.Value:MMM dd, yyyy}";
                        return $"Filtering {_entityType} from {StartDate.Value:MMM dd} to {EndDate.Value:MMM dd, yyyy}";
                    }
                    return $"Select date range to filter {_entityType}";
                }
                catch (Exception ex)
                {
                    return $"Select date range to filter {_entityType}";
                }
            }
        }
        public ICommand SelectionChangedCommand { get; }
        public void SetSingleDate(DateTime date)
        {
            StartDate = date;
            EndDate = date;
        }
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
                else if (args.NewValue is null)
                {
                    StartDate = null;
                    EndDate = null;
                }
                else
                {
                    if (args.NewValue is DateTime date)
                    {
                        StartDate = date;
                        EndDate = date;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SelectionChanged: {ex.Message}");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}