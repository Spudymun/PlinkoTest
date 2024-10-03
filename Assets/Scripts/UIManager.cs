using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private Button resetButton;

	void Start()
	{
		// Находим кнопку и привязываем метод ResetScore к ее событию onClick
		resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
		if (resetButton != null)
		{
			resetButton.onClick.AddListener(DiskFaller.ResetScore);
		}
		else
		{
			Debug.LogError("ResetButton object not found or missing Button component.");
		}

		// Находим объект TextMeshProUGUI по имени
		DiskFaller.scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
		if (DiskFaller.scoreText == null)
		{
			Debug.LogError("ScoreText object not found or missing TextMeshProUGUI component.");
		}
	}
}
