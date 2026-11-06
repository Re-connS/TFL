using UnityEngine;

/// <summary>
/// Генерирует «еле разборчивое» бормотание по рации из набора коротких сэмплов.
/// Вешается на тот же GameObject, что и LocalizedText (или на родителя).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RadioVoice : MonoBehaviour
{
    [Header("Clips")]
    [Tooltip("Короткие сэмплы-слоги (гласные/согласные). 0.08–0.15 сек каждый.")]
    public AudioClip[] syllables;

    [Header("Pitch")]
    [Tooltip("Базовая высота тона. Для рации 1.3–1.8")]
    public float basePitch = 1.5f;
    [Tooltip("Случайный разброс высоты тона (±)")]
    [Range(0f, 0.5f)] public float pitchJitter = 0.15f;

    [Header("Volume")]
    [Range(0f, 1f)] public float volume = 0.8f;

    public AudioSource source;

    void Awake()
    {
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f; // 2D — голос из рации
    }

    /// <summary>Проиграть один случайный слог.</summary>
    public void PlaySyllable()
    {
        if (syllables == null || syllables.Length == 0) return;

        var clip = syllables[Random.Range(0, syllables.Length)];
        if (clip == null) return;

        source.pitch = basePitch + Random.Range(-pitchJitter, pitchJitter);
        source.PlayOneShot(clip, volume);
    }

    /// <summary>Остановить текущий звук (при скипе/смене реплики).</summary>
    public void Stop()
    {
        if (source != null) source.Stop();
    }
}