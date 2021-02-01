using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways, RequireComponent(typeof(CanvasScaler))]
public class PixelPerfectCanvasScaler : MonoBehaviour
{
	private CanvasScaler _canvasScaler = null;
	private PixelPerfectCameraExecuteAlways _pixelPerfectCamera = null;

	private void Awake()
	{
		ApplyScale();
	}

	private void LateUpdate()
	{
		ApplyScale();
	}

	private void ApplyScale()
	{
		if (_canvasScaler == null)
		{
			_canvasScaler = GetComponent<CanvasScaler>();
		}
		if (_pixelPerfectCamera == null)
		{
			_pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCameraExecuteAlways>();
		}
		if (_canvasScaler != null && _pixelPerfectCamera != null)
		{
			try
			{
				_canvasScaler.scaleFactor = _pixelPerfectCamera.pixelRatio;
			}
			catch
			{ }
		}
	}
}
