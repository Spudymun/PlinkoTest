using UnityEngine;

public class CameraScaler : MonoBehaviour
{
	public float minWidth = 1136f;  // ����������� ������ ������
	public float maxWidth = 2732f;  // ������������ ������ ������
	public float minHeight = 640f;  // ����������� ������ ������
	public float maxHeight = 2048f; // ������������ ������ ������
	public float minOrthographicSize = 4f;  // ����������� ������ ������
	public float maxOrthographicSize = 25f; // ������������ ������ ������
	private Vector2 lastScreenSize;  // ������ ��������� ���������� ������

	void Start()
	{
		lastScreenSize = new Vector2(Screen.width, Screen.height);
		AdjustCamera();
	}

	void Update()
	{
		// ���������, ���������� �� ���������� ������
		if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
		{
			AdjustCamera();
			lastScreenSize = new Vector2(Screen.width, Screen.height); // ��������� ��������� ����������
		}
	}

	void AdjustCamera()
	{
		// �������� ������� ���������� ������
		float currentWidth = Screen.width;
		float currentHeight = Screen.height;

		// ����������, ������������ ���������� ��� ��������������
		bool isPortrait = currentHeight > currentWidth;

		// ������������ ������� �� ������ ����������� � ������������ ����������
		float widthFactor = Mathf.InverseLerp(minWidth, maxWidth, isPortrait ? currentHeight : currentWidth);
		float heightFactor = Mathf.InverseLerp(minHeight, maxHeight, isPortrait ? currentWidth : currentHeight);

		// ������������ ������� �������� ����� �������������� �� ������ � ������
		float averageFactor = (widthFactor + heightFactor) / 2f;

		// ������������ �������� ��������������� ������ �� ������ �������� ������������
		float orthographicSize = Mathf.Lerp(minOrthographicSize, maxOrthographicSize, averageFactor);

		// ������������ ��������������� ������ � ���������� ��������
		orthographicSize = Mathf.Clamp(orthographicSize, minOrthographicSize, maxOrthographicSize);

		// ��������� ����� ������ ������
		Camera.main.orthographicSize = orthographicSize;

		// ������� �������� ���������������� ������� � ������� ��� �������
		Debug.Log($"Camera Orthographic Size: {Camera.main.orthographicSize}");
	}
}
