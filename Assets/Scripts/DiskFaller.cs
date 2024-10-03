using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiskFaller : MonoBehaviour
{
	public float fallDelay = 1.0f; // Задержка перед началом падения
	public float destroyHeight = -10.0f; // Высота, ниже которой диск будет уничтожен
	private Rigidbody2D rb;
	private static float score = 0; // Общий счет
	public static TextMeshProUGUI scoreText;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.isKinematic = true; // Сначала устанавливаем Rigidbody в кинематический режим
		Invoke("StartFalling", fallDelay); // Запускаем метод StartFalling после задержки

		UpdateScoreUI(); // Обновляем UI при старте
	}

	void StartFalling()
	{
		rb.isKinematic = false; // Убираем кинематический режим, чтобы диск начал падать
	}

	void Update()
	{
		// Проверяем, если диск опустился ниже destroyHeight
		if (transform.position.y < destroyHeight)
		{
			Destroy(gameObject); // Уничтожаем диск
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("Collision detected with: " + collision.gameObject.name); // Добавьте это для отладки
																			// Обработка взаимодействия с колышками
		if (collision.gameObject.CompareTag("Peg"))
		{
			Debug.Log("Disk hit a peg!");
			// Вы можете добавить дополнительную логику взаимодействия здесь

			// Пример кода для того, чтобы диск катился по колышкам
			Vector2 contactPoint = collision.contacts[0].point; // Точка контакта
			Vector2 pegPosition = collision.transform.position; // Позиция колышка
			Vector2 direction = (contactPoint - pegPosition).normalized; // Направление от колышка к диску

			// Применение силы в сторону колышка для катания
			rb.AddForce(direction * 5f, ForceMode2D.Impulse); // Настройте силу по вашему усмотрению

			// Добавление вращения для более реалистичного катания
			rb.AddTorque(10f); // Настройте момент силы по вашему усмотрению
		}
		// Обработка взаимодействия с ячейкой
		else if (collision.gameObject.CompareTag("Cell"))
		{
			CoefficientHolder coefficientHolder = collision.gameObject.GetComponent<CoefficientHolder>();
			if (coefficientHolder != null)
			{
				float coefficient = coefficientHolder.coefficient;
				score += 1 * coefficient; // Увеличиваем счет с учетом коэффициента
				UpdateScoreUI(); // Обновляем UI
			}
			Destroy(gameObject); // Уничтожаем диск
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
		score = 0; // Обнуляем счет
		UpdateScoreUI(); // Обновляем UI
	}
}
