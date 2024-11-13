using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace DataTool
{
    public class OutputManager
    {
        private readonly TextBlock    _outputText;
        private readonly ScrollViewer _outputScroller;

        public OutputManager(TextBlock outputText, ScrollViewer outputScroller)
        {
            _outputText     = outputText ?? throw new ArgumentNullException(nameof(outputText));
            _outputScroller = outputScroller ?? throw new ArgumentNullException(nameof(outputScroller));
        }

        public void AppendMessage(string message, OutputType type = OutputType.Normal)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var textRun = new Run
                {
                    Text       = $"{message}\n",
                    Foreground = GetForegroundBrush(type)
                };

                _outputText.Inlines?.Add(textRun);
                ScrollToBottom();
            });
        }

        private void ScrollToBottom()
        {
            _outputScroller.Offset = new Vector(0, _outputScroller.Extent.Height);
        }

        private static IBrush GetForegroundBrush(OutputType type)
        {
            return type switch
            {
                OutputType.Error   => Brushes.Red,
                OutputType.Success => new SolidColorBrush(Color.Parse("#FF90EE90")), // LightGreen
                _                  => new SolidColorBrush(Color.Parse("#FFE6E6E6"))  // Light Gray
            };
        }
    }
}