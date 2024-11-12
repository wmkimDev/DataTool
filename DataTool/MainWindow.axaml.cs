using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace DataTool
{
    public enum OutputType
    {
        Normal,
        Error,
        Warning,
        Success,
        Info
    }

    public partial class MainWindow : Window
    {
        private readonly Settings         _settings = new();
        private readonly SettingsManager  _settingsManager;
        private readonly UIControlManager _controlManager;
        private readonly FileDialogHelper _fileDialogHelper;

        public MainWindow()
        {
            InitializeComponent();

            _settingsManager  = new SettingsManager((msg, type) => AppendToOutput(msg, type));
            _controlManager   = new UIControlManager(this);
            _fileDialogHelper = new FileDialogHelper(this);

            var settings = _settingsManager.LoadSettings();
            _controlManager.ApplySettings(settings);
        }

        public void OnClickSaveSettings(object sender, RoutedEventArgs e)
        {
            var settings = _controlManager.GetCurrentSettings();
            _settingsManager.SaveSettings(settings);
        }

        public async void OnClickSearchTablePath(object sender, RoutedEventArgs e) =>
            await _fileDialogHelper.ShowFolderDialog("TablePathTextBox");

        public async void OnClickSearchScriptPath(object sender, RoutedEventArgs e) =>
            await _fileDialogHelper.ShowFolderDialog("ScriptPathTextBox");

        public async void OnClickSearchTableOutputPath(object sender, RoutedEventArgs e) =>
            await _fileDialogHelper.ShowFolderDialog("TableOutputPathTextBox");

        public async void OnClickSearchStringOutputPath(object sender, RoutedEventArgs e) =>
            await _fileDialogHelper.ShowFolderDialog("StringOutputPathTextBox");

        private ValidationBuilder CreateBaseValidationBuilder()
        {
            return new ValidationBuilder(_controlManager.TextBoxes, _controlManager.CheckBoxes);
        }

        public void OnClickExecuteAll(object sender, RoutedEventArgs e)
        {
            var result = CreateBaseValidationBuilder()
                .ValidateAll()
                .Build();

            if (!result.IsValid)
            {
                AppendToOutput(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            // Execute all logic here
        }

        public void OnClickExtractScript(object sender, RoutedEventArgs e)
        {
            var result = CreateBaseValidationBuilder()
                .ValidateTable()
                .ValidateScript()
                .ValidateEnumDefinition()
                .Build();

            if (!result.IsValid)
            {
                AppendToOutput(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            // Extract script logic here
        }

        public void OnClickExtractTable(object sender, RoutedEventArgs e)
        {
            var result = CreateBaseValidationBuilder()
                .ValidateTable()
                .ValidateTableOutput()
                .ValidateEncryption()
                .ValidateEnumDefinition()
                .Build();

            if (!result.IsValid)
            {
                AppendToOutput(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            // Extract table logic here
        }

        public void OnClickExtractString(object sender, RoutedEventArgs e)
        {
            var result = CreateBaseValidationBuilder()
                .ValidateTable()
                .ValidateStringOutput()
                .ValidateStringTableFileName()
                .ValidateEncryption()
                .Build();

            if (!result.IsValid)
            {
                AppendToOutput(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            // Extract string logic here
        }

        private void AppendToOutput(string message, OutputType type = OutputType.Normal)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var outputText = this.FindControl<TextBlock>("OutputText")!;
                var timestamp  = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ";

                var textRun = new Run
                {
                    Text = $"{timestamp}{message}\n",
                    Foreground = type switch
                    {
                        OutputType.Error   => Brushes.Red,
                        _                  => new SolidColorBrush(Color.Parse("#FFE6E6E6"))
                    }
                };

                outputText.Inlines?.Add(textRun);
                var scrollViewer = this.FindControl<ScrollViewer>("OutputScroller")!;
                scrollViewer.Offset = new Vector(0, scrollViewer.Extent.Height);
            });
        }
    }
}
