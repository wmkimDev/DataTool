using System;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Platform.Storage;

namespace DataTool
{
    public class Settings
    {
        public string? TablePath { get; set; }
        public string? ScriptPath { get; set; }
        public string? TableOutputPath { get; set; }
        public string? StringOutputPath { get; set; }
        public string? StringTableFileName { get; set; }
        public bool EnableEnumDefinition { get; set; }
        public string? EnumDefinitionFileName { get; set; }
        public bool EnableEncryption { get; set; }
        public string? EncryptionKey { get; set; }
    }
    
    public partial class MainWindow : Window
    {
        private const string SettingsFileName = "settings.json";
        private Settings _settings = new Settings();
        
        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }
        
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFileName))
                {
                    var json = File.ReadAllText(SettingsFileName);
                    _settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();

                    // UI에 설정값 적용
                    this.FindControl<TextBox>("TablePathTextBox")!.Text = _settings.TablePath;
                    this.FindControl<TextBox>("ScriptPathTextBox")!.Text = _settings.ScriptPath;
                    this.FindControl<TextBox>("TableOutputPathTextBox")!.Text = _settings.TableOutputPath;
                    this.FindControl<TextBox>("StringOutputPathTextBox")!.Text = _settings.StringOutputPath;
                    this.FindControl<TextBox>("StringTableFileNameTextBox")!.Text = _settings.StringTableFileName;
                    this.FindControl<CheckBox>("EnableEnumDefinitionFileName")!.IsChecked = _settings.EnableEnumDefinition;
                    this.FindControl<TextBox>("EnumDefinitionFileNameTextBox")!.Text = _settings.EnumDefinitionFileName;
                    this.FindControl<CheckBox>("EnableEncryptionKey")!.IsChecked = _settings.EnableEncryption;
                    this.FindControl<TextBox>("EncryptionKeyTextBox")!.Text = _settings.EncryptionKey;

                    AppendToOutput("설정을 성공적으로 불러왔습니다.");
                }
            }
            catch (Exception ex)
            {
                AppendToOutput($"설정을 불러오는 중 오류가 발생했습니다: {ex.Message}");
            }
        }
        
        public void OnClickSaveSettings(object sender, RoutedEventArgs e)
        {
            try
            {
                _settings.TablePath = this.FindControl<TextBox>("TablePathTextBox")!.Text;
                _settings.ScriptPath = this.FindControl<TextBox>("ScriptPathTextBox")!.Text;
                _settings.TableOutputPath = this.FindControl<TextBox>("TableOutputPathTextBox")!.Text;
                _settings.StringOutputPath = this.FindControl<TextBox>("StringOutputPathTextBox")!.Text;
                _settings.StringTableFileName = this.FindControl<TextBox>("StringTableFileNameTextBox")!.Text;
                _settings.EnableEnumDefinition = this.FindControl<CheckBox>("EnableEnumDefinitionFileName")!.IsChecked ?? false;
                _settings.EnumDefinitionFileName = this.FindControl<TextBox>("EnumDefinitionFileNameTextBox")!.Text;
                _settings.EnableEncryption = this.FindControl<CheckBox>("EnableEncryptionKey")!.IsChecked ?? false;
                _settings.EncryptionKey = this.FindControl<TextBox>("EncryptionKeyTextBox")!.Text;

                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFileName, json);

                AppendToOutput("설정이 성공적으로 저장되었습니다.");
            }
            catch (Exception ex)
            {
                AppendToOutput($"설정을 저장하는 중 오류가 발생했습니다: {ex.Message}");
            }
        }
        
        private void AppendToOutput(string message)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var outputText = this.FindControl<TextBox>("OutputText")!;
                outputText.Text       += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                outputText.CaretIndex =  outputText.Text.Length;
            });
        }

        public async void OnClickSearchTablePath(object sender, RoutedEventArgs e)
        {
            var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions());

            if (result?.Count > 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    this.FindControl<TextBox>("TablePathTextBox")!.Text = result[0].Path.LocalPath;
                });
            }
        }

        public async void OnClickSearchScriptPath(object sender, RoutedEventArgs e)
        {
            var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions());

            if (result?.Count > 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    this.FindControl<TextBox>("ScriptPathTextBox")!.Text = result[0].Path.LocalPath;
                });
            }
        }

        public async void OnClickSearchTableOutputPath(object sender, RoutedEventArgs e)
        {
            var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
            });

            if (result?.Count > 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    this.FindControl<TextBox>("TableOutputPathTextBox")!.Text = result[0].Path.LocalPath;
                });
            }
        }
        
        public async void OnClickSearchStringOutputPath(object sender, RoutedEventArgs e)
        {
            var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
            });

            if (result?.Count > 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    this.FindControl<TextBox>("StringOutputPathTextBox")!.Text = result[0].Path.LocalPath;
                });
            }
        }
    }
}