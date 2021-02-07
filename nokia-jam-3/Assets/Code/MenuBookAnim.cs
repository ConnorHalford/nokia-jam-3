using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuBookAnim : MonoBehaviour
{
	[Header("UI")]
	[SerializeField] private MenuBookSelection _menuSelection = null;

	[Header("Input")]
	[SerializeField] private InputActionAsset _allInputs = null;
	[SerializeField] private InputActionReference _inputConfirm = null;

	private void Awake()
	{
		_allInputs.Enable();
	}

	private void OnEnable()
	{
		_inputConfirm.action.performed += OnConfirm;
	}

	private void OnDisable()
	{
		_inputConfirm.action.performed -= OnConfirm;
	}

	private void OnConfirm(InputAction.CallbackContext context)
	{
		gameObject.SetActive(false);
		_menuSelection.ResetSelection();
	}
}
