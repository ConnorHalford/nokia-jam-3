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
	[SerializeField] private Image _scrollbar = null;
	[SerializeField] private Vector2 _scrollbarRange = Vector2.zero;

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
		int numBooks = _bookData.Length;
		int numTexts = _bookText.Length;
		_topmostBookIndex = Mathf.Clamp(index, 0, numBooks);
		for (int i = 0; i < numTexts; ++i)
		{
			_bookText[i].text = _bookData[_topmostBookIndex + i].TitleAndAuthor;
		}
		UpdateSelection();
	}

	private void SetHighlight(int index)
	{
		if (index != _highlightIndex)
		{
			int numTexts = _bookText.Length;
			_highlightIndex = Mathf.Clamp(index, 0, numTexts - 1);

			_highlight.transform.position = _bookText[_highlightIndex].transform.position;

			for (int i = 0; i < numTexts; ++i)
			{
				_bookText[i].color = (i == _highlightIndex) ? _colorLight : _colorDark;
			}
		}
		UpdateSelection();
	}

	private void UpdateSelection()
	{
		// Number text
		int number = 1 + _topmostBookIndex + _highlightIndex;
		_numberRenderer.SetNumber(number, TextAlignment.Right);

		// Scrollbar
		Vector3 pos = _scrollbar.transform.localPosition;
		float percent = (float)(number - 1) / (_bookData.Length - 1);
		pos.y = Mathf.Floor(Mathf.Lerp(_scrollbarRange.x, _scrollbarRange.y, percent));
		_scrollbar.transform.localPosition = pos;
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
