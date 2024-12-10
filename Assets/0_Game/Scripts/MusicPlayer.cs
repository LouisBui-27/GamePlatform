using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	public AudioSource introSource, loopSound;

	private void Start()
	{
		introSource.Play();
		loopSound.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
	}
}
