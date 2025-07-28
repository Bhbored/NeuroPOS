using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using NeuroPOS.MVVM.ViewModel;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Popups
{
    public partial class ChatAssistantPopup : Popup
    {
        public ChatAssistantPopup(HomeVM hostVm)
        {
            InitializeComponent();
            BindingContext = new ChatAssistantVM(
                     hostVm,
                     AddBubble,
                     Close);

        }

        private void Close()
        {
            CloseAsync();
        }

        private Frame AddBubble(string text, bool fromUser)
        {
            var bubble = new Frame
            {
                BackgroundColor = fromUser ? Color.FromRgb(0, 122, 255) : Color.FromRgb(240, 240, 240),
                Padding = new Thickness(12, 8),
                CornerRadius = 14,
                HasShadow = false,
                HorizontalOptions = fromUser ? LayoutOptions.End : LayoutOptions.Start,
                Content = new Label
                {
                    Text = text,
                    FontSize = 15,
                    TextColor = fromUser ? Colors.White : Colors.Black
                }
            };

            MessagesStack.Children.Add(bubble);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(50);
                await ChatScroll.ScrollToAsync(bubble, ScrollToPosition.End, true);
            });

            return bubble;
        }

    }
}


