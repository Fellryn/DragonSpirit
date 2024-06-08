using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using KurtSingle;
using System.Threading.Tasks;
using UnityEngine.InputSystem;


public class vkEnabler : MonoBehaviour
{
    [SerializeField]
    HighScoreDisplay highScoreDisplay;

    [SerializeField]
    InputActionReference submitAction;

    [SerializeField]
    InputActionReference navigateAction;

    [SerializeField]
    bool isPlayerOneInput = true;

    [SerializeField]
    GameObject hKey;

    [SerializeField]
    EventSystem eventSystem;

    [SerializeField]
    GameObject menuButtons;

    [SerializeField]
    TextMeshProUGUI playerText;

    [SerializeField]
    GameObject playerOneInputField;

    [SerializeField]
    GameObject playerTwoInputField;

    static bool hasAlreadySubscribed = false;
    private bool pressedTwice = false;



    // Start is called before the first frame update
    void Start()
    {
        //ShowVirtualKeyboard();

        if (!hasAlreadySubscribed)
        {
            hasAlreadySubscribed = true;
            submitAction.action.performed += OpenKeyboardFromTextfield;
            navigateAction.action.performed += MoveFromTextfield;
            Debug.Log("subbed twice");
        }
    }

    private void OnDestroy()
    {
        submitAction.action.performed -= OpenKeyboardFromTextfield;
        navigateAction.action.performed -= MoveFromTextfield;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OpenKeyboardFromTextfield(InputAction.CallbackContext obj)
    {
        if (highScoreDisplay.isUsingGamepad && (EventSystem.current.currentSelectedGameObject == playerOneInputField))
        {
            ShowVirtualKeyboard(isPlayerOneInput);
        } else if (highScoreDisplay.isUsingGamepad && EventSystem.current.currentSelectedGameObject == playerTwoInputField)
        {
            ShowVirtualKeyboard(isPlayerOneInput);
        }
    }

    private void MoveFromTextfield(InputAction.CallbackContext obj)
    {
        if (highScoreDisplay.isUsingGamepad && (EventSystem.current.currentSelectedGameObject == playerOneInputField || EventSystem.current.currentSelectedGameObject == playerTwoInputField))
        {
            if (pressedTwice)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
                pressedTwice = false;
            } else
            {
                pressedTwice = true; 
            }

        } else if ((highScoreDisplay.isUsingGamepad && EventSystem.current.currentSelectedGameObject == playerTwoInputField))
        {
            if (pressedTwice)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
                pressedTwice = false;
            } else
            {
                pressedTwice = true;
            }
        }
    }

    public void OpenTextField()
    {
        if (highScoreDisplay.isUsingGamepad && (EventSystem.current.currentSelectedGameObject == playerOneInputField))
        {
            ShowVirtualKeyboard(isPlayerOneInput);
        }
        else if (highScoreDisplay.isUsingGamepad && EventSystem.current.currentSelectedGameObject == playerTwoInputField)
        {
            ShowVirtualKeyboard(isPlayerOneInput);
        }
    }
	

	public async void ShowVirtualKeyboard(bool isPlayerOne){
        await Task.Delay(5);



        TNVirtualKeyboard.instance.isPlayerOneName = isPlayerOne;

        if (isPlayerOne)
        {
            playerText.text = "Player One Name:";
        } else
        {
            playerText.text = "Player Two Name:";
        }

        TNVirtualKeyboard.instance.words = "";
		TNVirtualKeyboard.instance.ShowVirtualKeyboard();
        menuButtons.SetActive(false);
        eventSystem.firstSelectedGameObject = hKey;
        eventSystem.SetSelectedGameObject(null);
        //eventSystem.SetSelectedGameObject(hKey);

		//TNVirtualKeyboard.instance.targetText = gameObject.GetComponent<TMP_InputField>();
	}
}
