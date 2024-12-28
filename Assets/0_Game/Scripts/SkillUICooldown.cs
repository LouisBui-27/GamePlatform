using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUICooldown : MonoBehaviour
{
    public Image cooldownOverlay; // Hình ?nh che ph?
    public Text cooldownText; // Text hi?n th? th?i gian
    private float cooldownDuration; // Th?i gian h?i chiêu
    private float cooldownTimer; // B? ??m th?i gian

    public void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownTimer = duration;
        cooldownOverlay.fillAmount = 1f;
        cooldownText.text = cooldownDuration.ToString("F1");
        cooldownOverlay.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;

            // C?p nh?t UI
            cooldownOverlay.fillAmount = cooldownTimer / cooldownDuration;
            cooldownText.text = Mathf.Ceil(cooldownTimer).ToString();

            if (cooldownTimer <= 0)
            {
                // K?t thúc h?i chiêu
                cooldownOverlay.gameObject.SetActive(false);
                cooldownText.gameObject.SetActive(false);
            }
        }
    }
}
