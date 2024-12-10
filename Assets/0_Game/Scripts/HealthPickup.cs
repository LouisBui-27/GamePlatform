using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class HealthPickup : MonoBehaviour
{
    public float healthRestore = 20;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);
    AudioSource pickupSound;
	private float currentVolume => PlayerPrefs.GetFloat("SFXVolume", 1f);

	private void Awake()
	{
		pickupSound = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Damagable damagable = collision.GetComponent<Damagable>();
        if (damagable)
        {
           bool wasHealed =  damagable.Heal(healthRestore);
            if (wasHealed)
            {
			AudioSource.PlayClipAtPoint(pickupSound.clip, gameObject.transform.position, currentVolume);
            Destroy(gameObject);

            } 
        }
    }

	private void Update()
	{
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime * (float)0.3;
	}
}
