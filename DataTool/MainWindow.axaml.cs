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
        private readonly Settings _settings = new();
        private readonly SettingsManager _settingsManager;
        private readonly UIControlManager _controlManager;
        private readonly FileDialogHelper _fileDialogHelper;
        private readonly SheetContextCollector _sheetContextCollector;
        private readonly OutputManager _outputManager;

        public MainWindow()
        {
            InitializeComponent();

            var outputText = this.FindControl<TextBlock>("OutputText")!;
            var outputScroller = this.FindControl<ScrollViewer>("OutputScroller")!;
            _outputManager = new OutputManager(outputText, outputScroller);

            _settingsManager = new SettingsManager(_outputManager.AppendMessage);
            _controlManager = new UIControlManager(this);
            _fileDialogHelper = new FileDialogHelper(this);

            var settings = _settingsManager.LoadSettings();
            _controlManager.ApplySettings(settings);
            _sheetContextCollector = new SheetContextCollector(_controlManager, _outputManager);
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

        private ValidationBuilder CreateBaseValidationBuilder() => new(_controlManager);

        public void OnClickExecuteAll(object sender, RoutedEventArgs e)
        {
            var result = CreateBaseValidationBuilder()
                .ValidateAll()
                .Build();

            if (!result.IsValid)
            {
                _outputManager.AppendMessage(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            try
            {
                _outputManager.AppendMessage("전체 추출 작업을 시작합니다...", OutputType.Info);
                _sheetContextCollector.CollectAll();
                _outputManager.AppendMessage("전체 추출 작업이 완료되었습니다.", OutputType.Success);
            }
            catch (Exception exception)
            {
                _outputManager.AppendMessage($"{exception.Message}", OutputType.Error);
            }
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
                _outputManager.AppendMessage(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            try
            {
                _outputManager.AppendMessage("스크립트 추출을 시작합니다...", OutputType.Info);
                // Extract script logic here
                _outputManager.AppendMessage("스크립트 추출이 완료되었습니다.", OutputType.Success);
            }
            catch (Exception exception)
            {
                _outputManager.AppendMessage($"{exception.Message}", OutputType.Error);
            }
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
                _outputManager.AppendMessage(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            try
            {
                _outputManager.AppendMessage("테이블 추출을 시작합니다...", OutputType.Info);
                // Extract table logic here
                _outputManager.AppendMessage("테이블 추출이 완료되었습니다.", OutputType.Success);
            }
            catch (Exception exception)
            {
                _outputManager.AppendMessage($"{exception.Message}", OutputType.Error);
            }
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
                _outputManager.AppendMessage(result.ErrorMessage ?? "알 수 없는 오류가 발생했습니다.", OutputType.Error);
                return;
            }

            try
            {
                _outputManager.AppendMessage("문자열 추출을 시작합니다...", OutputType.Info);
                // Extract string logic here
                _outputManager.AppendMessage("문자열 추출이 완료되었습니다.", OutputType.Success);
            }
            catch (Exception exception)
            {
                _outputManager.AppendMessage($"{exception.Message}", OutputType.Error);
            }
        }
    }
}