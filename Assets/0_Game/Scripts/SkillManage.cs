using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SkillManage : MonoBehaviour
{
    Attack attack;
    StaminaManage stamina;
    ManaManage mana;
    PlayerController playerController;
    Damagable damagable;
   // ExpManage exp;

    public float buffMultiplier = 1.5f; // Tăng 50%
    public float buffDuration = 3f; // Thời gian hiệu lực của buff
    public float buffCooldown = 20f; // Thời gian chờ trước khi dùng lại buff
    public bool isBuffActive = false;
    public bool isBuffOnCooldown = false;

    public float rangeAttackCooldown = 15f;
    public bool isRangeAttackCooldown = false;

    public float buffForAllCooldown = 15f;
    public bool isbuffForAllCooldown = false;
    public float buffForAllDuration = 10f;
    public bool isBuffForAllActive = false;

    public SkillUICooldown rangeAttackUI; // UI cho Range Attack
    public SkillUICooldown buffAttackUI; // UI cho Buff Attack
    public SkillUICooldown buffForAllUI; // UI cho Buff For All

    private void Awake()
    {
        attack = GetComponent<Attack>();
        mana = GetComponent<ManaManage>();
        stamina = GetComponent<StaminaManage>();
        playerController = GetComponent<PlayerController>();
        damagable = GetComponent<Damagable>();
    }
    public IEnumerator ActivateBuff()
    {
        isBuffActive = true;
        isBuffOnCooldown = true;

       

        // Tìm các đối tượng Attack trong các con của Player
        Attack[] attackComponents = GetComponentsInChildren<Attack>();
        foreach (Attack attack in attackComponents)
        {
            attack.attackDam *= buffMultiplier; // Tăng damage
        }

        // Chờ hết thời gian buff
        yield return new WaitForSeconds(buffDuration);

        // Khôi phục lại damage ban đầu
        foreach (Attack attack in attackComponents)
        {
            attack.attackDam /= buffMultiplier;
        }

        isBuffActive = false;

        // Bắt đầu thời gian hồi chiêu
        buffAttackUI.StartCooldown(buffCooldown);
        yield return new WaitForSeconds(buffCooldown);
        isBuffOnCooldown = false;
    }

    public IEnumerator ActiveBuffForAll()
    {
        isBuffForAllActive = true;
        isbuffForAllCooldown = true;

        mana.manaRegenRate *= 5f;
        stamina.staminaRegenRate *= 3f;
        playerController.speed *= 1.5f;
        playerController.jumpImpulse *= 1.2f;
        damagable.Heal(30f);
 
        yield return new WaitForSeconds(buffForAllDuration);

        mana.manaRegenRate /= 5f;
        stamina.staminaRegenRate /= 3f;
        playerController.speed /= 1.5f;
        playerController.jumpImpulse /= 1.2f;

        isBuffForAllActive = false;
        buffForAllUI.StartCooldown(buffForAllCooldown);
        yield return new WaitForSeconds(buffForAllCooldown);
       

        isbuffForAllCooldown = false;
    }

    public IEnumerator ActiveRangeAttack()
    {
        isRangeAttackCooldown = true;
        rangeAttackUI.StartCooldown(rangeAttackCooldown);
        yield return new WaitForSeconds(rangeAttackCooldown);
       
        isRangeAttackCooldown = false;
    }
}
