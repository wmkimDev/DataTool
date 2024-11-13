using System;
using System.Collections.Generic;
using Avalonia.Controls;

namespace DataTool;

public class ValidationBuilder
{
    private readonly UIControlManager _controls;
    private readonly List<ValidationRule> _validations = new();

    private record ValidationRule(string Field, Func<bool> Validator, string ErrorMessage);

    public ValidationBuilder(UIControlManager controls)
    {
        _controls = controls;
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

    private ValidationBuilder AddValidation(string field, Func<bool> validator, string errorMessage)
    {
        _validations.Add(new ValidationRule(field, validator, errorMessage));
        return this;
    }

    private bool ValidateTextBox(TextBoxId id) => 
        !string.IsNullOrWhiteSpace(_controls.GetTextBoxValue(id));

    private bool ValidateConditionalTextBox(CheckBoxId checkBoxId, TextBoxId textBoxId) =>
        !_controls.GetCheckBoxValue(checkBoxId)!.Value || 
        !string.IsNullOrWhiteSpace(_controls.GetTextBoxValue(textBoxId));

    public ValidationBuilder ValidateTable() =>
        AddValidation(
            "테이블 경로",
            () => ValidateTextBox(TextBoxId.TablePath),
            "테이블 경로를 입력하세요.");

    public ValidationBuilder ValidateScript() =>
        AddValidation(
            "스크립트 경로",
            () => ValidateTextBox(TextBoxId.ScriptPath),
            "스크립트 경로를 입력하세요.");

    public ValidationBuilder ValidateTableOutput() =>
        AddValidation(
            "테이블 출력 경로",
            () => ValidateTextBox(TextBoxId.TableOutputPath),
            "테이블 출력 경로를 입력하세요.");

    public ValidationBuilder ValidateStringOutput() =>
        AddValidation(
            "문자열 출력 경로",
            () => ValidateTextBox(TextBoxId.StringOutputPath),
            "문자열 출력 경로를 입력하세요.");

    public ValidationBuilder ValidateStringTableFileName() =>
        AddValidation(
            "문자열 테이블 파일명",
            () => ValidateTextBox(TextBoxId.StringTableFileName),
            "문자열 테이블 파일명을 입력하세요.");

    public ValidationBuilder ValidateEncryption() =>
        AddValidation(
            "암호화 키",
            () => ValidateConditionalTextBox(
                CheckBoxId.EnableEncryptionKey,
                TextBoxId.EncryptionKey),
            "암호화 키를 입력하세요.");

    public ValidationBuilder ValidateEnumDefinition() =>
        AddValidation(
            "열거형 정의 파일명",
            () => ValidateConditionalTextBox(
                CheckBoxId.EnableEnumDefinitionFileName,
                TextBoxId.EnumDefinitionFileName),
            "열거형 정의 파일명을 입력하세요.");

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