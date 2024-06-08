using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TNVirtualKeyboard : MonoBehaviour
{
	
	public static TNVirtualKeyboard instance;
	
	public string words = "";
	
	public GameObject vkCanvas;
	
	public TMP_InputField targetText;

	[SerializeField]
	InputActionReference cancelAction;

	[SerializeField]
	TMP_InputField playerOneNameText;
	[SerializeField]
	TMP_InputField playerTwoNameText;
	public bool isPlayerOneName = true;

	[SerializeField]
	GameObject firstSelectedObjectPlayerOne;

	[SerializeField]
	GameObject firstSelectedObjectPlayerTwo;

	[SerializeField]
	GameObject menuButtons;

	[SerializeField]
	GameObject[] toDisableOnKeyboard;


	// Start is called before the first frame update
	void Start()
    {
        instance = this;
		//HideVirtualKeyboard();
		cancelAction.action.performed += HideKeyboardCancel;
    }

    private void OnDestroy()
    {
		cancelAction.action.performed -= HideKeyboardCancel;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void KeyPress(string k){
		words += k;
		targetText.text = words;	
	}
	
	public void Del(){
		words = words.Remove(words.Length - 1, 1);
		targetText.text = words;	
	}
	
	public void ShowVirtualKeyboard(){
		vkCanvas.SetActive(true);

		targetText.text = words;
		playerOneNameText.interactable = false;
		playerTwoNameText.interactable = false;

		foreach (GameObject go in toDisableOnKeyboard)
		{
			go.SetActive(false);
		}
	}

	private void HideKeyboardCancel(InputAction.CallbackContext obj)
    {
		if (vkCanvas.activeSelf)
		{
			vkCanvas.SetActive(false);

			playerOneNameText.interactable = true;
			playerTwoNameText.interactable = true;

			if (isPlayerOneName)
			{
				EventSystem.current.firstSelectedGameObject = firstSelectedObjectPlayerOne;
			}
			else
			{
				EventSystem.current.firstSelectedGameObject = firstSelectedObjectPlayerTwo;
			}

			EventSystem.current.SetSelectedGameObject(null);
			menuButtons.SetActive(true);

			foreach (GameObject go in toDisableOnKeyboard)
			{
				go.SetActive(true);
			}
		}
	}
	
	public void HideVirtualKeyboard(){
		vkCanvas.SetActive(false);

		playerOneNameText.interactable = true;
		playerTwoNameText.interactable = true;

		if (isPlayerOneName)
        {
			playerOneNameText.text = targetText.text;
			EventSystem.current.firstSelectedGameObject = firstSelectedObjectPlayerOne;
		} else
        {
			playerTwoNameText.text = targetText.text;
			EventSystem.current.firstSelectedGameObject = firstSelectedObjectPlayerTwo;
		}

		foreach (GameObject go in toDisableOnKeyboard)
		{
			go.SetActive(true);
		}

		EventSystem.current.SetSelectedGameObject(null);
		menuButtons.SetActive(true);
	}
}
