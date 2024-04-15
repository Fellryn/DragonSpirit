using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to deselect all elements when using a mouse in Unity
	/// </summary>
	public class UINavigationDeselectInGame : MonoBehaviour
	{
		//Vector3 lastMousePosition;
        EventSystem eventSystem;

        [SerializeField]
        InputActionReference mouseMoved;
        [SerializeField]
        InputActionReference uiNavigate;
        [SerializeField]
        InputActionReference playerMove;
        [SerializeField]
        InputActionReference playerShoot;


        private void OnEnable()
        {
            eventSystem = EventSystem.current;
            mouseMoved.action.performed += MouseMoved;
            uiNavigate.action.performed += NavigateCheck;
            playerMove.action.performed += ClearSelection;
            playerShoot.action.performed += ClearSelection;
        }


        //private void Start()
        //{
        //    lastMousePosition = Input.mousePosition;
        //}

        private void OnDisable()
        {
            mouseMoved.action.performed -= MouseMoved;
            uiNavigate.action.performed -= NavigateCheck;
            playerMove.action.performed -= ClearSelection;
            playerShoot.action.performed -= ClearSelection;
        }


        //private void Update()
        //{
        //    if (Input.mousePosition != lastMousePosition)
        //    {
        //        eventSystem.SetSelectedGameObject(null);
        //    }

        //}


        private void MouseMoved(InputAction.CallbackContext obj)
        {
            eventSystem.SetSelectedGameObject(null);
        }


        private void NavigateCheck(InputAction.CallbackContext obj)
        {
            //Debug.Log(GameManager.Instance.State);
            //if (GameManager.Instance.State == GameManager.GameStates.Running)
            //{
            //    eventSystem.SetSelectedGameObject(null);
            //    return;
            //}

            if (eventSystem.currentSelectedGameObject != null) return;
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }


        private void ClearSelection(InputAction.CallbackContext obj)
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }
}