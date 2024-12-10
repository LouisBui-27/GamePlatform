using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManage : MonoBehaviour
{
	public float maxStamina = 100f;
	public float currentStamina;
	public float staminaRegenRate = 5f; // Tốc độ hồi stamina mỗi giây
	public float staminaCostPerSecondRunning = 1f;

	private bool isRunning;
	private bool isMoving;
	private bool isAttacking;
	private bool isJumping;

	private void Awake()
	{
		currentStamina = maxStamina;
	}
	private void Update()
	{
		// Hồi stamina nếu không chạy và không tấn công
		if (currentStamina < maxStamina && !isRunning && !isAttacking &&!isJumping)
		{
			currentStamina += staminaRegenRate * Time.deltaTime;
			currentStamina = Mathf.Min(currentStamina, maxStamina);
			CharacterEvents.staminaChanged?.Invoke(gameObject, currentStamina, maxStamina);
		}

		
	}
	public bool UseStamina(float amount)
	{
		if (currentStamina >= amount)
		{
			currentStamina -= amount;
			currentStamina = Mathf.Max(currentStamina, 0); // Đảm bảo stamina không giảm dưới 0
			CharacterEvents.staminaChanged?.Invoke(gameObject, currentStamina, maxStamina); // Cập nhật stamina
			return true; // Đủ stamina để thực hiện hành động
		}
		return false; // Không đủ stamina
	}
	public bool HealStamina(float amount)
	{
		if (currentStamina < maxStamina)
		{
			float maxSta = Mathf.Max(maxStamina - currentStamina, 0);
			float actualSta = Mathf.Min(maxSta, amount);
			actualSta = Mathf.Round(actualSta * 100f) / 100f;
			currentStamina += actualSta;
			CharacterEvents.staminaHealed(gameObject,actualSta);
			CharacterEvents.staminaChanged?.Invoke(gameObject, currentStamina, maxStamina);
			return true;
		}
		return false;
	}
	public void IncreaseStamina(float amount)
	{
		maxStamina += amount;
		currentStamina = maxStamina; 
		CharacterEvents.staminaChanged?.Invoke(gameObject, currentStamina, maxStamina);

	}
	public void changeStaminaRegenRate(float amount)
	{
		staminaRegenRate += amount;
	}
	public void SetRunning(bool running)
	{
		isRunning = running;
	}
	public void SetMoving(bool moving)
	{
		isMoving = moving;
	}
	
	public void SetJumping(bool jumping)
	{
		isJumping = jumping;
	}

	public void SetAttacking(bool attacking)
	{
		isAttacking = attacking;
	}
}
