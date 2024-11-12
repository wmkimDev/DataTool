using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace DataTool;

public class UIControlManager
{
    private readonly Dictionary<string, TextBox>  _textBoxes  = new();
    private readonly Dictionary<string, CheckBox> _checkBoxes = new();
    private readonly Window                       _window;

    public UIControlManager(Window window)
    {
        _window = window;
        InitializeControls();
    }

    private void InitializeControls()
    {
        var controlIds = new[]
        {
            "TablePathTextBox", "ScriptPathTextBox", "TableOutputPathTextBox",
            "StringOutputPathTextBox", "StringTableFileNameTextBox",
            "EnumDefinitionFileNameTextBox", "EncryptionKeyTextBox"
        };

        var checkboxIds = new[]
        {
            "EnableEnumDefinitionFileName", "EnableEncryptionKey"
        };

        foreach (var id in controlIds)
        {
            var control = _window.FindControl<TextBox>(id)
                          ?? throw new InvalidOperationException($"{id} not found");
            _textBoxes.Add(id, control);
        }

        foreach (var id in checkboxIds)
        {
            var checkbox = _window.FindControl<CheckBox>(id)
                           ?? throw new InvalidOperationException($"{id} not found");
            _checkBoxes.Add(id, checkbox);
        }
    }

    public void ApplySettings(Settings settings)
    {
        _textBoxes["TablePathTextBox"].Text                   = settings.TablePath ?? string.Empty;
        _textBoxes["ScriptPathTextBox"].Text                  = settings.ScriptPath ?? string.Empty;
        _textBoxes["TableOutputPathTextBox"].Text             = settings.TableOutputPath ?? string.Empty;
        _textBoxes["StringOutputPathTextBox"].Text            = settings.StringOutputPath ?? string.Empty;
        _textBoxes["StringTableFileNameTextBox"].Text         = settings.StringTableFileName ?? string.Empty;
        _textBoxes["EnumDefinitionFileNameTextBox"].Text      = settings.EnumDefinitionFileName ?? string.Empty;
        _textBoxes["EncryptionKeyTextBox"].Text               = settings.EncryptionKey ?? string.Empty;
        _checkBoxes["EnableEnumDefinitionFileName"].IsChecked = settings.EnableEnumDefinition;
        _checkBoxes["EnableEncryptionKey"].IsChecked          = settings.EnableEncryption;
    }

    public Settings GetCurrentSettings()
    {
        return new Settings
        {
            TablePath              = _textBoxes["TablePathTextBox"].Text,
            ScriptPath             = _textBoxes["ScriptPathTextBox"].Text,
            TableOutputPath        = _textBoxes["TableOutputPathTextBox"].Text,
            StringOutputPath       = _textBoxes["StringOutputPathTextBox"].Text,
            StringTableFileName    = _textBoxes["StringTableFileNameTextBox"].Text,
            EnableEnumDefinition   = _checkBoxes["EnableEnumDefinitionFileName"].IsChecked ?? false,
            EnumDefinitionFileName = _textBoxes["EnumDefinitionFileNameTextBox"].Text,
            EnableEncryption       = _checkBoxes["EnableEncryptionKey"].IsChecked ?? false,
            EncryptionKey          = _textBoxes["EncryptionKeyTextBox"].Text
        };
    }

    public Dictionary<string, TextBox>  TextBoxes  => _textBoxes;
    public Dictionary<string, CheckBox> CheckBoxes => _checkBoxes;
}