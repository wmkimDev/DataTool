using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace DataTool;

public enum TextBoxId
{
    TablePath,
    ScriptPath,
    TableOutputPath,
    StringOutputPath,
    StringTableFileName,
    EnumDefinitionFileName,
    EncryptionKey
}

public enum CheckBoxId
{
    EnableEnumDefinitionFileName,
    EnableEncryptionKey
}

public class UIControlManager
{
    private readonly Dictionary<TextBoxId, TextBox> _textBoxes = new();
    private readonly Dictionary<CheckBoxId, CheckBox> _checkBoxes = new();
    private readonly Window _window;

    public UIControlManager(Window window)
    {
        _window = window;
        InitializeControls();
    }

    private void InitializeControls()
    {
        // TextBox 초기화
        foreach (TextBoxId id in Enum.GetValues<TextBoxId>())
        {
            var controlId = GetControlId(id);
            var control = _window.FindControl<TextBox>(controlId)
                         ?? throw new InvalidOperationException($"{controlId} not found");
            _textBoxes.Add(id, control);
        }

        // CheckBox 초기화
        foreach (CheckBoxId id in Enum.GetValues<CheckBoxId>())
        {
            var controlId = GetControlId(id);
            var checkbox = _window.FindControl<CheckBox>(controlId)
                          ?? throw new InvalidOperationException($"{controlId} not found");
            _checkBoxes.Add(id, checkbox);
        }
    }

    private static string GetControlId(TextBoxId id) => $"{id}TextBox";
    private static string GetControlId(CheckBoxId id) => id.ToString();

    public void ApplySettings(Settings settings)
    {
        _textBoxes[TextBoxId.TablePath].Text = settings.TablePath ?? string.Empty;
        _textBoxes[TextBoxId.ScriptPath].Text = settings.ScriptPath ?? string.Empty;
        _textBoxes[TextBoxId.TableOutputPath].Text = settings.TableOutputPath ?? string.Empty;
        _textBoxes[TextBoxId.StringOutputPath].Text = settings.StringOutputPath ?? string.Empty;
        _textBoxes[TextBoxId.StringTableFileName].Text = settings.StringTableFileName ?? string.Empty;
        _textBoxes[TextBoxId.EnumDefinitionFileName].Text = settings.EnumDefinitionFileName ?? string.Empty;
        _textBoxes[TextBoxId.EncryptionKey].Text = settings.EncryptionKey ?? string.Empty;
        _checkBoxes[CheckBoxId.EnableEnumDefinitionFileName].IsChecked = settings.EnableEnumDefinition;
        _checkBoxes[CheckBoxId.EnableEncryptionKey].IsChecked = settings.EnableEncryption;
    }

    public Settings GetCurrentSettings()
    {
        return new Settings
        {
            TablePath = _textBoxes[TextBoxId.TablePath].Text,
            ScriptPath = _textBoxes[TextBoxId.ScriptPath].Text,
            TableOutputPath = _textBoxes[TextBoxId.TableOutputPath].Text,
            StringOutputPath = _textBoxes[TextBoxId.StringOutputPath].Text,
            StringTableFileName = _textBoxes[TextBoxId.StringTableFileName].Text,
            EnableEnumDefinition = _checkBoxes[CheckBoxId.EnableEnumDefinitionFileName].IsChecked ?? false,
            EnumDefinitionFileName = _textBoxes[TextBoxId.EnumDefinitionFileName].Text,
            EnableEncryption = _checkBoxes[CheckBoxId.EnableEncryptionKey].IsChecked ?? false,
            EncryptionKey = _textBoxes[TextBoxId.EncryptionKey].Text
        };
    }

    public string GetTextBoxValue(TextBoxId id)
    {
        if (_textBoxes.TryGetValue(id, out var textBox))
        {
            return textBox.Text ?? string.Empty;
        }
        throw new KeyNotFoundException($"TextBox with id '{id}' not found.");
    }

    public bool? GetCheckBoxValue(CheckBoxId id)
    {
        if (_checkBoxes.TryGetValue(id, out var checkBox))
        {
            return checkBox.IsChecked;
        }
        throw new KeyNotFoundException($"CheckBox with id '{id}' not found.");
    }

    public IReadOnlyDictionary<TextBoxId, TextBox> TextBoxes => _textBoxes;
    public IReadOnlyDictionary<CheckBoxId, CheckBox> CheckBoxes => _checkBoxes;
}