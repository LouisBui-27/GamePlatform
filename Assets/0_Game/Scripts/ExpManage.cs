using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpManage : MonoBehaviour
{
    public int currentExp;
    public int ExpToUpLevel = 100;
    public int level = 1;

	public StaminaManage staminaManage;
	public ManaManage manaManage;
	public PlayerController playerController;

	private void Awake()
	{
		playerController = GetComponent<PlayerController>();
		staminaManage = GetComponent<StaminaManage>();
		manaManage = GetComponent<ManaManage>();
	}

	public void AddExp(int amount)
    {
        currentExp += amount;
		CharacterEvents.expChanged?.Invoke(gameObject, currentExp, ExpToUpLevel);
		PlayerPrefs.SetInt("currentExp", currentExp);
		if (currentExp >= ExpToUpLevel)
        {
            LevelUp();
        }
		
	}
    public void LevelUp()
    {
        currentExp -= ExpToUpLevel;
        level++;
		ExpToUpLevel = Mathf.FloorToInt(ExpToUpLevel + (level)  * 10);
		CharacterEvents.expChanged?.Invoke(gameObject, currentExp, ExpToUpLevel);

		playerController.IncreaseAttack(level * 1f);
		playerController.AddHealth(level * 5);
		manaManage.IncreaseMana(level * 10);
		manaManage.changeManaRegenRate((float)(level * .1));
		staminaManage.IncreaseStamina(level * 2);
		staminaManage.changeStaminaRegenRate((float)(level * .15));
		playerController.SaveHealthData();

		PlayerData.instance.SaveData();
	}
}
