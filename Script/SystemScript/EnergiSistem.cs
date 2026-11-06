using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnergiSistem : MonoBehaviour
{
    public GameObject Vilka;
    public float MaxEnergi;
    public float RealEnergi;
    public List<ActivCode> BoxEhergi = new List<ActivCode>();
    public GameObject MaikLight;
    public List<GameObject> Light = new List<GameObject>();
    public Slider slider;
    public AudioSource AS;
    public AudioSource AS2Off;
    public ActivCode AC;

    // Таймер отключения при нулевой энергии
    private float _blackoutTimer = 0f;

    public void Update()
    {
        // 1. Отображение энергии
        slider.value = RealEnergi;

        // 2. Клэмп, чтобы не уходило в минус
        if (RealEnergi < 0f) RealEnergi = 0f;
        if (RealEnergi > MaxEnergi) RealEnergi = MaxEnergi;

        // 3. Проверка, что список выключателей не пуст
        bool hasBoxes = BoxEhergi != null && BoxEhergi.Count >= 2;

        // 4. Генератор выключен — гасим ВСЁ
        if (AC == null || AC.Activ == false)
        {
            if (AS.isPlaying)
            {
                AS.Stop();
                AS2Off.Play();
            } 

            if (MaikLight.activeSelf) MaikLight.SetActive(false);

            for (int i = 0; i < Light.Count; i++)
            {
                if (Light[i].activeSelf) Light[i].SetActive(false);
            }

            _blackoutTimer = 0f;
            return; // дальше логику не выполняем
        }

        // 5. Генератор включён, но энергии нет — тоже гасим всё и считаем таймер
        if (RealEnergi <= 0f)
        {
            if (AS.isPlaying)
            {
                AS.Stop();
                AS2Off.Play();
            } 

            if (MaikLight.activeSelf) MaikLight.SetActive(false);

            for (int i = 0; i < Light.Count; i++)
            {
                if (Light[i].activeSelf) Light[i].SetActive(false);
            }

            _blackoutTimer += Time.deltaTime;
            if (_blackoutTimer > 5f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        // 6. Энергия есть и генератор включён — сбрасываем таймер
        _blackoutTimer = 0f;

        if (AS.isPlaying == false) AS.Play();

        if (!hasBoxes) return;

        // 7. Маяк: два выключателя
        if (Vilka.GetComponent<ActivCode>().Activ == true && BoxEhergi[1].Activ == true)
        {
            if (MaikLight.activeSelf == false)
                MaikLight.SetActive(true);

            RealEnergi -= 0.005f * Time.deltaTime; // расход во времени
        }
        else
        {
            if (MaikLight.activeSelf == true)
                MaikLight.SetActive(false);
        }

        // 8. Малые лампы
        if (BoxEhergi[0].Activ == true)
        {
            for (int i = 0; i < Light.Count; i++)
            {
                if (Light[i].activeSelf == false)
                    Light[i].SetActive(true);
            }
            RealEnergi -= 0.002f * Time.deltaTime; // расход во времени
        }
        else
        {
            for (int i = 0; i < Light.Count; i++)
            {
                if (Light[i].activeSelf == true)
                    Light[i].SetActive(false);
            }
        }

        // 9. Финальный клэмп на случай, если ушли в минус в этом кадре
        if (RealEnergi < 0f) RealEnergi = 0f;
    }
}