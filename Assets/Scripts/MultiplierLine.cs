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
		// Получаем позицию нижнего ряда колышков
		float bottomRowYPosition = GetBottomRowYPosition();

		// Проходим по всем дочерним объектам пирамиды
		foreach (Transform child in pyramidTransform)
		{
			RectTransform childRectTransform = child.GetComponent<RectTransform>();
			if (childRectTransform != null && Mathf.Approximately(childRectTransform.anchoredPosition.y, bottomRowYPosition))
			{
				// Получаем информацию о колышке
				Vector2 pegPosition = childRectTransform.anchoredPosition;
				float pegHeight = childRectTransform.rect.height;

				// Рассчитываем позицию ячейки
				Vector2 cellPosition = new Vector2(pegPosition.x, pegPosition.y - pegHeight / 2 - cellHeightOffset);

				// Создаем ячейку как дочерний элемент пирамиды и устанавливаем ее позицию
				GameObject cellInstance = Instantiate(cellPrefab, child); // Здесь используем child, а не childTransform
				RectTransform cellRectTransform = cellInstance.GetComponent<RectTransform>();
				cellRectTransform.anchoredPosition = cellPosition;
			}
		}
	}

	float GetBottomRowYPosition()
	{
		// Находим минимальное значение Y среди всех дочерних объектов пирамиды
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