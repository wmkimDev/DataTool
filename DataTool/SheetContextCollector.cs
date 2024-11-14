using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DataTool;

public class SheetContextCollector
{
    private readonly        UIControlManager                    _controlManager;
    private readonly        OutputManager                       _outputManager;
    private readonly        IReadOnlyDictionary<string, string> _typeMap;
    private readonly        List<SheetContext>                  _sheetContexts      = new();
    private static readonly string[]                            SupportedExtensions = { ".xlsx", ".xls" };
    private readonly TableAddress _address;
    
    public SheetContextCollector(UIControlManager controlManager, OutputManager outputManager)
    {
        _controlManager = controlManager;
        _outputManager  = outputManager;
        _typeMap        = InitializeTypeMap();
        _address = new TableAddress();
    }

    private static IReadOnlyDictionary<string, string> InitializeTypeMap() =>
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "int", "int" },
            { "bool", "bool" },
            { "long", "long" },
            { "float", "float" },
            { "double", "double" },
            { "string", "string" },
            { "datetime", "datetime" },
            { "timespan", "timespan" }
        };

    public IReadOnlyList<SheetContext> SheetContexts => _sheetContexts;

    public void CollectAll()
    {
        var excelFiles = GetExcelFileList();
        ProcessExcelFiles(excelFiles);
    }

    private List<string> GetExcelFileList()
    {
        var tablePath = _controlManager.GetTextBoxValue(TextBoxId.TablePath);
        var excelFiles = Directory.GetFiles(tablePath, "*.*", SearchOption.AllDirectories)
            .Where(IsExcelFile)
            .OrderBy(file => file)
            .ToList();

        if (excelFiles.Count != 0) return excelFiles;
        
        var message = $"지정된 디렉토리에서 엑셀 파일을 찾을 수 없습니다: {tablePath}";
        throw new FileNotFoundException(message);
    }

    private static bool IsExcelFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return SupportedExtensions.Contains(extension);
    }

    private void ProcessExcelFiles(IEnumerable<string> files)
    {
        _sheetContexts.Clear();
        foreach (var file in files)
        {
            if (IsSpecialSheet(Path.GetFileNameWithoutExtension(file)))
                continue;

            _address.TableName = Path.GetFileNameWithoutExtension(file);
            var contexts = ProcessExcelFile(file);
            _sheetContexts.AddRange(contexts);
        }
    }

    private bool IsSpecialSheet(string sheetName)
    {
        return sheetName == _controlManager.GetTextBoxValue(TextBoxId.EnumDefinitionFileName)
               || sheetName == _controlManager.GetTextBoxValue(TextBoxId.StringTableFileName);
    }

    private List<SheetContext> ProcessExcelFile(string filePath)
    {
        var sheetNames = new HashSet<string>();
        
        
        using var fs       = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var       workbook = new XSSFWorkbook(fs);
        var       contexts = new List<SheetContext>();

        for (var i = 0; i < workbook.NumberOfSheets; i++)
        {
            var sheet        = workbook.GetSheetAt(i);
            _address.SheetName = sheet.SheetName;
            
            var sheetContext = GetSheetContext(sheet);
            if (!sheetNames.Add(sheetContext.SheetName))
                _address.ThrowException("중복된 시트 이름이 발견되었습니다");
            
            contexts.Add(sheetContext);
        }

        return contexts;
    }

    private SheetContext GetSheetContext(ISheet sheet)
    {
        var sheetName = GetSheetName(sheet);
        var (rowCount, columnCount) = GetTableDimensions(sheet);
        _outputManager.AppendMessage($"{sheetName} 테이블 크기: {rowCount}행 x {columnCount}열", OutputType.Info);
        var propertyNames        = ExtractPropertyNames(sheet, columnCount);
        var propertyTypes        = ExtractPropertyTypes(sheet, columnCount);
        var propertyDescriptions = ExtractPropertyDescriptions(sheet, columnCount);

        PrintPropertyInfo(propertyNames, propertyTypes);

        var primaryKeyIndex = FindPrimaryKeyIndex(sheet, columnCount);
        var foreignKeys     = FindForeignKeys(sheet, propertyNames);
        PrintKeyInfo(primaryKeyIndex, foreignKeys, propertyNames, propertyTypes);

        return new SheetContext(
            _address.TableName!,
            sheetName,
            sheet,
            rowCount,
            columnCount,
            propertyTypes,
            propertyNames,
            propertyDescriptions,
            primaryKeyIndex,
            foreignKeys
        );
    }

    private string GetSheetName(ISheet sheet)
    {
        var nameCell = sheet.GetRow(0)?.GetCell(1);
        if (nameCell?.CellType != CellType.String)
            _address.ThrowException("시트 이름 셀이 비어있거나 문자열이 아닙니다");
        return nameCell!.StringCellValue;
    }

    private (int rowCount, int columnCount) GetTableDimensions(ISheet sheet)
    {
        var firstRow = sheet.GetRow(0);
        if (firstRow == null)
            _address.ThrowException("첫 번째 행을 찾을 수 없습니다.");

        var (firstMarker, lastMarker) = (-1, -1);
        for (int j = 0; j < firstRow.LastCellNum; j++)
        {
            if (firstRow.GetCell(j)?.StringCellValue?.Contains("@") != true) continue;
            if (firstMarker == -1) firstMarker = j;
            lastMarker = j;
        }

        if (firstMarker == -1)
            _address.ThrowException("테이블 범위 마커(@)를 찾을 수 없습니다.");

        var rowCount = 4;
        while (sheet.GetRow(rowCount)?.GetCell(firstMarker)?.StringCellValue?.Contains("@") == true)
            rowCount++;

        return (rowCount - 4 - 1, lastMarker - firstMarker - 1);
    }

    private List<string> ExtractPropertyNames(ISheet sheet, int columnCount)
    {
        var propertyRow   = sheet.GetRow(2);
        var propertyNames = new List<string>();

        for (int i = 1; i <= columnCount; i++)
        {
            var cell = propertyRow.GetCell(i);
            if (cell?.CellType != CellType.String)
                _address.ThrowException("속성 이름은 문자열이어야 합니다. 열: " + i);

            var propertyName = cell.StringCellValue;
            if (propertyName.Contains(" "))
                _address.ThrowException("속성 이름에 공백이 포함되어 있습니다: " + propertyName);
            
            if (string.IsNullOrWhiteSpace(propertyName))
                _address.ThrowException("속성 이름이 비어있습니다.");
            
            if (_address.SheetName == propertyName)
                _address.ThrowException("속성 이름이 시트 이름과 동일합니다: " + propertyName);

            propertyNames.Add(propertyName);
        }

        return propertyNames;
    }

    private List<string> ExtractPropertyTypes(ISheet sheet, int columnCount)
    {
        var typeRow       = sheet.GetRow(3);
        var propertyTypes = new List<string>();

        for (int i = 1; i <= columnCount; i++)
        {
            var cell = typeRow.GetCell(i);
            if (cell?.CellType != CellType.String)
            {
                var message = $"속성 타입은 문자열이어야 합니다. 열: {i}";
                _outputManager.AppendMessage(message, OutputType.Error);
                throw new Exception(message);
            }

            var typeString = cell.StringCellValue;
            if (_typeMap.TryGetValue(typeString.ToLower(), out var mappedType))
            {
                propertyTypes.Add(mappedType);
            }
            else
            {
                var message = $"지원되지 않는 타입입니다: {typeString}";
                _outputManager.AppendMessage(message, OutputType.Error);
                throw new Exception(message);
            }
        }

        return propertyTypes;
    }

    private List<string> ExtractPropertyDescriptions(ISheet sheet, int columnCount)
    {
        var descriptionRow = sheet.GetRow(1);
        var descriptions   = new List<string>();

        for (int i = 1; i <= columnCount; i++)
        {
            var cell = descriptionRow.GetCell(i);
            descriptions.Add(cell?.StringCellValue ?? string.Empty);
        }

        return descriptions;
    }

    private int FindPrimaryKeyIndex(ISheet sheet, int columnCount)
    {
        var headerRow = sheet.GetRow(2);
        for (int i = 1; i <= columnCount; i++)
        {
            var cell = headerRow.GetCell(i);
            if (cell?.StringCellValue.ToLower() == "pkey")
                return i - 1;
        }

        return -1;
    }

    private List<ForeignKeyInfo> FindForeignKeys(ISheet sheet, List<string> propertyNames)
    {
        var foreignKeys     = new List<ForeignKeyInfo>();
        var propertyNameSet = new HashSet<string>();
        var regex           = new System.Text.RegularExpressions.Regex(@"^(\w+)\[(\w+)\]$");

        for (int i = 0; i < propertyNames.Count; i++)
        {
            var match = regex.Match(propertyNames[i]);
            if (match.Success)
            {
                var propertyName    = match.Groups[1].Value;
                var referencedTable = match.Groups[2].Value;

                if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(referencedTable))
                {
                    var message = $"외래 키 형식이 잘못되었습니다. 열: {i + 1}, 값: {propertyNames[i]}";
                    _outputManager.AppendMessage(message, OutputType.Error);
                    throw new Exception(message);
                }

                foreignKeys.Add(new ForeignKeyInfo(i, referencedTable));
                propertyNames[i] = propertyName;
            }

            if (!propertyNameSet.Add(propertyNames[i]))
            {
                var message = $"중복된 속성 이름이 발견되었습니다: {propertyNames[i]}";
                _outputManager.AppendMessage(message, OutputType.Error);
                throw new Exception(message);
            }
        }

        return foreignKeys;
    }

    private void PrintPropertyInfo(List<string> propertyNames, List<string> propertyTypes)
    {
        _outputManager.AppendMessage("\n[속성 정보]", OutputType.Info);
        var maxNameLength = propertyNames.Max(n => n.Length);
        var maxTypeLength = propertyTypes.Max(t => t.Length);

        for (int i = 0; i < propertyNames.Count; i++)
        {
            _outputManager.AppendMessage(
                $"  * {propertyNames[i].PadRight(maxNameLength)} : {propertyTypes[i].PadRight(maxTypeLength)}",
                OutputType.Info);
        }
    }

    private void PrintKeyInfo(int primaryKeyIndex, List<ForeignKeyInfo> foreignKeys, List<string> propertyNames,
        List<string> propertyTypes)
    {
        _outputManager.AppendMessage("\n[테이블 키 정보]", OutputType.Info);

        if (primaryKeyIndex != -1)
        {
            _outputManager.AppendMessage(
                $"  PKey: {propertyNames[primaryKeyIndex]} ({propertyTypes[primaryKeyIndex]})",
                OutputType.Info);
        }
        else
        {
            _outputManager.AppendMessage(
                "  PKey가 설정되지 않았습니다. 기본 테이블 순서(행 인덱스)를 키로 사용합니다.",
                OutputType.Warning);
        }

        if (foreignKeys.Any())
        {
            _outputManager.AppendMessage("\n[외래 키 정보]", OutputType.Info);
            foreach (var fk in foreignKeys)
            {
                _outputManager.AppendMessage(
                    $"  FKey: {propertyNames[fk.ColumnIndex]} (참조 테이블: {fk.ReferencedTableName})",
                    OutputType.Info);
            }
        }
        else
        {
            _outputManager.AppendMessage("  외래 키가 없습니다.", OutputType.Info);
        }
    }
}