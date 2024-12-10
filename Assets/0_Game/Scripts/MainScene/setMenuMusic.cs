using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setMenuMusic : MonoBehaviour
{
	public Slider musicVolumeSlider;
	public Slider sfxVolumeSlider;

	public AudioSource menuAudio;
	private void Start()
	{
		musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
		sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

		SetMenuVolume(musicVolumeSlider.value);
		SetSFXVolume(sfxVolumeSlider.value);

		musicVolumeSlider.onValueChanged.AddListener(SetMenuVolume);
		sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
	}
	public void SetMenuVolume(float volume)
	{
		menuAudio.volume = volume;	
		PlayerPrefs.SetFloat("MusicVolume", volume);
	}
	public void SetSFXVolume(float volume)
	{
		PlayerPrefs.SetFloat("SFXVolume", volume);
	}
}
