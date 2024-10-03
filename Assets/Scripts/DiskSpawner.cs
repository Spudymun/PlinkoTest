using UnityEngine;

public class DiskSpawner : MonoBehaviour
{
	public GameObject diskPrefab; // Префаб диска
	public float spawnHeight = 3.5f; // Высота спавна над колышками
	private float lastSpawnTime; // Время последнего спавна
	public float spawnInterval = 1f; // Интервал между спавнами в секундах

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastSpawnTime + spawnInterval) // Проверка нажатия кнопки пробел и интервала
		{
			SpawnDisk();
			lastSpawnTime = Time.time; // Обновляем время последнего спавна
		}
	}

	void SpawnDisk()
	{
		// Находим все колышки в сцене
		GameObject[] pegs = GameObject.FindGameObjectsWithTag("Peg");

		if (pegs.Length == 0)
		{
			Debug.LogWarning("No pegs found in the scene!");
			return;
		}

		// Находим самый верхний ряд колышков
		float highestY = float.MinValue;
		foreach (GameObject peg in pegs)
		{
			if (peg.transform.position.y > highestY)
			{
				highestY = peg.transform.position.y;
			}
		}

		// Находим крайние колышки среди самых верхних
		float leftmostX = float.MaxValue;
		float rightmostX = float.MinValue;

		foreach (GameObject peg in pegs)
		{
			if (Mathf.Approximately(peg.transform.position.y, highestY))
			{
				float pegX = peg.transform.position.x;
				if (pegX < leftmostX)
				{
					leftmostX = pegX;
				}
				if (pegX > rightmostX)
				{
					rightmostX = pegX;
				}
			}
		}

		// Генерируем случайную позицию по X между крайними колышками
		float randomX = Random.Range(leftmostX, rightmostX);
		Vector3 spawnPosition = new Vector3(randomX, highestY + spawnHeight, 0); // Позиция спавна

		// Спавним диск
		Instantiate(diskPrefab, spawnPosition, Quaternion.identity);
	}
}
