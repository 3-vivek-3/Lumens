using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager instance;
    private void Awake()
    {
        if (instance)
        {
            Debug.LogError("Audio manager already exists.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private const string VOLUME_KEY = "MasterVolume";

    [SerializeField] private AudioSource bgmSource;

    [SerializeField] private AudioClip backgroundLoop;
    [SerializeField] private float fadeDuration;

    private void OnEnable()
    {
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        SetGlobalVolume(savedVolume);

        bgmSource.clip = backgroundLoop;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();
    }

    public void SetGlobalVolume(float value)
    {
        value = Mathf.Clamp01(value);

        AudioListener.volume = value;

        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public float GetGlobalVolume()
    {
        return AudioListener.volume;
    }

    public void FadeOutMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeVolume(0f));
    }

    public void FadeInMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeVolume(1f));
    }

    public IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = bgmSource.volume;
        float startTime = Time.time;

        while(Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            bgmSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }
}
