using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace DataTool;

public partial class TypeMapWindow : Window
{
    private const string TypeMapFileName = "typemap.json";
    private readonly Grid _typeMapGrid;
    private readonly List<TextBox> _targetBoxes = new();
    private Dictionary<string, string> _typeMap;

    private static readonly string[] DefaultTypes = 
    { 
        "int", "bool", "long", "float", 
        "double", "string", "datetime", "timespan" 
    };

    public TypeMapWindow()
    {
        InitializeComponent();
        _typeMapGrid = this.FindControl<Grid>("TypeMapGrid")!;
        _typeMap = LoadTypeMap();
        InitializeGrid();
    }

    private Dictionary<string, string> LoadTypeMap()
    {
        try
        {
            if (File.Exists(TypeMapFileName))
            {
                var json = File.ReadAllText(TypeMapFileName);
                var loadedMap = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (loadedMap != null)
                {
                    // 기본 타입이 모두 있는지 확인
                    foreach (var type in DefaultTypes)
                    {
                        loadedMap.TryAdd(type, type);
                    }
                    return new Dictionary<string, string>(loadedMap, StringComparer.OrdinalIgnoreCase);
                }
            }
        }
        catch
        {
            // 로드 실패시 기본값 사용
        }

        // 기본 타입맵 생성
        var defaultMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var type in DefaultTypes)
        {
            defaultMap[type] = type;
        }
        return defaultMap;
    }

    private void InitializeGrid()
    {
        // Grid 행 정의 추가
        for (int i = 0; i < DefaultTypes.Length; i++)
        {
            _typeMapGrid.RowDefinitions.Add(new RowDefinition(35, GridUnitType.Pixel));
        }

        // 열 정의
        _typeMapGrid.ColumnDefinitions.Add(new ColumnDefinition(150, GridUnitType.Pixel));  // 소스 타입
        _typeMapGrid.ColumnDefinitions.Add(new ColumnDefinition(30, GridUnitType.Pixel));   // 화살표
        _typeMapGrid.ColumnDefinitions.Add(new ColumnDefinition(150, GridUnitType.Pixel));  // 타겟 타입

        for (int i = 0; i < DefaultTypes.Length; i++)
        {
            var sourceType = DefaultTypes[i];
            var targetType = _typeMap.GetValueOrDefault(sourceType, sourceType);

            // 소스 타입
            var sourceBlock = new TextBlock 
            { 
                Text = sourceType,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = 14
            };
            Grid.SetRow(sourceBlock, i);
            Grid.SetColumn(sourceBlock, 0);

            // 화살표
            var arrowBlock = new TextBlock 
            { 
                Text = "→",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.Parse("#FF666666")),
                FontSize = 14
            };
            Grid.SetRow(arrowBlock, i);
            Grid.SetColumn(arrowBlock, 1);

            // 타겟 타입
            var targetBox = new TextBox 
            { 
                Text = targetType,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                Background = new SolidColorBrush(Color.Parse("#FF2D2D30")),
                BorderBrush = new SolidColorBrush(Color.Parse("#FF3E3E42")),
                Foreground = Brushes.White,
                FontSize = 14,
                Height = 30,
                CornerRadius = new CornerRadius(4)
            };
            Grid.SetRow(targetBox, i);
            Grid.SetColumn(targetBox, 2);

            _typeMapGrid.Children.Add(sourceBlock);
            _typeMapGrid.Children.Add(arrowBlock);
            _typeMapGrid.Children.Add(targetBox);
            
            _targetBoxes.Add(targetBox);
        }
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        var newTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        for (int i = 0; i < DefaultTypes.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(_targetBoxes[i].Text))
            {
                newTypeMap[DefaultTypes[i]] = _targetBoxes[i].Text;
            }
        }

        _typeMap = newTypeMap;
        SaveTypeMap();
        Close();
    }

    private void SaveTypeMap()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_typeMap, options);
            File.WriteAllText(TypeMapFileName, json);
        }
        catch
        {
            // 저장 실패시 무시
        }
    }

    public Dictionary<string, string> GetTypeMap()
    {
        return new Dictionary<string, string>(_typeMap, StringComparer.OrdinalIgnoreCase);
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}