using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private Button resetButton;

	void Start()
	{
		// ������� ������ � ����������� ����� ResetScore � �� ������� onClick
		resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
		if (resetButton != null)
		{
			resetButton.onClick.AddListener(DiskFaller.ResetScore);
		}
		else
		{
			Debug.LogError("ResetButton object not found or missing Button component.");
		}

		// ������� ������ TextMeshProUGUI �� �����
		DiskFaller.scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
		if (DiskFaller.scoreText == null)
		{
			Debug.LogError("ScoreText object not found or missing TextMeshProUGUI component.");
		}
	}
}
