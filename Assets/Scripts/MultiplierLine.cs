using UnityEngine;

public class MultiplierLine : MonoBehaviour
{
	public GameObject cellPrefab;
	public Transform pyramidTransform;
	public float cellHeightOffset = 0.5f;

	void Start()
	{
		GenerateMultiplierCells();
	}

	void GenerateMultiplierCells()
	{
		// �������� ������� ������� ���� ��������
		float bottomRowYPosition = GetBottomRowYPosition();

		// �������� �� ���� �������� �������� ��������
		foreach (Transform child in pyramidTransform)
		{
			RectTransform childRectTransform = child.GetComponent<RectTransform>();
			if (childRectTransform != null && Mathf.Approximately(childRectTransform.anchoredPosition.y, bottomRowYPosition))
			{
				// �������� ���������� � �������
				Vector2 pegPosition = childRectTransform.anchoredPosition;
				float pegHeight = childRectTransform.rect.height;

				// ������������ ������� ������
				Vector2 cellPosition = new Vector2(pegPosition.x, pegPosition.y - pegHeight / 2 - cellHeightOffset);

				// ������� ������ ��� �������� ������� �������� � ������������� �� �������
				GameObject cellInstance = Instantiate(cellPrefab, child); // ����� ���������� child, � �� childTransform
				RectTransform cellRectTransform = cellInstance.GetComponent<RectTransform>();
				cellRectTransform.anchoredPosition = cellPosition;
			}
		}
	}

	float GetBottomRowYPosition()
	{
		// ������� ����������� �������� Y ����� ���� �������� �������� ��������
		float lowestY = float.MaxValue;
		foreach (Transform child in pyramidTransform)
		{
			RectTransform childRectTransform = child.GetComponent<RectTransform>();
			if (childRectTransform != null)
			{
				lowestY = Mathf.Min(lowestY, childRectTransform.anchoredPosition.y);
			}
		}
		return lowestY;
	}
}