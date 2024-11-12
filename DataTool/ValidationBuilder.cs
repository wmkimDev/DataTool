using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace DataTool;

public class ValidationBuilder
{
    private readonly Dictionary<string, TextBox>                                     _textBoxes;
    private readonly Dictionary<string, CheckBox>                                    _checkBoxes;
    private readonly List<(string Field, Func<bool> Validator, string ErrorMessage)> _validations = new();

    public ValidationBuilder(Dictionary<string, TextBox> textBoxes, Dictionary<string, CheckBox> checkBoxes)
    {
        _textBoxes  = textBoxes;
        _checkBoxes = checkBoxes;
    }

    public ValidationBuilder ValidateAll()
    {
        return ValidateTable()
            .ValidateScript()
            .ValidateTableOutput()
            .ValidateStringOutput()
            .ValidateStringTableFileName()
            .ValidateEncryption()
            .ValidateEnumDefinition();
    }

    public ValidationBuilder ValidateTable()
    {
        _validations.Add(("테이블 경로",
            () => !string.IsNullOrWhiteSpace(_textBoxes["TablePathTextBox"].Text),
            "테이블 경로를 입력하세요."));
        return this;
    }

    public ValidationBuilder ValidateScript()
    {
        _validations.Add(("스크립트 경로",
            () => !string.IsNullOrWhiteSpace(_textBoxes["ScriptPathTextBox"].Text),
            "스크립트 경로를 입력하세요."));
        return this;
    }

    public ValidationBuilder ValidateTableOutput()
    {
        _validations.Add(("테이블 출력 경로",
            () => !string.IsNullOrWhiteSpace(_textBoxes["TableOutputPathTextBox"].Text),
            "테이블 출력 경로를 입력하세요."));
        return this;
    }

    public ValidationBuilder ValidateStringOutput()
    {
        _validations.Add(("문자열 출력 경로",
            () => !string.IsNullOrWhiteSpace(_textBoxes["StringOutputPathTextBox"].Text),
            "문자열 출력 경로를 입력하세요."));
        return this;
    }

    public ValidationBuilder ValidateStringTableFileName()
    {
        _validations.Add(("문자열 테이블 파일명",
            () => !string.IsNullOrWhiteSpace(_textBoxes["StringTableFileNameTextBox"].Text),
            "문자열 테이블 파일명을 입력하세요."));
        return this;
    }

    public ValidationBuilder ValidateEncryption()
    {
        _validations.Add(("암호화 키",
            () => !_checkBoxes["EnableEncryptionKey"].IsChecked!.Value ||
                  !string.IsNullOrWhiteSpace(_textBoxes["EncryptionKeyTextBox"].Text),
            "암호화 키를 입력하세요."));
        return this;
    }

    public ValidationBuilder ValidateEnumDefinition()
    {
        _validations.Add(("열거형 정의 파일명",
            () => !_checkBoxes["EnableEnumDefinitionFileName"].IsChecked!.Value ||
                  !string.IsNullOrWhiteSpace(_textBoxes["EnumDefinitionFileNameTextBox"].Text),
            "열거형 정의 파일명을 입력하세요."));
        return this;
    }

    public ValidationResult Build()
    {
        foreach (var validation in _validations)
        {
            if (!validation.Validator())
            {
                return ValidationResult.Error(validation.ErrorMessage);
            }
        }

        return ValidationResult.Success();
    }
}