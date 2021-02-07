using UnityEngine;
using UnityEngine.UI;

public class SmallNumberRenderer : MonoBehaviour
{
	[SerializeField] private Image _digitImage = null;
	[SerializeField] private Sprite[] _digitSprites = null;

	private Image[] _digits = null;

	public const int DIGIT_WIDTH = 6;

	public void SetNumber(int number, TextAlignment alignment)
	{
		Debug.Assert(number >= 0);

		// Ensure we have the correct number of digit images
		if (_digits == null)
		{
			_digits = new Image[1] { _digitImage };
		}
		int numDigits = _digits.Length;
		int targetDigits = number.ToString().Length;
		if (numDigits < targetDigits)
		{
			// Make new digits
			System.Array.Resize<Image>(ref _digits, targetDigits);
			for (int i = numDigits; i < targetDigits; ++i)
			{
				_digits[i] = Instantiate<Image>(_digitImage, transform);
			}
		}
		else if (numDigits > targetDigits)
		{
			// Disable extra digits
			for (int i = targetDigits; i < numDigits; ++i)
			{
				_digits[i].gameObject.SetActive(false);
			}
		}

		// Assign digits
		Vector3 deltaPos = DIGIT_WIDTH * Vector3.left;
		Vector3 pos = Vector3.zero;
		if (alignment == TextAlignment.Left)
		{
			pos -= (targetDigits - 1) * deltaPos;
		}
		else if (alignment == TextAlignment.Center)
		{
			Debug.LogError("Centre alignment not implemented");
		}
		else if (alignment == TextAlignment.Right)
		{
			pos += deltaPos;
		}
		for (int i = 0; i < targetDigits; ++i)
		{
			// Enable and assign sprite
			_digits[i].gameObject.SetActive(true);
			_digits[i].sprite = _digitSprites[number % 10];
			number /= 10;

			// Position
			_digits[i].transform.localPosition = pos;
			pos += deltaPos;
		}
	}
}
