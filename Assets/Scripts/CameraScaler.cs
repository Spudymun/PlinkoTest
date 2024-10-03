using UnityEngine;

public class CameraScaler : MonoBehaviour
{
	public float minWidth = 1136f;  // Минимальная ширина экрана
	public float maxWidth = 2732f;  // Максимальная ширина экрана
	public float minHeight = 640f;  // Минимальная высота экрана
	public float maxHeight = 2048f; // Максимальная высота экрана
	public float minOrthographicSize = 4f;  // Минимальный размер камеры
	public float maxOrthographicSize = 25f; // Максимальный размер камеры
	private Vector2 lastScreenSize;  // Хранит последнее разрешение экрана

	void Start()
	{
		lastScreenSize = new Vector2(Screen.width, Screen.height);
		AdjustCamera();
	}

	void Update()
	{
		// Проверяем, изменилось ли разрешение экрана
		if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
		{
			AdjustCamera();
			lastScreenSize = new Vector2(Screen.width, Screen.height); // Обновляем последнее разрешение
		}
	}

	void AdjustCamera()
	{
		// Получаем текущее разрешение экрана
		float currentWidth = Screen.width;
		float currentHeight = Screen.height;

		// Определяем, вертикальная ориентация или горизонтальная
		bool isPortrait = currentHeight > currentWidth;

		// Рассчитываем масштаб на основе минимальных и максимальных разрешений
		float widthFactor = Mathf.InverseLerp(minWidth, maxWidth, isPortrait ? currentHeight : currentWidth);
		float heightFactor = Mathf.InverseLerp(minHeight, maxHeight, isPortrait ? currentWidth : currentHeight);

		// Рассчитываем среднее значение между коэффициентами по ширине и высоте
		float averageFactor = (widthFactor + heightFactor) / 2f;

		// Рассчитываем итоговый ортографический размер на основе среднего коэффициента
		float orthographicSize = Mathf.Lerp(minOrthographicSize, maxOrthographicSize, averageFactor);

		// Ограничиваем ортографический размер в допустимых пределах
		orthographicSize = Mathf.Clamp(orthographicSize, minOrthographicSize, maxOrthographicSize);

		// Применяем новый размер камеры
		Camera.main.orthographicSize = orthographicSize;

		// Выводим значение ортографического размера в консоль для отладки
		Debug.Log($"Camera Orthographic Size: {Camera.main.orthographicSize}");
	}
}
