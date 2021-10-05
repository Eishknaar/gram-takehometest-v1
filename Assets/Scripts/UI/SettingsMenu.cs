using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    private void Awake()
    {
        float volume = PlayerPrefs.GetFloat("volume");
        volumeSlider.value = volume;
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnVolumeChanged(float value)
	{
		PlayerPrefs.SetFloat("volume", value);
	}
}
