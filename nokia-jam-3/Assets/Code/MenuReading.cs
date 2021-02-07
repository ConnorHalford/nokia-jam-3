using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuReading : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private MenuBookSelection _menuSelection = null;
	[SerializeField] private TextMeshProUGUI _textChapter = null;
	[SerializeField] private TextMeshProUGUI _textPage = null;
	[SerializeField] private TextMeshProUGUI _textBook = null;

	[Header("Input")]
	[SerializeField] private InputActionReference _inputBack = null;
	[SerializeField] private InputActionReference _inputNextPage = null;
	[SerializeField] private InputActionReference _inputPrevPage = null;
	[SerializeField] private InputActionReference _inputFirstPage = null;
	[SerializeField] private InputActionReference _inputLastPage = null;
	[SerializeField] private InputActionReference _inputNextChapter = null;
	[SerializeField] private InputActionReference _inputPrevChapter = null;

	private string _bookResourcesFolder = null;
	private int _numChapters = -1;
	private TextAsset _currentChapter = null;
	private int _chapterIndex = -1;
	private int _pageIndex = -1;

	public void LoadBook(string resourcesFolder, int numChapters)
	{
		gameObject.SetActive(true);
		_bookResourcesFolder = resourcesFolder;
		_numChapters = numChapters;
		LoadChapter(0);
	}

	private void OnEnable()
	{
		_inputBack.action.performed += OnBack;
		_inputNextPage.action.performed += OnNextPage;
		_inputPrevPage.action.performed += OnPrevPage;
		_inputFirstPage.action.performed += OnFirstPage;
		_inputLastPage.action.performed += OnLastPage;
		_inputNextChapter.action.performed += OnNextChapter;
		_inputPrevChapter.action.performed += OnPrevChapter;
	}

	private void OnDisable()
	{
		_inputBack.action.performed -= OnBack;
		_inputNextPage.action.performed -= OnNextPage;
		_inputPrevPage.action.performed -= OnPrevPage;
		_inputFirstPage.action.performed -= OnFirstPage;
		_inputLastPage.action.performed -= OnLastPage;
		_inputNextChapter.action.performed -= OnNextChapter;
		_inputPrevChapter.action.performed -= OnPrevChapter;
	}

	private void LoadChapter(int index)
	{
		_chapterIndex = index;
		_currentChapter = Resources.Load<TextAsset>(_bookResourcesFolder + "/" + _chapterIndex.ToString());
		_textChapter.text = _chapterIndex.ToString() + "/" + _numChapters.ToString();
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

	private void OnBack(InputAction.CallbackContext context)
	{
		gameObject.SetActive(false);
		_menuSelection.gameObject.SetActive(true);
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
		if (_chapterIndex < _numChapters)
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
