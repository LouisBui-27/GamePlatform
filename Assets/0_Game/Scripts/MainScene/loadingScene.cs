using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadingScene : MonoBehaviour
{
	public Slider progressBar; // Thanh tiến trình
	public Text progressText; // Text hiển thị %

	public static string sceneToLoad; // Scene cần tải
	private void Start()
	{
		StartCoroutine(LoadSceneAsync());
	}

	private IEnumerator LoadSceneAsync()
	{
		// Bắt đầu tải scene cần đến
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);

		// Vô hiệu hóa tự động chuyển đổi để kiểm soát quá trình
		operation.allowSceneActivation = false;

		while (!operation.isDone)
		{
			// Lấy tiến trình tải (từ 0 đến 0.9)
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			progressBar.value = progress; // Cập nhật thanh tiến trình
			progressText.text = (progress * 100).ToString("F0") + "%"; // Cập nhật % text

			// Kiểm tra nếu tải xong (progress = 0.9) và chờ kích hoạt
			if (operation.progress >= 0.9f)
			{
				// Cho phép chuyển sang scene mới
				operation.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}
