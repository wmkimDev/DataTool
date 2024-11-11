using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Platform.Storage;

namespace DataTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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