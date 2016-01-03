using UnityEngine;
using System.Collections;

public class CursorUtils : MonoBehaviour
{
	private static Vector2 _lastMousePos;
	private static Vector2 _currentMousePos;

	public static Vector2 MouseDeltaPos { get; private set; }

	void Update()
	{
		_lastMousePos = _currentMousePos;
		_currentMousePos = Input.mousePosition;
		MouseDeltaPos = _currentMousePos - _lastMousePos;
	}
}
