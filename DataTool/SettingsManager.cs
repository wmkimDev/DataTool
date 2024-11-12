using System;
using System.IO;
using System.Text.Json;
using DataTool;

public class Settings
    {
        public string? TablePath              { get; set; }
        public string? ScriptPath             { get; set; }
        public string? TableOutputPath        { get; set; }
        public string? StringOutputPath       { get; set; }
        public string? StringTableFileName    { get; set; }
        public bool    EnableEnumDefinition   { get; set; }
        public string? EnumDefinitionFileName { get; set; }
        public bool    EnableEncryption       { get; set; }
        public string? EncryptionKey          { get; set; }
    }

    public class SettingsManager
    {
        private const    string                     SettingsFileName = "settings.json";
        private readonly Action<string, OutputType> _logger;

        public SettingsManager(Action<string, OutputType> logger)
        {
            _logger = logger;
        }

        public Settings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFileName))
                {
                    var json     = File.ReadAllText(SettingsFileName);
                    var settings = JsonSerializer.Deserialize<Settings>(json);
                    _logger("설정을 성공적으로 불러왔습니다.", OutputType.Success);
                    return settings ?? new Settings();
                }
            }
            catch (Exception ex)
            {
                _logger($"설정을 불러오는 중 오류가 발생했습니다: {ex.Message}", OutputType.Error);
            }

            return new Settings();
        }

        public void SaveSettings(Settings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFileName, json);
                _logger("설정이 성공적으로 저장되었습니다.", OutputType.Success);
            }
            catch (Exception ex)
            {
                _logger($"설정을 저장하는 중 오류가 발생했습니다: {ex.Message}", OutputType.Error);
            }
        }
    }