using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Popups
{
    public enum AddProductResult
    {
        Cancel,
        Add
    }

    public partial class AddProductPopup : Popup
    {
        public AddProductResult Result { get; private set; } = AddProductResult.Cancel;

        public AddProductPopup()
        {
            InitializeComponent();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            Result = AddProductResult.Cancel;
            await CloseAsync();
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(ProductNameEntry.Text))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Please enter a product name.", "OK");
                return;
            }

            if (CategoryPicker.SelectedItem == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Please select a category.", "OK");
                return;
            }

            Result = AddProductResult.Add;
            await CloseAsync();
        }

        private async void OnSelectImageClicked(object sender, EventArgs e)
        {
            try
            {
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.image" } },
                    { DevicePlatform.Android, new[] { "image/*" } },
                    { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" } },
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "public.image" } }
                });

                var options = new PickOptions
                {
                    PickerTitle = "Select Product Image",
                    FileTypes = customFileType
                };

                var result = await FilePicker.Default.PickAsync(options);

                if (result != null)
                {
                    // Generate unique filename
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var extension = Path.GetExtension(result.FileName);
                    var newFileName = $"product_{timestamp}{extension}";

                    // Use the local app data directory
                    var localPath = Path.Combine(FileSystem.Current.AppDataDirectory, "ProductImages");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }

                    var destinationPath = Path.Combine(localPath, newFileName);

                    // Copy the file
                    using (var sourceStream = await result.OpenReadAsync())
                    using (var destinationStream = File.Create(destinationPath))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }

                    // Update the ViewModel with the new image path
                    if (BindingContext is NeuroPOS.MVVM.ViewModel.InventoryVM viewModel)
                    {
                        viewModel.NewProductImageUrl = destinationPath;
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to select image: {ex.Message}", "OK");
            }
        }
    }
}