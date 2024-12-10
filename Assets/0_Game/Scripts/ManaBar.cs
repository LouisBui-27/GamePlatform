using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
	public Slider manaSlider;       
	public TMP_Text manaBarText;    
	private ManaManage manaManage;

	private void Awake()
	{
		// Tìm đối tượng có tag "Player" và lấy StaminaManage component
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
		{
			Debug.Log("No player found");
			return;
		}

		manaManage = player.GetComponent<ManaManage>();
	}

	private void Start()
	{
		// Khởi tạo thanh stamina ban đầu
		if (manaManage != null)
		{
			UpdateManaBar(manaManage.currentMana, manaManage.maxMana);
		}
	}
	private void OnEnable()
	{
		// Đăng ký nghe sự kiện stamina
		CharacterEvents.manaUsed += OnManaUsed;
		CharacterEvents.manaChanged += OnManaChange;
	}

	private void OnDisable()
	{
		// Hủy đăng ký sự kiện
		CharacterEvents.manaUsed -= OnManaUsed;
		CharacterEvents.manaChanged -= OnManaChange;
	}
	private void OnManaUsed(GameObject character, float amount)
	{
		Debug.Log($"{character.name} đã sử dụng {amount} stamina.");
	}
	private void OnManaChange(GameObject character, float currentMana, float maxMana)
	{
		// Cập nhật UI với stamina mới
		UpdateManaBar(currentMana, maxMana);
	}

	private void UpdateManaBar(float currentMana, float maxMana)
	{
		manaSlider.value = CalculateSliderPercentage(currentMana, maxMana);
		manaBarText.text = "Mana";
	}

	private float CalculateSliderPercentage(float current, float max)
	{
		return current / max;
	}
}
