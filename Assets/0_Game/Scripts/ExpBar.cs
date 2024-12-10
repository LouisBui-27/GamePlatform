using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    public Slider ExpSlider;
	public TMP_Text expBarText;
	private ExpManage expManage;
	public TMP_Text levelText;

	private void Awake()
	{
		// Tìm đối tượng có tag "Player" và lấy StaminaManage component
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
		{
			Debug.Log("No player found");
			return;
		}

		expManage = player.GetComponent<ExpManage>();
	}
	private void Start()
	{
		if (expManage != null) 
		{
			updateExpBar(expManage.currentExp,expManage.ExpToUpLevel);
			UpdateLevelText(expManage.level);
		}
	}
	private void OnEnable()
	{
		// Đăng ký nghe sự kiện stamina
		CharacterEvents.expUsed += OnExpUsed;
		CharacterEvents.expChanged += OnExpChanged;
	}

	private void OnDisable()
	{
		// Hủy đăng ký sự kiện
		CharacterEvents.expUsed -= OnExpUsed;
		CharacterEvents.expChanged -= OnExpChanged;
	}

	private void OnExpUsed(GameObject character, int amount)
	{
		Debug.Log("1");
	}
	private void OnExpChanged(GameObject character, int currentExp, int ExpToUpLevel)
	{
		updateExpBar(currentExp,ExpToUpLevel);
		UpdateLevelText(expManage.level);
	}
	private void updateExpBar(int currentExp,int ExpToUpLevel)
	{
		ExpSlider.value = CalculateSliderPercentage(currentExp, ExpToUpLevel);
		expBarText.text = "Exp " + expManage.currentExp + " / " + expManage.ExpToUpLevel; ;
	}
	private void UpdateLevelText(int level)
	{
		levelText.text = "Level: " + level; // Cập nhật văn bản hiển thị level
	}
	private float CalculateSliderPercentage(int current, int max)
	{
		return (float)current / max;
	}
}
