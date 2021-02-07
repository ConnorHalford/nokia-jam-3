using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuReading : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private MenuBookSelection _menuSelection = null;
	[SerializeField] private SmallFractionRenderer _fractionChapter = null;
	[SerializeField] private SmallFractionRenderer _fractionPage = null;
	[SerializeField] private TextMeshProUGUI _textBook = null;

	[Header("Input")]
	[SerializeField] private float _holdInputDelay = 0.5f;
	[SerializeField] private float _holdInputPeriod = 0.2f;
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

	private Coroutine _holdInputCoroutine = null;
	private System.Action _heldAction = null;

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
		_inputNextPage.action.canceled += OnNextPage;
		_inputPrevPage.action.performed += OnPrevPage;
		_inputPrevPage.action.canceled += OnPrevPage;
		_inputFirstPage.action.performed += OnFirstPage;
		_inputLastPage.action.performed += OnLastPage;
		_inputNextChapter.action.performed += OnNextChapter;
		_inputNextChapter.action.canceled += OnNextChapter;
		_inputPrevChapter.action.performed += OnPrevChapter;
		_inputPrevChapter.action.canceled += OnPrevChapter;
	}

	private void OnDisable()
	{
		StopCoroutine(ref _holdInputCoroutine);
		_inputBack.action.performed -= OnBack;
		_inputNextPage.action.performed -= OnNextPage;
		_inputNextPage.action.canceled -= OnNextPage;
		_inputPrevPage.action.performed -= OnPrevPage;
		_inputPrevPage.action.canceled -= OnPrevPage;
		_inputFirstPage.action.performed -= OnFirstPage;
		_inputLastPage.action.performed -= OnLastPage;
		_inputNextChapter.action.performed -= OnNextChapter;
		_inputNextChapter.action.canceled -= OnNextChapter;
		_inputPrevChapter.action.performed -= OnPrevChapter;
		_inputPrevChapter.action.canceled -= OnPrevChapter;
	}

	private void LoadChapter(int index)
	{
		_chapterIndex = index;
		_currentChapter = Resources.Load<TextAsset>(_bookResourcesFolder + "/" + _chapterIndex.ToString());
		_fractionChapter.SetFraction(_chapterIndex, _numChapters, TextAlignment.Left);
		_textBook.text = _currentChapter.text;
		_textBook.ForceMeshUpdate();
		LoadPage(1);
	}

	private void LoadNextChapter()
	{
		LoadChapter((_chapterIndex + 1) % (_numChapters + 1));
	}

	private void LoadPrevChapter()
	{
		LoadChapter((_numChapters + _chapterIndex) % (_numChapters + 1));
	}

	private void LoadPage(int index)
	{
		_pageIndex = index;
		_textBook.pageToDisplay = _pageIndex;
		_fractionPage.SetFraction(_pageIndex, _textBook.textInfo.pageCount, TextAlignment.Right);
	}

	private void LoadNextPage()
	{
		LoadPage(1 + (_pageIndex % _textBook.textInfo.pageCount));
	}

	private void LoadPrevPage()
	{
		int numPages = _textBook.textInfo.pageCount;
		LoadPage(1 + ((numPages + _pageIndex - 2) % numPages));
	}

	private void OnBack(InputAction.CallbackContext context)
	{
		gameObject.SetActive(false);
		_menuSelection.gameObject.SetActive(true);
	}

	private void OnFirstPage(InputAction.CallbackContext context)
	{
		LoadPage(1);
	}

	private void OnLastPage(InputAction.CallbackContext context)
	{
		LoadPage(_textBook.textInfo.pageCount);
	}

	private void OnNextPage(InputAction.CallbackContext context)
	{
		HandleHeldInput(context, LoadNextPage);
	}

	private void OnPrevPage(InputAction.CallbackContext context)
	{
		HandleHeldInput(context, LoadPrevPage);
	}

	private void OnNextChapter(InputAction.CallbackContext context)
	{
		HandleHeldInput(context, LoadNextChapter);
	}

	private void OnPrevChapter(InputAction.CallbackContext context)
	{
		HandleHeldInput(context, LoadPrevChapter);
	}

	private void HandleHeldInput(InputAction.CallbackContext context, System.Action action)
	{
		if (context.performed)
		{
			StopCoroutine(ref _holdInputCoroutine);
			_heldAction = action;
			_holdInputCoroutine = StartCoroutine(HoldInputCoroutine());
		}
		else if (context.canceled && _heldAction == action)
		{
			StopCoroutine(ref _holdInputCoroutine);
		}
	}

	private IEnumerator HoldInputCoroutine()
	{
		_heldAction();
		yield return new WaitForSeconds(_holdInputDelay);

		WaitForSeconds period = new WaitForSeconds(_holdInputPeriod);
		while (true)
		{
			_heldAction();
			yield return period;
		}
	}

	private void StopCoroutine(ref Coroutine coroutine)
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}
	}
}
