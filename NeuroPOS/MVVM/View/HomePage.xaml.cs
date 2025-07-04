using NeuroPOS.MVVM.ViewModel;
namespace NeuroPOS.MVVM.View;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();

        BindingContext = new HomeVM();
        this.listView.ItemGenerator = new Animation.ItemGeneratorExt(this.listView);
    }
}