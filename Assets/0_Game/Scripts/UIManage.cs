using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManage : MonoBehaviour
{
    public GameObject damageText;
    public GameObject healthText;
	public GameObject staminaText;
    public Canvas gameCanvas;

	private void Awake()
	{
		gameCanvas = FindObjectOfType<Canvas>();
	}

	private void OnEnable()
	{
		CharacterEvents.characterDamaged += (CharacterTookDamage);
		CharacterEvents.characterHealed += (CharacterHealed);
		CharacterEvents.staminaHealed += (StaminaHealed);
	}
	private void OnDisable()
	{
		CharacterEvents.characterDamaged -= (CharacterTookDamage);
		CharacterEvents.characterHealed -= (CharacterHealed);
		CharacterEvents.staminaHealed -= (StaminaHealed);
	}
	public void CharacterTookDamage(GameObject character, float damageReceive)
    {
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);
        TMP_Text tmpText = Instantiate(damageText,spawnPos, Quaternion.identity,gameCanvas.transform)
            .GetComponent<TMP_Text>();
        tmpText.text = damageReceive.ToString();
    
    }
    public void CharacterHealed(GameObject character, float healthRestored)
    {
		Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);
		TMP_Text tmpText = Instantiate(healthText, spawnPos, Quaternion.identity, gameCanvas.transform)
			.GetComponent<TMP_Text>();
		tmpText.text = healthRestored.ToString();
	}
	public void StaminaHealed(GameObject character, float staminaHealed)
	{
		Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);
		TMP_Text tmpText = Instantiate(staminaText, spawnPos, Quaternion.identity, gameCanvas.transform)
			.GetComponent<TMP_Text>();
		tmpText.text = staminaHealed.ToString();
	}
	public void OnExitGame(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			#if (UNITY_EDITOR || DEVELOPMENT_BIULD)
				Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
			#endif
			#if (UNITY_EDITOR)
				UnityEditor.EditorApplication.isPlaying = false;
			#elif (UNITY_STANDALONE)
				Application.Quit();
			#elif (UNITY_WEBGL)
				SceneManage.LoadSnece("QuitScene");
			#endif
		}
	}
}
