using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PyramidBuilder : MonoBehaviour
{
	public GameObject pegPrefab;   // Префаб колышка 
	public GameObject cellPrefab;  // Префаб ячейки
	public Transform canvasTransform;  // Ссылка на Canvas
	public int topRowCount = 3;    // Количество колышков в верхнем ряду
	public int bottomRowCount = 16; // Количество колышков в основании
	public float spacingMultiplier = 1.5f; // Множитель для расстояния между колышками
	public float desiredPegScale = 0.15f; // Значение масштаба, которое тебе нравится
	public float heightOffsetPercentage = 0.15f; // Процент от высоты камеры для смещения по Y
	public float heightOffset = 2f; // Смещение по Y
	private float finalScale;
	private Vector2 lastScreenSize; // Хранит последнее разрешение экрана
	private string pegPrefabTag = "Peg";
	private string cellPrefabTag = "Cell";
	private List<float> cellCoefficients = new List<float>(); // Список для хранения коэффициентов

	void Start()
	{
		lastScreenSize = new Vector2(Screen.width, Screen.height);
		GenerateCellCoefficients();
		GeneratePegPyramid();
	}

	void Update()
	{
		// Проверка изменения разрешения экрана или каждые 60 кадров
		if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y || Time.frameCount % 60 == 0)
		{
			GeneratePegPyramid();
			lastScreenSize = new Vector2(Screen.width, Screen.height);
		}
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

	void GeneratePegPyramid()
	{
		FindFinalScale();
		ClearOldObjects();
		CreatePyramid();
		ScaleBasedOnScreen();
	}

	void ClearOldObjects()
	{
		// Удаление старых колышков и ячеек
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
	}

	void CreatePyramid()
	{
		float pegDiameter = pegPrefab.GetComponent<Renderer>().bounds.size.x;
		float baseSpacing = pegDiameter * spacingMultiplier;
		float spacing = baseSpacing * (finalScale / desiredPegScale) * 1.5f;
		int rowCount = bottomRowCount - topRowCount + 1;

		for (int row = 0; row < rowCount; row++)
		{
			CreatePegRow(row, rowCount, pegDiameter, spacing);
		}
	}

	void CreatePegRow(int row, int rowCount, float pegDiameter, float spacing)
	{
		int pegsInRow = topRowCount + row;
		float rowOffset = -(pegsInRow - 1) * spacing / 2f;

		for (int col = 0; col < pegsInRow; col++)
		{
			Vector3 pegPosition = new Vector3(col * spacing + rowOffset, -row * spacing + heightOffset * finalScale, 0);
			GameObject peg = Instantiate(pegPrefab, pegPosition, Quaternion.identity, transform);

			if (row == rowCount - 1 && col < pegsInRow - 1)
			{
				CreateCell(peg.transform.position, col, pegDiameter);
			}
		}
	}

	void CreateCell(Vector3 pegPosition, int col, float pegDiameter)
	{
		Vector3 cellPosition = pegPosition + new Vector3(14 * pegDiameter * finalScale, -3.0f * finalScale, 0);
		GameObject cell = Instantiate(cellPrefab, canvasTransform);
		RectTransform cellRectTransform = cell.GetComponent<RectTransform>();

		Vector2 anchoredPosition = WorldToCanvasPosition(canvasTransform.GetComponent<Canvas>(), cellPosition);
		cellRectTransform.anchoredPosition = anchoredPosition;

		SetCellText(cell, col);
		CreateInvisibleCollider(anchoredPosition, cellRectTransform, col);
	}

	void SetCellText(GameObject cell, int col)
	{
		TextMeshProUGUI cellText = cell.transform.Find("MultiplierText").GetComponent<TextMeshProUGUI>();
		cellText.text = cellCoefficients[col].ToString("F1");
	}

	void CreateInvisibleCollider(Vector2 anchoredPosition, RectTransform cellRectTransform, int col)
	{
		Vector3 worldPosition = CanvasToWorldPosition(canvasTransform.GetComponent<Canvas>(), anchoredPosition);
		GameObject invisibleCollider = new GameObject("InvisibleCollider");
		invisibleCollider.transform.position = worldPosition;
		BoxCollider2D collider = invisibleCollider.AddComponent<BoxCollider2D>();
		collider.isTrigger = false;
		SpriteRenderer renderer = invisibleCollider.AddComponent<SpriteRenderer>();
		renderer.enabled = false;
		invisibleCollider.tag = "Cell";

		CoefficientHolder coefficientHolder = invisibleCollider.AddComponent<CoefficientHolder>();
		coefficientHolder.coefficient = cellCoefficients[col];

		invisibleCollider.transform.SetParent(transform);

		Vector2 pixelSize = new Vector2(cellRectTransform.rect.width, cellRectTransform.rect.height);
		Vector3 worldPos1, worldPos2;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(cellRectTransform, Vector2.zero, Camera.main, out worldPos1);
		RectTransformUtility.ScreenPointToWorldPointInRectangle(cellRectTransform, pixelSize, Camera.main, out worldPos2);
		Vector2 worldSize = new Vector2(Mathf.Abs(worldPos2.x - worldPos1.x), Mathf.Abs(worldPos2.y - worldPos1.y));
		collider.size = worldSize;
	}

	void FindFinalScale()
	{
		float baseHeight = 1920f;
		float baseWidth = 1080f;
		float scaleX = Screen.width / baseWidth;
		float scaleY = Screen.height / baseHeight;
		finalScale = Mathf.Max(scaleX, scaleY) * desiredPegScale;
		finalScale = Mathf.Clamp(finalScale, 0.09f, 0.9f);
	}

	void ScaleBasedOnScreen()
	{
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
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
		return new Vector2((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
						   (viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
	}

	Vector3 CanvasToWorldPosition(Canvas canvas, Vector2 canvasPosition)
	{
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		Vector2 viewportPosition = new Vector2((canvasPosition.x + (canvasRect.sizeDelta.x * 0.5f)) / canvasRect.sizeDelta.x,
												(canvasPosition.y + (canvasRect.sizeDelta.y * 0.5f)) / canvasRect.sizeDelta.y);
		return Camera.main.ViewportToWorldPoint(new Vector3(viewportPosition.x, viewportPosition.y, Camera.main.nearClipPlane));
	}
}
