using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : Menu
{
    [SerializeField] private Slider volumeSlider;

    private void OnEnable()
    {
        volumeSlider.value = AudioManager.instance.GetGlobalVolume();
    }

    public void OnVolumeAdjusted(float value)
    {
        AudioManager.instance.SetGlobalVolume(value);
    }

    public void OnBackPressed()
    {
        MenuManager.instance.CloseMenu();
    }
}