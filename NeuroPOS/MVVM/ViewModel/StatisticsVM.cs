using Microsoft.Maui.ApplicationModel;
using NeuroPOS.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Transaction = NeuroPOS.MVVM.Model.Transaction;
namespace NeuroPOS.MVVM.ViewModel
{
    #region Helper classes
    public class InventoryBar
    {
        public string Week { get; set; }
        public double Value { get; set; }
    }
    public class CashFlowMonthStat
    {
        public string Month { get; set; }
        public double Value { get; set; }
    }
    #endregion
    public class StatisticsVM : INotifyPropertyChanged
    {
        #region Properties
        private string _salesPctText = "+0%";
        public string SalesPctText
        {
            get => _salesPctText;
            private set { _salesPctText = value; OnPropertyChanged(nameof(SalesPctText)); }
        }
        private Color _salesPctColor = Colors.DarkGreen;
        public Color SalesPctColor
        {
            get => _salesPctColor;
            private set { _salesPctColor = value; OnPropertyChanged(nameof(SalesPctColor)); }
        }
        private string _invPctText = "+0%";
        public string InvPctText
        {
            get => _invPctText;
            private set { _invPctText = value; OnPropertyChanged(nameof(InvPctText)); }
        }
        private Color _invPctColor = Colors.DarkGreen;
        public Color InvPctColor
        {
            get => _invPctColor;
            private set { _invPctColor = value; OnPropertyChanged(nameof(InvPctColor)); }
        }
        private string _pctText = "+0%";
        public string PctText
        {
            get => _pctText;
            private set { _pctText = value; OnPropertyChanged(nameof(PctText)); }
        }
        private Color _pctColor = Colors.DarkGreen;
        public Color PctColor
        {
            get => _pctColor;
            private set { _pctColor = value; OnPropertyChanged(nameof(PctColor)); }
        }
        public ObservableCollection<SalesStat> SalesData { get; } = new();
        public ObservableCollection<InventorySnapshot> InventorySnapshots { get; } = new();
        public InventorySnapshot? TodaySnapshot
        {
            get => InventorySnapshots.FirstOrDefault(s => s.Date.Date == DateTime.Today);
        }
        public ObservableCollection<CashFlowSnapshot> CashFlowSnapshots { get; } = new();
        public DateTime MinDate { get; private set; }
        public DateTime MaxDate { get; private set; }
        public double MaxSales { get; private set; }
        public double YInterval { get; private set; }
        private string _salesSummary = "Last 30 Days";
        public string SalesSummary
        {
            get => _salesSummary;
            private set { _salesSummary = value; OnPropertyChanged(nameof(SalesSummary)); }
        }
        public CashRegister Register { get; private set; } = new();
        public ObservableCollection<InventoryBar> InventoryBars { get; } = new();
        private double _currentInventory;
        public double CurrentInventory
        {
            get => _currentInventory;
            private set { _currentInventory = value; OnPropertyChanged(nameof(CurrentInventory)); }
        }
        public ObservableCollection<CashFlowMonthStat> CashFlowMonthlyBars { get; } = new();
        public double CurrentCashFlow { get; private set; }
        public string CashFlowPctText { get; private set; } = "+0%";
        public Color CashFlowPctColor { get; private set; } = Colors.DarkGreen;
        #endregion

        #region Methods
        public void ComputeStats()
        {
            var today = DateTime.Today;
            var windowStart = today.AddDays(-30);
            var raw = Register.Transactions
                .Where(t => t.TransactionType == "sell" && t.Date >= windowStart)
                .GroupBy(t => t.Date.Date)
                .Select(g => new SalesStat
                {
                    Date = g.Key,
                    Total = g.Sum(t => t.TotalAmount)
                })
                .ToList();
            var padded = FillMissingDays(raw, windowStart, today);
            SalesData.Clear();
            foreach (var p in padded) SalesData.Add(p);
            MinDate = windowStart;
            MaxDate = today;
            var peak = padded.Max(s => s.Total);
            MaxSales = Math.Ceiling(peak / 500d) * 500;
            YInterval = MaxSales > 0 ? MaxSales / 5 : 100;
            OnPropertyChanged(nameof(MinDate));
            OnPropertyChanged(nameof(MaxDate));
            OnPropertyChanged(nameof(MaxSales));
            OnPropertyChanged(nameof(YInterval));
            var currentTotal = padded.Sum(s => s.Total - 0.01);
            var previousTotal = Register.Transactions
                .Where(t => t.TransactionType == "sell"
                            && t.Date < windowStart
                            && t.Date >= windowStart.AddDays(-30))
                .Sum(t => t.TotalAmount);
            var pct = previousTotal == 0
           ? 100
           : (currentTotal - previousTotal) / previousTotal * 100;
            SalesPctText = $"{(pct >= 0 ? "+" : "‑")}{Math.Abs(pct):0.#}%";
            SalesPctColor = pct >= 0 ? Colors.Green : Colors.Red;
            SalesSummary = "Last 30 Days •";
        }
        private static List<SalesStat> FillMissingDays(IEnumerable<SalesStat> raw,
                                                       DateTime start,
                                                       DateTime end)
        {
            var map = raw.ToDictionary(s => s.Date, s => s.Total);
            var list = new List<SalesStat>();
            for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                var value = map.TryGetValue(d, out var v) ? v : 0;
                list.Add(new SalesStat
                {
                    Date = d,
                    Total = value + 0.01
                });
            }
            return list;
        }
        private void BuildInventoryBars()
        {
            var today = DateTime.Today;
            var startRange = today.AddDays(-56);
            var weekly = InventorySnapshots
                .Where(s => s.Date >= startRange)
                .GroupBy(s => s.Date.Date.AddDays(-(int)s.Date.DayOfWeek))
                .OrderBy(g => g.Key)
                .Select(g => new InventoryBar
                {
                    Week = g.Key.ToString("dd/MM"),
                    Value = g.Sum(x => x.TotalValue)
                })
                .ToList();
            while (weekly.Count < 8)
            {
                DateTime padStart;
                if (weekly.Count == 0)
                {
                    padStart = today.Date.AddDays(-(int)today.DayOfWeek);
                }
                else
                {
                    var firstStr = weekly.First().Week;
                    if (!DateTime.TryParseExact(firstStr,
                                                "dd/MM",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out var firstDate))
                    {
                        firstDate = today;
                    }
                    padStart = firstDate.AddDays(-7);
                }
                weekly.Insert(0, new InventoryBar
                {
                    Week = padStart.ToString("dd/MM"),
                    Value = 0
                });
            }
            InventoryBars.Clear();
            foreach (var bar in weekly) InventoryBars.Add(bar);
            CurrentInventory = weekly.Last().Value;
            var prevWeekVal = weekly[^2].Value;
            var pct = prevWeekVal == 0
              ? 100
              : (CurrentInventory - prevWeekVal) / prevWeekVal * 100;
            InvPctText = $"{(pct >= 0 ? "+" : "‑")}{Math.Abs(pct):0.#}%";
            InvPctColor = pct >= 0 ? Colors.Green : Colors.Red;
        }
        private void BuildCashFlowMonthlyBars()
        {
            var today = DateTime.Today;
            var sixMonthsStart = new DateTime(today.Year, today.Month, 1).AddMonths(-5);
            var twelveStart = today.AddMonths(-12);
            var prevTwelve = today.AddMonths(-24);
            var monthTotals = CashFlowSnapshots
                .Where(s => s.Date >= sixMonthsStart)
                .GroupBy(s => new { s.Date.Year, s.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new CashFlowMonthStat
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                    Value = g.Sum(x => x.TotalValue)
                })
                .ToList();
            while (monthTotals.Count < 6)
            {
                var padDate = sixMonthsStart.AddMonths(-(6 - monthTotals.Count));
                monthTotals.Insert(0, new CashFlowMonthStat
                {
                    Month = padDate.ToString("MMM"),
                    Value = 0
                });
            }
            CashFlowMonthlyBars.Clear();
            foreach (var row in monthTotals) CashFlowMonthlyBars.Add(row);
            CurrentCashFlow = CashFlowSnapshots
                .Where(s => s.Date >= twelveStart)
                .Sum(s => s.TotalValue);
            var prevTotal = CashFlowSnapshots
                .Where(s => s.Date >= prevTwelve && s.Date < twelveStart)
                .Sum(s => s.TotalValue);
            var pct = prevTotal == 0
                      ? 100
                      : (CurrentCashFlow - prevTotal) / prevTotal * 100;
            CashFlowPctText = $"{(pct >= 0 ? "+" : "‑")}{Math.Abs(pct):0.#}%";
            CashFlowPctColor = pct >= 0 ? Colors.Green : Colors.Red;
            OnPropertyChanged(nameof(CurrentCashFlow));
            OnPropertyChanged(nameof(CashFlowPctText));
            OnPropertyChanged(nameof(CashFlowPctColor));
        }
        #endregion

        #region Tasks
        public async Task LoadDB()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SalesData.Clear();
                InventorySnapshots.Clear();
                Register = new CashRegister();
                CashFlowSnapshots.Clear();
            });
            var tx = await Task.Run(() => App.TransactionRepo.GetItemsWithChildren());
            var shots = await Task.Run(() => App.InventorySnapShotRepo.GetItems());
            var cashflow = await Task.Run(() => App.CashFlowSnapshotRepo.GetItems());
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Register.Transactions = new List<Transaction>(tx);
                foreach (var s in shots) InventorySnapshots.Add(s);
                foreach (var s in cashflow) CashFlowSnapshots.Add(s);
                ComputeStats();
                BuildInventoryBars();
                OnPropertyChanged(nameof(TodaySnapshot));
                BuildCashFlowMonthlyBars();
            });
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}