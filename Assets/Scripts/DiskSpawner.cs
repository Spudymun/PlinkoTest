using UnityEngine;

public class DiskSpawner : MonoBehaviour
{
	public GameObject diskPrefab; // ������ �����
	public float spawnHeight = 3.5f; // ������ ������ ��� ���������
	private float lastSpawnTime; // ����� ���������� ������
	public float spawnInterval = 1f; // �������� ����� �������� � ��������

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastSpawnTime + spawnInterval) // �������� ������� ������ ������ � ���������
		{
			SpawnDisk();
			lastSpawnTime = Time.time; // ��������� ����� ���������� ������
		}
	}

	void SpawnDisk()
	{
		// ������� ��� ������� � �����
		GameObject[] pegs = GameObject.FindGameObjectsWithTag("Peg");

		if (pegs.Length == 0)
		{
			Debug.LogWarning("No pegs found in the scene!");
			return;
		}

		// ������� ����� ������� ��� ��������
		float highestY = float.MinValue;
		foreach (GameObject peg in pegs)
		{
			if (peg.transform.position.y > highestY)
			{
				highestY = peg.transform.position.y;
			}
		}

		// ������� ������� ������� ����� ����� �������
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

		// ���������� ��������� ������� �� X ����� �������� ���������
		float randomX = Random.Range(leftmostX, rightmostX);
		Vector3 spawnPosition = new Vector3(randomX, highestY + spawnHeight, 0); // ������� ������

		// ������� ����
		Instantiate(diskPrefab, spawnPosition, Quaternion.identity);
	}
}
