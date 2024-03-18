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
	public class UINavigationDeselect : MonoBehaviour
	{
		//Vector3 lastMousePosition;
        EventSystem eventSystem;

        [SerializeField]
        InputActionReference mouseMoved;
        [SerializeField]
        InputActionReference uiNavigate;

        private void OnEnable()
        {
            eventSystem = EventSystem.current;
            mouseMoved.action.performed += MouseMoved;
            uiNavigate.action.performed += NavigateCheck;
        }

        //private void Start()
        //{
        //    lastMousePosition = Input.mousePosition;
        //}

        private void OnDisable()
        {
            mouseMoved.action.performed -= MouseMoved;
            uiNavigate.action.performed -= NavigateCheck;
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
            if (eventSystem.currentSelectedGameObject != null) return;
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }

    }
}