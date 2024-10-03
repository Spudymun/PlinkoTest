using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiskFaller : MonoBehaviour
{
	public float fallDelay = 1.0f; // �������� ����� ������� �������
	public float destroyHeight = -10.0f; // ������, ���� ������� ���� ����� ���������
	private Rigidbody2D rb;
	private static float score = 0; // ����� ����
	public static TextMeshProUGUI scoreText;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.isKinematic = true; // ������� ������������� Rigidbody � �������������� �����
		Invoke("StartFalling", fallDelay); // ��������� ����� StartFalling ����� ��������

		UpdateScoreUI(); // ��������� UI ��� ������
	}

	void StartFalling()
	{
		rb.isKinematic = false; // ������� �������������� �����, ����� ���� ����� ������
	}

	void Update()
	{
		// ���������, ���� ���� ��������� ���� destroyHeight
		if (transform.position.y < destroyHeight)
		{
			Destroy(gameObject); // ���������� ����
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("Collision detected with: " + collision.gameObject.name); // �������� ��� ��� �������
																			// ��������� �������������� � ���������
		if (collision.gameObject.CompareTag("Peg"))
		{
			Debug.Log("Disk hit a peg!");
			// �� ������ �������� �������������� ������ �������������� �����

			// ������ ���� ��� ����, ����� ���� ������� �� ��������
			Vector2 contactPoint = collision.contacts[0].point; // ����� ��������
			Vector2 pegPosition = collision.transform.position; // ������� �������
			Vector2 direction = (contactPoint - pegPosition).normalized; // ����������� �� ������� � �����

			// ���������� ���� � ������� ������� ��� �������
			rb.AddForce(direction * 5f, ForceMode2D.Impulse); // ��������� ���� �� ������ ����������

			// ���������� �������� ��� ����� ������������� �������
			rb.AddTorque(10f); // ��������� ������ ���� �� ������ ����������
		}
		// ��������� �������������� � �������
		else if (collision.gameObject.CompareTag("Cell"))
		{
			CoefficientHolder coefficientHolder = collision.gameObject.GetComponent<CoefficientHolder>();
			if (coefficientHolder != null)
			{
				float coefficient = coefficientHolder.coefficient;
				score += 1 * coefficient; // ����������� ���� � ������ ������������
				UpdateScoreUI(); // ��������� UI
			}
			Destroy(gameObject); // ���������� ����
		}
	}

	public static void UpdateScoreUI()
	{
		if (scoreText != null)
		{
			scoreText.text = "Score: " + score.ToString("F1");
		}
	}

	public static void ResetScore()
	{
		score = 0; // �������� ����
		UpdateScoreUI(); // ��������� UI
	}
}
