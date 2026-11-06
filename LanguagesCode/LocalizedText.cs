using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LocalizedText : MonoBehaviour
{
    public string key;
    public string suffixKey;

    [Header("Typewriter Settings")]
    [Tooltip("Задержка между символами (в секундах)")]
    public float charDelay = 0.05f;
    [Tooltip("Включить эффект печатной машинки")]
    public bool useTypewriter = true;
    [Tooltip("Запускать печать при старте")]
    public bool playOnStart = true;

    [Header("Voice")]
    [Tooltip("Компонент бормотания. Если null — ищется на этом же объекте.")]
    public RadioVoice radioVoice;
    [Tooltip("Играть слог не на каждый символ, а на каждые N символов")]
    [Range(1, 4)] public int charsPerSyllable = 2;
    [Tooltip("Включить озвучку печати")]
    public bool useVoice = true;

    private Text uiText;
    private TextMeshProUGUI tmpText;

    private Coroutine typingCoroutine;
    private string fullText = "";
    private int typingVersion = 0;

    private int charCounter = 0;

    void Start()
    {
        uiText = GetComponent<Text>();
        tmpText = GetComponent<TextMeshProUGUI>();

        if (radioVoice == null)
            radioVoice = GetComponent<RadioVoice>();

        if (playOnStart)
            UpdateText();
        else
            SetFullTextImmediate();

        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
    }

    public void SetKey(string newKey, string newSuffixKey = "")
    {
        newSuffixKey = newSuffixKey ?? "";
        string currentSuffix = suffixKey ?? "";

        if (key == newKey && currentSuffix == newSuffixKey)
            return;

        key = newKey;
        suffixKey = newSuffixKey;
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance == null) return;

        string translated = LocalizationManager.Instance.GetText(key);
        if (!string.IsNullOrEmpty(suffixKey))
            translated += LocalizationManager.Instance.GetText(suffixKey);

        fullText = translated;

        // [+ ACTIVE CHECK] Если объект неактивен — просто сохраняем текст,
        // запуск печати произойдёт при активации (см. OnEnable)
        if (!isActiveAndEnabled)
        {
            SetTextImmediate(fullText);
            return;
        }

        if (useTypewriter)
            StartTyping();
        else
            SetTextImmediate(fullText);
    }

    private void StartTyping()
    {
        // [+ ACTIVE CHECK]
        if (!isActiveAndEnabled)
        {
            SetTextImmediate(fullText);
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingVersion++;
        typingCoroutine = StartCoroutine(TypeText(fullText, typingVersion));
    }

    private IEnumerator TypeText(string text, int version)
    {
        SetTextImmediate("");
        StopVoice();
        charCounter = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (version != typingVersion)
            {
                StopVoice();
                yield break;
            }

            // [+ ACTIVE CHECK] Объект мог быть отключён во время печати
            if (!isActiveAndEnabled)
            {
                StopVoice();
                typingCoroutine = null;
                SetTextImmediate(fullText); // сразу показываем полный текст
                yield break;
            }

            SetTextImmediate(text.Substring(0, i + 1));

            char c = text[i];

            if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
            {
                yield return new WaitForSeconds(charDelay * 0.5f);
                continue;
            }

            if (useVoice && charCounter % charsPerSyllable == 0)
                PlaySyllable();
            charCounter++;

            yield return new WaitForSeconds(charDelay);
        }

        typingCoroutine = null;
    }

    private void PlaySyllable()
    {
        // [+ ACTIVE CHECK]
        if (!isActiveAndEnabled) return;
        if (radioVoice != null)
            radioVoice.PlaySyllable();
    }

    private void StopVoice()
    {
        // [+ ACTIVE CHECK] при выключении объект может быть уже неактивен
        if (radioVoice != null)
            radioVoice.Stop();
    }

    private void SetTextImmediate(string text)
    {
        if (uiText != null) uiText.text = text;
        if (tmpText != null) tmpText.text = text;
    }

    private void SetFullTextImmediate()
    {
        if (LocalizationManager.Instance == null) return;

        string translated = LocalizationManager.Instance.GetText(key);
        if (!string.IsNullOrEmpty(suffixKey))
            translated += LocalizationManager.Instance.GetText(suffixKey);

        fullText = translated;
        SetTextImmediate(fullText);
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        typingVersion++;
        StopVoice();
        SetTextImmediate(fullText);
    }

    // [+ ACTIVE CHECK] При включении объекта — если текст ещё не был напечатан,
    // запускаем печать
    void OnEnable()
    {
        if (playOnStart && useTypewriter && !string.IsNullOrEmpty(fullText))
            StartTyping();
    }

    void OnDisable()
    {
        // [+ ACTIVE CHECK] Останавливаем печать и звук при выключении
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        typingVersion++;
        StopVoice();
    }

    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        StopVoice();
    }
}