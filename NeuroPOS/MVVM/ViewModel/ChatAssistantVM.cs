using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using NeuroPOS.DTO;
using System.Windows.Input;

namespace NeuroPOS.MVVM.ViewModel
{
    public partial class ChatAssistantVM : ObservableObject
    {

        #region Private fields
        private readonly HomeVM _host;
        private readonly Func<string, bool, Frame> _addBubble;
        private readonly Action _closePopup;

        #endregion


        public ChatAssistantVM(
        HomeVM host,
        Func<string, bool, Frame> bubbleAdder,
        Action closePopup)
        {
            _host = host;
            _addBubble = bubbleAdder;
            _closePopup = closePopup;
        }



        [ObservableProperty] private string currentInput;

        [RelayCommand]
        private async Task Send()
        {
            var prompt = CurrentInput?.Trim();
            if (string.IsNullOrWhiteSpace(prompt)) return;
            _addBubble(prompt, true);
            CurrentInput = string.Empty;
            var typingBubble = _addBubble("…", false);

            try
            {
                var raw = await _host._assistant.GetRawAssistantReplyAsync(
                              prompt,
                              _host.Products.Select(p => p.Name),
                              _host.Contacts.Select(c => c.Name));

                AssistantIntent? intent = null;
                try
                {
                    intent = JsonSerializer.Deserialize<AssistantIntent>(
                 raw,
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });

                }
                catch { /* not JSON, treat as chat */ }
                if (intent == null)
                {
                    ((Label)typingBubble.Content).Text = raw;
                    return;
                }
                if (intent.Action == "error")
                {
                    ((Label)typingBubble.Content).Text = "❌ I couldn’t match any products or contacts.";
                    return;
                }
                if (intent != null && intent.Action != "error")
                {
                    await _host.ExecuteIntentAsync(intent);
                    ((Label)typingBubble.Content).Text = "✅ Done";
                }
                else
                {
                    ((Label)typingBubble.Content).Text = raw;
                }
            }
            catch (Exception ex)
            {
                ((Label)typingBubble.Content).Text = $"❌ {ex.Message}";

            }
        }

        public ICommand CloseCommand => new Command(_closePopup);
    }
}