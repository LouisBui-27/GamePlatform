using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;
    Damagable damagable;

	private void Awake()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player == null)
		{
			Debug.Log("No player");
		}
		damagable = player.GetComponent<Damagable>();
	}
	// Start is called before the first frame update
	void Start()
    {
        healthSlider.value = CalculateSliderPercentage(damagable.Health,damagable.MaxHealth);
        healthBarText.text = "HP " + damagable.Health + " / " + damagable.MaxHealth;

    }

	private void OnEnable()
	{
		damagable.healthChange.AddListener(OnPlayerHealthChange);
	}

	private void OnDisable()
	{
		damagable.healthChange.RemoveListener(OnPlayerHealthChange);
	}
	private float CalculateSliderPercentage(float currentHealth, float maxHealth)
    {
        return currentHealth/maxHealth;
    }

	private void OnPlayerHealthChange(float newHealth, float maxHealth)
	{
		healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
		healthBarText.text = "HP " + newHealth + " / " + maxHealth;
	} 
}

