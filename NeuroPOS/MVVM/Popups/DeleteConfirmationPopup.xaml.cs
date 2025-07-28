using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Popups
{
    public partial class DeleteConfirmationPopup : Popup
    {
        public bool Result { get; private set; } = false;

        public DeleteConfirmationPopup(string message = "Are you sure you want to delete this item?")
        {
            InitializeComponent();
            MessageLabel.Text = message;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            Result = false;
            await CloseAsync();
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            Result = true;
            await CloseAsync();
        }
    }
}