using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class SceneController : MonoBehaviour
{
	public static SceneController instance;
	//public Button[] buttons;
	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		
	}
	public void NextLevel()
	{
		PlayerData.instance.SaveData();
		SceneLoader.LoadScene("Level" + (SceneManager.GetActiveScene().buildIndex + 1));
		//SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
		PlayerData.instance.LoadData();
	}
	
}
