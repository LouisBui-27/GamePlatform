using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
	public Slider staminaSlider;       // Thanh trượt cho stamina
	public TMP_Text staminaBarText;    // Văn bản hiển thị giá trị stamina
	private StaminaManage staminaManage;

	private void Awake()
	{
		// Tìm đối tượng có tag "Player" và lấy StaminaManage component
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
		{
			Debug.Log("No player found");
			return;
		}

		staminaManage = player.GetComponent<StaminaManage>();
	}

	private void OnEnable()
	{
		// Đăng ký nghe sự kiện stamina
		CharacterEvents.staminaHealed += OnstaminaHealed;
		CharacterEvents.staminaChanged += OnStaminaChange;
	}

	private void OnDisable()
	{
		// Hủy đăng ký sự kiện
		CharacterEvents.staminaHealed -= OnstaminaHealed;
		CharacterEvents.staminaChanged -= OnStaminaChange;
	}

	private void Start()
	{
		// Khởi tạo thanh stamina ban đầu
		if (staminaManage != null)
		{
			UpdateStaminaBar(staminaManage.currentStamina, staminaManage.maxStamina);
		}
	}
	private void OnstaminaHealed(GameObject character, float amount)
	{
		Debug.Log($"{character.name} đã sử dụng {amount} stamina.");
	}

	private void OnStaminaChange(GameObject character, float currentStamina, float maxStamina)
	{
		// Cập nhật UI với stamina mới
		UpdateStaminaBar(currentStamina, maxStamina);
	}

	private void UpdateStaminaBar(float currentStamina, float maxStamina)
	{
		staminaSlider.value = CalculateSliderPercentage(currentStamina, maxStamina);
		staminaBarText.text = "Stamina";
	}

	private float CalculateSliderPercentage(float current, float max)
	{
		return current / max;
	}
}
