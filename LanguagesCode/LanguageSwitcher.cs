using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LanguageSwitcher : MonoBehaviour
{
    public int index;
    public void OnLanguageChanged()
    {
        if(index < 1)
        {
            index += 1;
        }
        else
        {
            index = 0;
        }
        string language = index == 0 ? "Russian" : "English";
        LocalizationManager.Instance.LoadLanguage(language);
    }
}