using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
	[System.Serializable]
	private class BookData
	{
		public string TitleAndAuthor = null;
		public string ResourcesFolder = null;
		public int NumChapters = 0;
	}

	[SerializeField] private BookData[] _books = null;

	[Header("UI")]
	[SerializeField] private TextMeshProUGUI _textChapter = null;
	[SerializeField] private TextMeshProUGUI _textPage = null;
	[SerializeField] private TextMeshProUGUI _textBook = null;

	[Header("Input")]
	[SerializeField] private InputAction _inputNextPage = null;
	[SerializeField] private InputAction _inputPrevPage = null;
	[SerializeField] private InputAction _inputFirstPage = null;
	[SerializeField] private InputAction _inputLastPage = null;
	[SerializeField] private InputAction _inputNextChapter = null;
	[SerializeField] private InputAction _inputPrevChapter = null;

	private int _bookIndex = -1;
	private int _chapterIndex = -1;
	private int _pageIndex = -1;
	private TextAsset _currentChapter = null;

	private void Awake()
	{
		LoadBook(0);

		EnableAndSubscribe(_inputNextPage, OnNextPage);
		EnableAndSubscribe(_inputPrevPage, OnPrevPage);
		EnableAndSubscribe(_inputFirstPage, OnFirstPage);
		EnableAndSubscribe(_inputLastPage, OnLastPage);
		EnableAndSubscribe(_inputNextChapter, OnNextChapter);
		EnableAndSubscribe(_inputPrevChapter, OnPrevChapter);
	}

	private void LoadBook(int index)
	{
		if (index < 0 || _books == null || index >= _books.Length)
		{
			return;
		}
		_bookIndex = index;
		LoadChapter(0);
	}

	private void LoadChapter(int index)
	{
		if (index < 0 || index > _books[_bookIndex].NumChapters)
		{
			return;
		}
		_chapterIndex = index;
		_currentChapter = Resources.Load<TextAsset>(_books[_bookIndex].ResourcesFolder + "/" + _chapterIndex.ToString());
		_textChapter.text = _chapterIndex.ToString() + "/" + _books[_bookIndex].NumChapters.ToString();
		_textBook.text = _currentChapter.text;
		_textBook.ForceMeshUpdate();
		LoadPage(1);
	}

	private void LoadPage(int index)
	{
		_pageIndex = index;
		_textBook.pageToDisplay = _pageIndex;
		_textPage.text = _pageIndex.ToString() + "/" + _textBook.textInfo.pageCount.ToString();
	}

	private void EnableAndSubscribe(InputAction action, System.Action<InputAction.CallbackContext> callback)
	{
		action.Enable();
		action.performed -= callback;
		action.performed += callback;
	}

	private void OnNextPage(InputAction.CallbackContext context)
	{
		if (_pageIndex < _textBook.textInfo.pageCount)
		{
			LoadPage(_pageIndex + 1);
		}
	}

	private void OnPrevPage(InputAction.CallbackContext context)
	{
		if (_pageIndex > 1)
		{
			LoadPage(_pageIndex - 1);
		}
	}

	private void OnFirstPage(InputAction.CallbackContext context)
	{
		LoadPage(1);
	}

	private void OnLastPage(InputAction.CallbackContext context)
	{
		LoadPage(_textBook.textInfo.pageCount);
	}

	private void OnNextChapter(InputAction.CallbackContext context)
	{
		if (_chapterIndex < _books[_bookIndex].NumChapters)
		{
			LoadChapter(_chapterIndex + 1);
		}
	}

	private void OnPrevChapter(InputAction.CallbackContext context)
	{
		if (_chapterIndex > 0)
		{
			LoadChapter(_chapterIndex - 1);
		}
	}
}
