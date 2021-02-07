using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuBookSelection : MonoBehaviour
{
	[System.Serializable]
	private class BookData
	{
		public string TitleAndAuthor = null;
		public string ResourcesFolder = null;
		public int NumChapters = 0;
	}

	[Header("UI")]
	[SerializeField] private MenuBookAnim _menuBookAnim = null;
	[SerializeField] private MenuReading _menuReading = null;
	[SerializeField] private Image _highlight = null;
	[SerializeField] private TextMeshProUGUI[] _bookText = null;
	[SerializeField] private SmallNumberRenderer _numberRenderer = null;
	[SerializeField] private Color _colorLight = new Color32(199, 240, 216, 255);
	[SerializeField] private Color _colorDark = new Color32(67, 82, 61, 255);

	[Header("Scrolling")]
	[SerializeField] private Image _scrollbar = null;
	[SerializeField] private Vector2 _scrollbarRange = Vector2.zero;
	[SerializeField] private float _horizontalVisibleWidth = 78.0f;
	[SerializeField] private float _horizontalDelay = 0.3f;
	[SerializeField] private float _horizontalSpeed = 5.0f;

	[Header("Input")]
	[SerializeField] private InputActionReference _inputConfirm = null;
	[SerializeField] private InputActionReference _inputBack = null;
	[SerializeField] private InputActionReference _inputNextPage = null;
	[SerializeField] private InputActionReference _inputPrevPage = null;
	[SerializeField] private InputActionReference _inputFirstPage = null;
	[SerializeField] private InputActionReference _inputLastPage = null;

	[Header("Data")]
	[SerializeField] private BookData[] _bookData = null;

	private int _topmostBookIndex = -1;
	private int _highlightIndex = -1;
	private Coroutine _horizontalScroll = null;

	public void ResetSelection()
	{
		gameObject.SetActive(true);
		ScrollTo(0);
		SetHighlight(0);
	}

	private void OnEnable()
	{
		_inputConfirm.action.performed += OnConfirm;
		_inputBack.action.performed += OnBack;
		_inputNextPage.action.performed += OnNextPage;
		_inputPrevPage.action.performed += OnPrevPage;
		_inputFirstPage.action.performed += OnFirstPage;
		_inputLastPage.action.performed += OnLastPage;
		UpdateSelection();
	}

	private void OnDisable()
	{
		_inputConfirm.action.performed -= OnConfirm;
		_inputBack.action.performed -= OnBack;
		_inputNextPage.action.performed -= OnNextPage;
		_inputPrevPage.action.performed -= OnPrevPage;
		_inputFirstPage.action.performed -= OnFirstPage;
		_inputLastPage.action.performed -= OnLastPage;
	}

	private void ScrollTo(int index)
	{
		if (_topmostBookIndex != index)
		{
			int numBooks = _bookData.Length;
			int numTexts = _bookText.Length;
			_topmostBookIndex = Mathf.Clamp(index, 0, numBooks);
			for (int i = 0; i < numTexts; ++i)
			{
				_bookText[i].text = _bookData[_topmostBookIndex + i].TitleAndAuthor;
			}
			UpdateSelection();
		}
	}

	private void SetHighlight(int index)
	{
		if (_highlightIndex != index)
		{
			int numTexts = _bookText.Length;
			_highlightIndex = Mathf.Clamp(index, 0, numTexts - 1);

			float y = _bookText[_highlightIndex].transform.position.y;
			_highlight.transform.position = new Vector3(_highlight.transform.position.x, y, _highlight.transform.position.z);

			for (int i = 0; i < numTexts; ++i)
			{
				_bookText[i].color = (i == _highlightIndex) ? _colorLight : _colorDark;
			}
			UpdateSelection();
		}
	}

	private void UpdateSelection()
	{
		if (_topmostBookIndex == -1 || _highlightIndex == -1)
		{
			return;
		}

		// Number text
		int number = 1 + _topmostBookIndex + _highlightIndex;
		_numberRenderer.SetNumber(number, TextAlignment.Right);

		// Scrollbar
		Vector3 pos = _scrollbar.transform.localPosition;
		float percent = (float)(number - 1) / (_bookData.Length - 1);
		pos.y = Mathf.Floor(Mathf.Lerp(_scrollbarRange.x, _scrollbarRange.y, percent));
		_scrollbar.transform.localPosition = pos;

		// Horizontal scroll
		int numTexts = _bookText.Length;
		for (int i = 0; i < numTexts; ++i)
		{
			pos = _bookText[i].transform.localPosition;
			pos.x = -42.0f;
			_bookText[i].transform.localPosition = pos;
		}
		if (_horizontalScroll != null)
		{
			StopCoroutine(_horizontalScroll);
		}
		_horizontalScroll = StartCoroutine(HorizontalScroll());
	}

	private IEnumerator HorizontalScroll()
	{
		TextMeshProUGUI text = _bookText[_highlightIndex];
		float startX = text.transform.localPosition.x;
		float endX = startX - text.preferredWidth + _horizontalVisibleWidth;
		WaitForSeconds delay = new WaitForSeconds(_horizontalDelay);
		while (true)
		{
			// Wait before scrolling
			yield return delay;

			// Scroll horizontally
			float x = text.transform.localPosition.x;
			while (x > endX)
			{
				x = Mathf.Max(endX, x - _horizontalSpeed * Time.deltaTime);
				text.transform.localPosition = new Vector3(x, text.transform.localPosition.y, text.transform.localPosition.z);
				yield return null;
			}

			// Wait before returning
			yield return delay;
			text.transform.localPosition = new Vector3(startX, text.transform.localPosition.y, text.transform.localPosition.z);
		}
	}

	private void OnConfirm(InputAction.CallbackContext context)
	{
		gameObject.SetActive(false);
		BookData data = _bookData[_topmostBookIndex + _highlightIndex];
		_menuReading.LoadBook(data.ResourcesFolder, data.NumChapters);
	}

	private void OnBack(InputAction.CallbackContext context)
	{
		gameObject.SetActive(false);
		_menuBookAnim.gameObject.SetActive(true);
	}

	private void OnNextPage(InputAction.CallbackContext context)
	{
		int numBooks = _bookData.Length;
		int numTexts = _bookText.Length;
		if (_highlightIndex < numTexts - 1)
		{
			SetHighlight(_highlightIndex + 1);
		}
		else
		{
			if (_topmostBookIndex < numBooks - numTexts)
			{
				ScrollTo(_topmostBookIndex + 1);
			}
			else
			{
				ScrollTo(0);
				SetHighlight(0);
			}
		}
	}

	private void OnPrevPage(InputAction.CallbackContext context)
	{
		int numBooks = _bookData.Length;
		int numTexts = _bookText.Length;
		if (_highlightIndex > 0)
		{
			SetHighlight(_highlightIndex - 1);
		}
		else
		{
			if (_topmostBookIndex > 0)
			{
				ScrollTo(_topmostBookIndex - 1);
			}
			else
			{
				ScrollTo(numBooks - numTexts);
				SetHighlight(numTexts - 1);
			}
		}
	}

	private void OnFirstPage(InputAction.CallbackContext context)
	{
		ScrollTo(0);
		SetHighlight(0);
	}

	private void OnLastPage(InputAction.CallbackContext context)
	{
		int numBooks = _bookData.Length;
		int numTexts = _bookText.Length;
		ScrollTo(numBooks - numTexts);
		SetHighlight(numTexts - 1);
	}
}
