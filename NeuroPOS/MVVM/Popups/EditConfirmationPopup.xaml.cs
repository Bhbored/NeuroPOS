using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Popups
{
    public enum EditConfirmationResult
    {
        Cancel,
        Save,
        Discard
    }

    public partial class EditConfirmationPopup : Popup
    {
        public EditConfirmationResult Result { get; private set; } = EditConfirmationResult.Cancel;

        public EditConfirmationPopup(string message = "Do you want to save your changes before continuing?")
        {
            InitializeComponent();
            MessageLabel.Text = message;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            Result = EditConfirmationResult.Cancel;
            await CloseAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            Result = EditConfirmationResult.Save;
            await CloseAsync();
        }

        private async void OnDiscardClicked(object sender, EventArgs e)
        {
            Result = EditConfirmationResult.Discard;
            await CloseAsync();
        }
    }
}