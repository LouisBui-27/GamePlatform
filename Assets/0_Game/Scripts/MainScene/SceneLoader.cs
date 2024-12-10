using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static void LoadScene(string sceneName)
	{
		// G�n scene c?n t?i v�o LoadingManager
		loadingScene.sceneToLoad = sceneName;

		// Chuy?n sang scene Loading
		SceneManager.LoadScene("LoadingScene");
	}
}
