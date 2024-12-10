using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoLevel : MonoBehaviour
{
	public Canvas infoCanvas; // Tham chiếu đến Canvas chứa thông tin
	public Text infoText; // Text hiển thị nội dung (hoặc TMP_Text nếu bạn sử dụng TextMeshPro)
	public string infoContent; // Nội dung cần hiển thị
	public float typingSpeed = 0.05f; // Tốc độ gõ chữ

	private bool isPaused = false; // Biến kiểm tra trạng thái tạm dừng trò chơi
	private Coroutine typingCoroutine;

	private void Start()
	{
		// Hiển thị Canvas và tạm dừng trò chơi khi vào màn chơi
		ShowInfoCanvas();
	}

	public void ShowInfoCanvas()
	{
		// Kích hoạt Canvas và tạm dừng trò chơi
		infoCanvas.gameObject.SetActive(true);
		PauseGame();

		// Hiển thị chữ chạy
		if (typingCoroutine != null)
		{
			StopCoroutine(typingCoroutine);
		}
		typingCoroutine = StartCoroutine(TypeText(infoContent));
	}

	public void CloseInfoCanvas()
	{
		// Đóng Canvas và tiếp tục trò chơi
		ResumeGame();
		infoCanvas.gameObject.SetActive(false);
	}

	private void PauseGame()
	{
		// Tạm dừng trò chơi
		isPaused = true;
		Time.timeScale = 0f; // Dừng thời gian
	}

	private void ResumeGame()
	{
		// Tiếp tục trò chơi
		isPaused = false;
		Time.timeScale = 1f; // Khôi phục thời gian
	}

	private IEnumerator TypeText(string content)
	{
		infoText.text = "";
		foreach (char letter in content.ToCharArray())
		{
			infoText.text += letter; // Thêm từng ký tự vào Text
			yield return new WaitForSecondsRealtime(typingSpeed); // Chờ trước khi thêm ký tự tiếp theo
		}
	}

	// Hàm được gọi khi nhấn nút
	public void OnButtonPress()
	{
		// Dừng Coroutine đang chạy (nếu có)
		if (typingCoroutine != null)
		{
			StopCoroutine(typingCoroutine);
		}

		// Hiển thị toàn bộ văn bản ngay lập tức
		infoText.text = infoContent;

		// Đợi một thời gian cho đến khi chữ chạy xong
		StartCoroutine(WaitAndCloseCanvas());
	}

	// Hàm này sẽ đợi một khoảng thời gian ngắn sau khi hiển thị hết chữ rồi đóng canvas
	private IEnumerator WaitAndCloseCanvas()
	{
		// Đợi một chút trước khi đóng canvas (ví dụ: đợi thêm 1 giây)
		yield return new WaitForSecondsRealtime(1f); // Thời gian đợi có thể điều chỉnh

		// Đóng canvas sau khi hiển thị xong
		CloseInfoCanvas();
	}
}
