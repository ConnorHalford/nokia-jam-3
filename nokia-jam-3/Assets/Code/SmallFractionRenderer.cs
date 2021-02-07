using UnityEngine;
using UnityEngine.UI;

public class SmallFractionRenderer : MonoBehaviour
{
	[SerializeField] private SmallNumberRenderer _numberLeft = null;
	[SerializeField] private Image _slash = null;
	[SerializeField] private SmallNumberRenderer _numberRight = null;

	const int SLASH_WIDTH = 4;

	public void SetFraction(int left, int right, TextAlignment alignment)
	{
		Debug.Assert(left >= 0 && right >= 0);

		// Set sprites
		_numberLeft.SetNumber(left, alignment);
		_numberRight.SetNumber(right, alignment);

		// Position
		const int DIGIT_WIDTH = SmallNumberRenderer.DIGIT_WIDTH;
		int leftWidth = DIGIT_WIDTH * left.ToString().Length;
		int rightWidth = DIGIT_WIDTH * right.ToString().Length;
		if (alignment == TextAlignment.Left)
		{
			_numberLeft.transform.localPosition = Vector3.zero;
			_slash.transform.localPosition = leftWidth * Vector3.right;
			_numberRight.transform.localPosition = (leftWidth + SLASH_WIDTH) * Vector3.right;
		}
		else if (alignment == TextAlignment.Center)
		{
			Debug.LogError("Centre alignment not implemented");
		}
		else if (alignment == TextAlignment.Right)
		{
			_numberRight.transform.localPosition = Vector3.zero;
			_slash.transform.localPosition = (rightWidth + SLASH_WIDTH) * Vector3.left;
			_numberLeft.transform.localPosition = _slash.transform.localPosition;
		}
	}
}
