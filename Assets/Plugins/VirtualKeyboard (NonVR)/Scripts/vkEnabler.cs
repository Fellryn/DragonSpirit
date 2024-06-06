using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Threading.Tasks;


public class vkEnabler : MonoBehaviour
{
    [SerializeField]
    GameObject hKey;
    [SerializeField]
    EventSystem eventSystem;

    [SerializeField]
    GameObject menuButtons;

 
    // Start is called before the first frame update
    void Start()
    {
        //ShowVirtualKeyboard();

    }

    private void OnDestroy()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
	

	public async void ShowVirtualKeyboard(){
        await Task.Delay(5);

		TNVirtualKeyboard.instance.ShowVirtualKeyboard();
        menuButtons.SetActive(false);
        eventSystem.firstSelectedGameObject = hKey;
        eventSystem.SetSelectedGameObject(null);
        //eventSystem.SetSelectedGameObject(hKey);

		TNVirtualKeyboard.instance.targetText = gameObject.GetComponent<TMP_InputField>();
	}
}
