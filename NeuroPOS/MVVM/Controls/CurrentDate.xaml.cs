using Microsoft.Maui.Dispatching;

namespace NeuroPOS.MVVM.Controls;

public partial class CurrentDate : ContentView, IDisposable
{
    private readonly PeriodicTimer _timer;

    public CurrentDate()
    {
        InitializeComponent();

        RightNow.Text = DateTime.Now.ToString("MMMM d, yyyy");
        TimeGauge.Text = DateTime.Now.ToString("hh:mm tt");
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        StartTimer();
    }

    private async void StartTimer()
    {
        while (await _timer.WaitForNextTickAsync())
        {
            RightNow.Text = DateTime.Now.ToString("MMMM d, yyyy");
            TimeGauge.Text = DateTime.Now.ToString("hh:mm tt");
        }
    }

    public void Dispose() => _timer?.Dispose();

}
