using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PyramidBuilder : MonoBehaviour
{
	public GameObject pegPrefab;   // ������ �������
	public GameObject cellPrefab;
	public Transform canvasTransform;  // ������ �� Canvas
	public int topRowCount = 3;    // ���������� �������� � ������� ����
	public int bottomRowCount = 16; // ���������� �������� � ���������
	public float spacingMultiplier = 1.5f; // ��������� ��� ���������� ����� ���������
	public float desiredPegScale = 0.15f; // �������� ��������, ������� ���� ��������
	public float heightOffsetPercentage = 0.15f; // ������� �� ������ ������ ��� �������� �� Y
	public float heightOffset = 2f;
	private float finalScale;
	private Vector2 lastScreenSize; // ������ ��������� ���������� ������
	private string pegPrefabTag = "Peg";
	private string cellPrefabTag = "Cell";
	private List<float> cellCoefficients = new List<float>(); // ������ ��� �������� �������������

	void Start()
	{
		lastScreenSize = new Vector2(Screen.width, Screen.height);
		GenerateCellCoefficients(); // ��������� ������������� �� ������
		GeneratePegPyramid();
	}

	void GenerateCellCoefficients()
	{
		// ��������� ��������� ������������� �� -5.0 �� +5.0
		for (int i = 0; i < bottomRowCount - 1; i++)
		{
			float coefficient = Random.Range(-5.0f, 5.0f);
			cellCoefficients.Add(coefficient);
		}
	}

	void Update()
	{
		// ���������, ���������� �� ���������� ������
		if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y || Time.frameCount % 60 == 0)
		{
			GeneratePegPyramid();
			lastScreenSize = new Vector2(Screen.width, Screen.height); // ��������� ��������� ����������
		}
	}

	void GeneratePegPyramid()
	{
		FindFinalScale();
		Transform canvasTransform = GameObject.Find("Canvas").transform;
		// ������� ������ �������, ���� ��� ����
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

		// �������� ������� ������� �� ��� X
		float pegDiameter = pegPrefab.GetComponent<Renderer>().bounds.size.x;

		// ������������� ������� ���������� ����� ���������
		float baseSpacing = pegDiameter * spacingMultiplier;

		// ������������ spacing � ������ �������� ��������
		float spacing = baseSpacing * (finalScale / desiredPegScale) * 1.5f;

		int rowCount = bottomRowCount - topRowCount + 1; // ����� ���������� �����

		for (int row = 0; row < rowCount; row++)
		{
			// ���������� �������� � ������� ����
			int pegsInRow = topRowCount + row;

			// ��������� �������� ��� ������������� ����
			float rowOffset = -(pegsInRow - 1) * spacing / 2f;

			for (int col = 0; col < pegsInRow; col++)
			{
				// ��������� ������� ��� �������
				Vector3 pegPosition = new Vector3(col * spacing + rowOffset, -row * spacing + heightOffset * finalScale, 0);

				// ������� �������
				GameObject peg = Instantiate(pegPrefab, pegPosition, Quaternion.identity, transform);

				// ���������, �������� �� ������� ��� ���������
				if (row == rowCount - 1 && col < pegsInRow - 1)
				{
					// ������ �������� ������� ������� ����� ��� ��������
					Vector3 actualPegPosition = peg.transform.position;
					// ��������� ������� ������ ���� �������
					Vector3 cellPosition = actualPegPosition + new Vector3(14*pegDiameter * finalScale, -3.0f * finalScale, 0); // ������� ���� ������� � ������ ��������

					// ������� ������ � ������������� �� �������
					GameObject cell = Instantiate(cellPrefab, canvasTransform);
					RectTransform cellRectTransform = cell.GetComponent<RectTransform>();

					// ����������� ������� ���������� � ��������� ��� Canvas
					Vector2 anchoredPosition = WorldToCanvasPosition(canvasTransform.GetComponent<Canvas>(), cellPosition);
					// ������������� ������ � ������ �����
					cellRectTransform.anchoredPosition = anchoredPosition;

					// ������������� �������� � ��������� TextMeshPro
					Transform multiplierTextTransform = cell.transform.Find("MultiplierText");
					TextMeshProUGUI cellText = multiplierTextTransform.GetComponent<TextMeshProUGUI>();
					cellText.text = cellCoefficients[col].ToString("F1"); // ����������� �������� ������������

					// ����������� ���������� Canvas � ������� ����������
					Vector3 worldPosition = CanvasToWorldPosition(canvasTransform.GetComponent<Canvas>(), anchoredPosition);
					// ������� ��������� ���������
					GameObject invisibleCollider = new GameObject("InvisibleCollider");
					invisibleCollider.transform.position = worldPosition;
					BoxCollider2D collider = invisibleCollider.AddComponent<BoxCollider2D>();
					collider.isTrigger = false;
					SpriteRenderer renderer = invisibleCollider.AddComponent<SpriteRenderer>();
					renderer.enabled = false;
					// ��������� ���
					invisibleCollider.tag = "Cell";
					// ��������� ��������� CoefficientHolder � ������������� �������� ������������
					CoefficientHolder coefficientHolder = invisibleCollider.AddComponent<CoefficientHolder>();
					coefficientHolder.coefficient = cellCoefficients[col];
					// ������������� PegPyramid ��� ������������ ������
					invisibleCollider.transform.SetParent(transform);

					Vector2 pixelSize = new Vector2(cellRectTransform.rect.width, cellRectTransform.rect.height);

					// ����������� ������� � ������� ����������
					RectTransformUtility.ScreenPointToWorldPointInRectangle(cellRectTransform, new Vector2(0, 0), Camera.main, out Vector3 worldPos1);
					RectTransformUtility.ScreenPointToWorldPointInRectangle(cellRectTransform, pixelSize, Camera.main, out Vector3 worldPos2);

					// ��������� ������� � �������� ����
					Vector2 worldSize = new Vector2(
						Mathf.Abs(worldPos2.x - worldPos1.x),
						Mathf.Abs(worldPos2.y - worldPos1.y)
					);

					// ������������� ������ ����������
					collider.size = worldSize;

				}
			}
		}

		ScaleBasedOnScreen();
	}

	void FindFinalScale()
	{
		// ������ ������� ����������
		float baseHeight = 1920f;
		float baseWidth = 1080f;

		// �������� ������� ���������� ������
		float currentWidth = Screen.width;
		float currentHeight = Screen.height;
		// ������������ ������������ ��������������� ��� ������ � ������
		float scaleX = currentWidth / baseWidth;
		float scaleY = currentHeight / baseHeight;

		// ���������� ������� �������
		finalScale = Mathf.Max(scaleX, scaleY);

		// �������� �� �������� �������
		finalScale *= desiredPegScale; // ����� desiredPegScale - ��� ��� �������, ������� ���� ��������
		finalScale = Mathf.Clamp(finalScale, 0.09f, 0.9f);
	}

	void ScaleBasedOnScreen()
	{
		// ������������ ������ �������
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
		// �������� RectTransform Canvas
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		// ����������� ������� ������� � ���������� Viewport
		Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);

		// ����������� ���������� Viewport � ���������� Canvas
		Vector2 canvasPosition = new Vector2(
			(viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
			(viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
		);

		return canvasPosition;
	}

	Vector3 CanvasToWorldPosition(Canvas canvas, Vector2 canvasPosition)
	{
		// �������� RectTransform Canvas
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();

		// ����������� ���������� Canvas � ���������� Viewport
		Vector2 viewportPosition = new Vector2(
			(canvasPosition.x + (canvasRect.sizeDelta.x * 0.5f)) / canvasRect.sizeDelta.x,
			(canvasPosition.y + (canvasRect.sizeDelta.y * 0.5f)) / canvasRect.sizeDelta.y
		);

		// ����������� ���������� Viewport � ������� ����������
		Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(viewportPosition.x, viewportPosition.y, Camera.main.nearClipPlane));

		return worldPosition;
	}

}
