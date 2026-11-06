using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    
    private Dictionary<string, string> localizedText;
    private string currentLanguage = "Russian";
    
    public event System.Action OnLanguageChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLanguage(currentLanguage);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void LoadLanguage(string language)
    {
        currentLanguage = language;
        
        TextAsset textAsset = Resources.Load<TextAsset>($"Languages/{language}");
        
        if (textAsset == null)
        {
            Debug.LogError($"Файл языка {language} не найден!");
            return;
        }
        
        // Используем свой парсер JSON вместо JsonUtility
        localizedText = ParseJson(textAsset.text);
        
        // Сохраняем выбор игрока
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();
        
        // Оповещаем все UI-элементы
        OnLanguageChanged?.Invoke();
        
        Debug.Log($"Язык загружен: {language}. Загружено ключей: {localizedText.Count}");
    }
    
    // Простой парсер JSON для формата "ключ": "значение"
    private Dictionary<string, string> ParseJson(string json)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        
        // Убираем фигурные скобки
        json = json.Trim();
        if (json.StartsWith("{")) json = json.Substring(1);
        if (json.EndsWith("}")) json = json.Substring(0, json.Length - 1);
        
        // Разбиваем на строки
        string[] lines = json.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            
            // Пропускаем пустые строки и запятые
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine == ",") continue;
            
            // Убираем запятую в конце
            if (trimmedLine.EndsWith(",")) trimmedLine = trimmedLine.Substring(0, trimmedLine.Length - 1);
            
            // Ищем двоеточие (разделитель ключа и значения)
            int colonIndex = trimmedLine.IndexOf(':');
            if (colonIndex == -1) continue;
            
            string key = trimmedLine.Substring(0, colonIndex).Trim();
            string value = trimmedLine.Substring(colonIndex + 1).Trim();
            
            // Убираем кавычки у ключа
            if (key.StartsWith("\"") && key.EndsWith("\""))
            {
                key = key.Substring(1, key.Length - 2);
            }
            
            // Убираем кавычки у значения
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
            }
            
            // Декодируем escape-последовательности
            value = value.Replace("\\n", "\n")
                         .Replace("\\\"", "\"")
                         .Replace("\\\\", "\\");
            
            result[key] = value;
        }
        
        return result;
    }
    
    public string GetText(string key)
    {
        if (localizedText == null || !localizedText.ContainsKey(key))
        {
            Debug.LogWarning($"Ключ '{key}' не найден в языке {currentLanguage}");
            return key;
        }
        return localizedText[key];
    }
    
    // С параметрами: GetText("score", 100) -> "Счёт: 100"
    public string GetText(string key, params object[] args)
    {
        string text = GetText(key);
        return string.Format(text, args);
    }
    
    public string CurrentLanguage => currentLanguage;
}