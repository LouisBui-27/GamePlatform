using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaManage : MonoBehaviour
{
	public float maxMana = 100f;
	public float currentMana;
	public float manaRegenRate = 0.5f; // Hồi mana mỗi giây

	private void Awake()
	{
		currentMana = maxMana;
	}
	private void Update()
	{
		if(currentMana < maxMana)
		{
			currentMana += manaRegenRate * Time.deltaTime;
			currentMana = Mathf.Min(currentMana, maxMana);
			CharacterEvents.manaChanged?.Invoke(gameObject, currentMana, maxMana);
		}
	}

	public bool UseMana(float amount)
	{
		if(currentMana >= amount)
		{
			currentMana -= amount;
			currentMana = Mathf.Max(currentMana, 0);

			CharacterEvents.manaUsed?.Invoke(gameObject, amount);
		//	CharacterEvents.manaChanged?.Invoke(gameObject, currentMana, maxMana);
			return true;
		}
		return false;
	}
	public void IncreaseMana(float amount)
	{
		maxMana += amount;
		currentMana = maxMana;
		CharacterEvents.manaChanged?.Invoke(gameObject, currentMana, maxMana);
	}
	public void changeManaRegenRate(float amount)
	{
		manaRegenRate += amount;
	}

}
