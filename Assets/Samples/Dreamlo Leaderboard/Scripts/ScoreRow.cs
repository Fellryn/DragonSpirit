using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ScoreRow : FastBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    //[SerializeField] private TextMeshProUGUI nameLabelDropShadow;
    [SerializeField] private TextMeshProUGUI scoreLabel;
    //[SerializeField] private TextMeshProUGUI scoreLabelDropShadow;
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private Button removeButton;

    private int scoreIndex = -1;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        nameLabel = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        scoreLabel = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        timeLabel = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    public void SetIndex(int index)
    {
        scoreIndex = index;
    }

    public void Show(string name, string score, string time)
    {
        nameLabel.text = name;
        //nameLabelDropShadow.text = name;
        scoreLabel.text = score;
        //scoreLabelDropShadow.text = score;
        timeLabel.text = score;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        //nameLabel.text = "";
        //nameLabelDropShadow.text = "";
        //scoreLabel.text = "";
        //scoreLabelDropShadow.text = "";
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }

    public void HideFully()
    {
        canvasGroup.gameObject.SetActive(false);
    }

    public void RemoveButton()
    {
        //RemoveScore(scoreIndex);
    }

    public void SelectButton()
    {
        removeButton.Select();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Hide();
        HideFully();
    }
}
