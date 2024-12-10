using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingManage : MonoBehaviour
{
	public Slider musicVolumeSlider;
	public Slider sfxVolumeSlider;

	public AudioSource introSource;
	public AudioSource loopSource;

	private void Start()
	{
		// Gán giá trị âm lượng đã lưu (nếu có) hoặc đặt giá trị mặc định
		musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
		sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

		// Áp dụng âm lượng
		SetMusicVolume(musicVolumeSlider.value);
		SetSFXVolume(sfxVolumeSlider.value);

		// Đăng ký sự kiện cho thanh trượt
		musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
		sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
	}

	public void SetMusicVolume(float volume)
	{
		introSource.volume = volume;
		loopSource.volume = volume;

		// Lưu lại âm lượng
		PlayerPrefs.SetFloat("MusicVolume", volume);
	}
	public void SetSFXVolume(float volume)
	{
		// Lưu âm lượng để dùng trong các hiệu ứng âm thanh của game
		PlayerPrefs.SetFloat("SFXVolume", volume);

		// Điều chỉnh âm lượng của PlayOneShotBehaviour nếu cần
		// Bạn có thể truy cập volume này trong các hiệu ứng âm thanh khác
	}
}
