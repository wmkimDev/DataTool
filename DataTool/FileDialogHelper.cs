using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace DataTool;

public class FileDialogHelper
{
    private readonly Window _window;

    public FileDialogHelper(Window window)
    {
        _window = window;
    }

    public async Task ShowFolderDialog(string textBoxId)
    {
        var result = await _window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions());
        if (result?.Count > 0)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _window.FindControl<TextBox>(textBoxId)!.Text = result[0].Path.LocalPath;
            });
        }
    }
}