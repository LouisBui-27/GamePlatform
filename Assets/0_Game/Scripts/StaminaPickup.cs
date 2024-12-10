using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPickup : MonoBehaviour
{
	public float staminaRestore = 5f;
	public float moveSpeed = 1f;
	public float moveAmplitude = .5f;

	AudioSource pickupSound;
	private Vector3 startPos;
	private float currentVolume => PlayerPrefs.GetFloat("SFXVolume", 1f);

	private void Awake()
	{
		startPos = transform.position;
		pickupSound = GetComponent<AudioSource>();
	}

	private void Update()
	{
		// Tính toán vị trí di chuyển lên xuống dựa vào sin
		float yOffset = Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
		transform.position = startPos + new Vector3(0, yOffset, 0);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		StaminaManage staminaManage = collision.GetComponent<StaminaManage>();
		if (staminaManage != null)
		{
			bool wasHeal = staminaManage.HealStamina(staminaRestore);
			if (wasHeal)
			{
				AudioSource.PlayClipAtPoint(pickupSound.clip, gameObject.transform.position, currentVolume);
				Destroy(gameObject);
			}
		}
	}
}
