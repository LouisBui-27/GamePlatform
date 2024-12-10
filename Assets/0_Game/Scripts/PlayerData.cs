using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
	public static PlayerData instance;

	private StaminaManage staminaManage;
	private ManaManage manaManage;
	private ExpManage expManage;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			//DontDestroyOnLoad(gameObject); // Giữ đối tượng này qua các cảnh
		}
		else
		{
			Destroy(gameObject); // Nếu đã có instance, thì hủy đối tượng này
		}

		// Lấy tham chiếu đến các component
		staminaManage = FindObjectOfType<StaminaManage>();
		manaManage = FindObjectOfType<ManaManage>();
		expManage = FindObjectOfType<ExpManage>();

		// Đảm bảo các giá trị ban đầu của mana, stamina, exp...
		LoadData();
	}

	// Hàm lưu trữ dữ liệu
	public void SaveData()
	{
		// Lưu các giá trị vào PlayerPrefs
		PlayerPrefs.SetFloat("maxStamina", staminaManage.maxStamina);
		PlayerPrefs.SetFloat("currentStamina", staminaManage.maxStamina);
		PlayerPrefs.SetFloat("staminaRegenRate", staminaManage.staminaRegenRate);
		PlayerPrefs.SetFloat("maxMana", manaManage.maxMana);
		PlayerPrefs.SetFloat("currentMana", manaManage.maxMana);
		PlayerPrefs.SetFloat("manaRegenRate", manaManage.manaRegenRate);
		PlayerPrefs.SetInt("level", expManage.level);
		PlayerPrefs.SetInt("currentExp", expManage.currentExp);
		PlayerPrefs.SetInt("expToUpLevel", expManage.ExpToUpLevel);

		PlayerPrefs.Save(); // Lưu ngay lập tức
		Debug.Log("Player data saved");
	}

	// Hàm tải dữ liệu
	public void LoadData()
	{
		// Kiểm tra nếu có dữ liệu trong PlayerPrefs trước khi tải
		if (PlayerPrefs.HasKey("maxStamina"))
		{
			staminaManage.maxStamina = PlayerPrefs.GetFloat("maxStamina");
			staminaManage.staminaRegenRate = PlayerPrefs.GetFloat("staminaRegenRate");
			
		}
		if (PlayerPrefs.HasKey("currentStamina"))
		{
			staminaManage.currentStamina = PlayerPrefs.GetFloat("currentStamina");
		}
		else
		{
			// Giá trị mặc định nếu không có trong PlayerPrefs
			staminaManage.maxStamina = 40f;
			staminaManage.currentStamina = 40f;
			staminaManage.staminaRegenRate = 1f;
		}

		if (PlayerPrefs.HasKey("maxMana"))
		{
			manaManage.maxMana = PlayerPrefs.GetFloat("maxMana");
			manaManage.currentMana = PlayerPrefs.GetFloat("currentMana");
			manaManage.manaRegenRate = PlayerPrefs.GetFloat("manaRegenRate");
		}
		else
		{
			// Giá trị mặc định nếu không có trong PlayerPrefs
			manaManage.maxMana = 100f;
			manaManage.currentMana = 100f;
			manaManage.manaRegenRate = 1f;
		}

		if (PlayerPrefs.HasKey("level"))
		{
			expManage.level = PlayerPrefs.GetInt("level");
		}
		else
		{
			// Giá trị mặc định nếu không có trong PlayerPrefs
			expManage.level = 1;
		}

		if (PlayerPrefs.HasKey("currentExp"))
		{
			expManage.currentExp = PlayerPrefs.GetInt("currentExp");
		}
		else
		{
			// Giá trị mặc định nếu không có trong PlayerPrefs
			expManage.currentExp = 0;
		}
		if (PlayerPrefs.HasKey("expToUpLevel"))
		{
			expManage.ExpToUpLevel = PlayerPrefs.GetInt("expToUpLevel", 100);
		}

		Debug.Log("Player data loaded");
	}
}
