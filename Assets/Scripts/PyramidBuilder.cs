using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PyramidBuilder : MonoBehaviour
{
	public GameObject pegPrefab;   // Префаб колышка
	public GameObject cellPrefab;
	public Transform canvasTransform;  // Ссылка на Canvas
	public int topRowCount = 3;    // Количество колышков в верхнем ряду
	public int bottomRowCount = 16; // Количество колышков в основании
	public float spacingMultiplier = 1.5f; // Множитель для расстояния между колышками
	public float desiredPegScale = 0.15f; // Значение масштаба, которое тебе нравится
	public float heightOffsetPercentage = 0.15f; // Процент от высоты камеры для смещения по Y
	public float heightOffset = 2f;
	private float finalScale;
	private Vector2 lastScreenSize; // Хранит последнее разрешение экрана
	private string pegPrefabTag = "Peg";
	private string cellPrefabTag = "Cell";
	private List<float> cellCoefficients = new List<float>(); // Список для хранения коэффициентов

	void Start()
	{
		lastScreenSize = new Vector2(Screen.width, Screen.height);
		GenerateCellCoefficients(); // Генерация коэффициентов на старте
		GeneratePegPyramid();
	}

	void GenerateCellCoefficients()
	{
		// Генерация случайных коэффициентов от -5.0 до +5.0
		for (int i = 0; i < bottomRowCount - 1; i++)
		{
			float coefficient = Random.Range(-5.0f, 5.0f);
			cellCoefficients.Add(coefficient);
		}
	}

	void Update()
	{
		// Проверяем, изменилось ли разрешение экрана
		if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y || Time.frameCount % 60 == 0)
		{
			GeneratePegPyramid();
			lastScreenSize = new Vector2(Screen.width, Screen.height); // Обновляем последнее разрешение
		}
	}

	void GeneratePegPyramid()
	{
		FindFinalScale();
		Transform canvasTransform = GameObject.Find("Canvas").transform;
		// Удаляем старые колышки, если они есть
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in canvasTransform)
		{
			if (child.CompareTag(cellPrefabTag))
			{
				Destroy(child.gameObject);
			}
		}

		// Получаем диаметр колышка по оси X
		float pegDiameter = pegPrefab.GetComponent<Renderer>().bounds.size.x;

		// Устанавливаем базовое расстояние между колышками
		float baseSpacing = pegDiameter * spacingMultiplier;

		// Рассчитываем spacing с учетом масштаба колышков
		float spacing = baseSpacing * (finalScale / desiredPegScale) * 1.5f;

		int rowCount = bottomRowCount - topRowCount + 1; // Общее количество рядов

		for (int row = 0; row < rowCount; row++)
		{
			// Количество колышков в текущем ряду
			int pegsInRow = topRowCount + row;

			// Вычисляем смещение для центрирования ряда
			float rowOffset = -(pegsInRow - 1) * spacing / 2f;

			for (int col = 0; col < pegsInRow; col++)
			{
				// Вычисляем позицию для колышка
				Vector3 pegPosition = new Vector3(col * spacing + rowOffset, -row * spacing + heightOffset * finalScale, 0);

				// Создаем колышек
				GameObject peg = Instantiate(pegPrefab, pegPosition, Quaternion.identity, transform);

				// Проверяем, является ли текущий ряд последним
				if (row == rowCount - 1 && col < pegsInRow - 1)
				{
					// Теперь получаем позицию колышка после его создания
					Vector3 actualPegPosition = peg.transform.position;
					// Вычисляем позицию ячейки ниже колышка
					Vector3 cellPosition = actualPegPosition + new Vector3(14*pegDiameter * finalScale, -3.0f * finalScale, 0); // Позиция ниже колышка с учетом масштаба

					// Создаем ячейку и устанавливаем ее позицию
					GameObject cell = Instantiate(cellPrefab, canvasTransform);
					RectTransform cellRectTransform = cell.GetComponent<RectTransform>();

					// Преобразуем мировые координаты в локальные для Canvas
					Vector2 anchoredPosition = WorldToCanvasPosition(canvasTransform.GetComponent<Canvas>(), cellPosition);
					// Устанавливаем ячейку в нужное место
					cellRectTransform.anchoredPosition = anchoredPosition;

					// Устанавливаем значение в компонент TextMeshPro
					Transform multiplierTextTransform = cell.transform.Find("MultiplierText");
					TextMeshProUGUI cellText = multiplierTextTransform.GetComponent<TextMeshProUGUI>();
					cellText.text = cellCoefficients[col].ToString("F1"); // Присваиваем значение коэффициента

					// Преобразуем координаты Canvas в мировые координаты
					Vector3 worldPosition = CanvasToWorldPosition(canvasTransform.GetComponent<Canvas>(), anchoredPosition);
					// Создаем невидимый коллайдер
					GameObject invisibleCollider = new GameObject("InvisibleCollider");
					invisibleCollider.transform.position = worldPosition;
					BoxCollider2D collider = invisibleCollider.AddComponent<BoxCollider2D>();
					collider.isTrigger = false;
					SpriteRenderer renderer = invisibleCollider.AddComponent<SpriteRenderer>();
					renderer.enabled = false;
					// Добавляем тег
					invisibleCollider.tag = "Cell";
					// Добавляем компонент CoefficientHolder и устанавливаем значение коэффициента
					CoefficientHolder coefficientHolder = invisibleCollider.AddComponent<CoefficientHolder>();
					coefficientHolder.coefficient = cellCoefficients[col];
					// Устанавливаем PegPyramid как родительский объект
					invisibleCollider.transform.SetParent(transform);

					Vector2 pixelSize = new Vector2(cellRectTransform.rect.width, cellRectTransform.rect.height);

					// Преобразуем размеры в мировые координаты
					RectTransformUtility.ScreenPointToWorldPointInRectangle(cellRectTransform, new Vector2(0, 0), Camera.main, out Vector3 worldPos1);
					RectTransformUtility.ScreenPointToWorldPointInRectangle(cellRectTransform, pixelSize, Camera.main, out Vector3 worldPos2);

					// Вычисляем размеры в единицах мира
					Vector2 worldSize = new Vector2(
						Mathf.Abs(worldPos2.x - worldPos1.x),
						Mathf.Abs(worldPos2.y - worldPos1.y)
					);

					// Устанавливаем размер коллайдера
					collider.size = worldSize;

				}
			}
		}

		ScaleBasedOnScreen();
	}

	void FindFinalScale()
	{
		// Задаем базовое разрешение
		float baseHeight = 1920f;
		float baseWidth = 1080f;

		// Получаем текущее разрешение экрана
		float currentWidth = Screen.width;
		float currentHeight = Screen.height;
		// Рассчитываем коэффициенты масштабирования для ширины и высоты
		float scaleX = currentWidth / baseWidth;
		float scaleY = currentHeight / baseHeight;

		// Используем средний масштаб
		finalScale = Mathf.Max(scaleX, scaleY);

		// Умножаем на желаемый масштаб
		finalScale *= desiredPegScale; // Здесь desiredPegScale - это тот масштаб, который тебе нравится
		finalScale = Mathf.Clamp(finalScale, 0.09f, 0.9f);
	}

	void ScaleBasedOnScreen()
	{
		// Масштабируем каждый колышек
		foreach (Transform child in transform)
		{
			if (child.CompareTag(pegPrefabTag))
			{
				child.localScale = new Vector3(finalScale, finalScale, finalScale);
			}
		}
	}

	Vector2 WorldToCanvasPosition(Canvas canvas, Vector3 worldPosition)
	{
		// Получаем RectTransform Canvas
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		// Преобразуем мировую позицию в координаты Viewport
		Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);

		// Преобразуем координаты Viewport в координаты Canvas
		Vector2 canvasPosition = new Vector2(
			(viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
			(viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
		);

		return canvasPosition;
	}

	Vector3 CanvasToWorldPosition(Canvas canvas, Vector2 canvasPosition)
	{
		// Получаем RectTransform Canvas
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		// Преобразуем координаты Canvas в координаты Viewport
		Vector2 viewportPosition = new Vector2(
			(canvasPosition.x + (canvasRect.sizeDelta.x * 0.5f)) / canvasRect.sizeDelta.x,
			(canvasPosition.y + (canvasRect.sizeDelta.y * 0.5f)) / canvasRect.sizeDelta.y
		);

		// Преобразуем координаты Viewport в мировые координаты
		Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(viewportPosition.x, viewportPosition.y, Camera.main.nearClipPlane));

		return worldPosition;
	}

}
