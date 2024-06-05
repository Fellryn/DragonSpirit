using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This allows the user to download the scores from a unique Dreamlo.com link.
/// Then it cycles through the scores and shows the ones nearest to the current player score.
/// Effectively, a near 5 (two above, two below, if they exist).
/// 
/// Original Author: John Abel Sem1 2023.
/// Modified by: Mark Hoey.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DisplayScorePosition : FastBehaviour, IPointerClickHandler
{
    [SerializeField] private HighScores highScores;
    [Range(15, 3_600)]
    [SerializeField] private int refreshInterval = 60;
    [SerializeField] private UnityEvent OnAllScoresUpdate;

    private CanvasGroup canvasGroup;
    private Vector3 canvasOpenLocation;
    [SerializeField] private Vector3 canvasClosedLocationOffset;

    [SerializeField] int currentScoreTotal = 5;
    private int lastKnownScoreTotal = 0;

    [SerializeField] private float scoreChangeHeight = 10f;
    [SerializeField] private float scoreChangeTime = 1f;
    private List<TextMeshProUGUI> scoreTextfields;
    private List<TextMeshProUGUI> usernameTextfields;
    private List<TextMeshProUGUI> timeTextfields;
    private List<GameObject> scoreGameObjects;
    private List<Vector3> originalPositions = new List<Vector3>();

    private bool showNames = true;
    private bool showTimes = true;

    private int currentRank = -1;

    private void Awake()
    {
        // Vertical Layout group caused issues with DoTween
        if (GetComponent<VerticalLayoutGroup>() != null)
        {
            Destroy(GetComponent<VerticalLayoutGroup>());
        }

        //Use some helper extension methods to grab the objects/components automatically
        scoreGameObjects = gameObject.GetChildrenList();
        scoreTextfields = gameObject.GetGrandChildrenList<TextMeshProUGUI>(0);
        usernameTextfields = gameObject.GetGrandChildrenList<TextMeshProUGUI>(1);
        //timeTextfields = gameObject.GetGrandChildrenList<TextMeshProUGUI>(2);

        //Store the original positions of the objects to stop DoTween "walking"
        foreach (var item in scoreGameObjects)
        {
            originalPositions.Add(item.GetComponent<RectTransform>().anchoredPosition3D);
        }
    }
    private void OnEnable()
    {
        OnAllScoresUpdate.AddListener(DisplayScoreDisplay);
    }

    private void OnDisable()
    {
        OnAllScoresUpdate.RemoveListener(DisplayScoreDisplay);
    }

    private void Start()
    {
        if (highScores  == null)
        {
            Debug.LogError($"Need to link to Highscores component. {this.name} on {this.gameObject.name} has been disabled.");
            this.enabled = false;
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvasOpenLocation = canvasGroup.GetComponent<RectTransform>().anchoredPosition3D;
        StartCoroutine("UpdateAllScores");
        DisplayScoreDisplay();
    }

    public void TestAddScore(int amount)
    {
        currentScoreTotal += amount;
    }

    private void Update()
    {
        if (currentScoreTotal <= lastKnownScoreTotal)
        {
            return;
        }

        lastKnownScoreTotal = currentScoreTotal;
        OnAllScoresUpdate?.Invoke();
    }

    void DisplayScoreDisplay()
    {
        UpdateFiveNearScoresDisplay();
        canvasGroup.DOFade(.625f, 5f);
    }

    IEnumerator UpdateAllScores()
    {
        //Clamp refresh to not be too excessive and max out at an hour
        refreshInterval = Mathf.Clamp(refreshInterval, 15, 3_600);

        while (true)
        {
            highScores.GetAllScores();
#if UNITY_EDITOR
            print("Updated Data from online: " + Time.time);
#endif
            yield return new WaitForSeconds(refreshInterval);
        }
    }

    public void ToggleNamesOnOff()
    {
        showNames= !showNames;
        ToggleTextfieldVisibility(_showNames: showNames, _showTimes: showTimes);
    }

    public void ToggleTimesOnOff()
    {
        showTimes= !showTimes;
        ToggleTextfieldVisibility(_showNames: showNames, _showTimes: showTimes);
    }

    private void ToggleTextfieldVisibility(bool _showNames = true, bool _showTimes = false)
    {
        foreach (var item in usernameTextfields)
        {
            item.transform.gameObject.SetActive(_showNames);
        }

        //foreach (var item in timeTextfields)
        //{
        //    item.transform.gameObject.SetActive(_showTimes);
        //}
    }
    private void UpdateFiveNearScoresDisplay()
    {
#if UNITY_EDITOR
        print("Updated score display: " + Time.time);
#endif
        foreach (var item in scoreGameObjects)
        {
            item.SetActive(false);
        }

        int currentScore = currentScoreTotal;

        if (currentScore <= 0) { return; }

        if (highScores.allScoresList == null) { return; }

        int scoresFound = highScores.allScoresList.Length;

        if (scoresFound <= 0) { return; }

        for (int i = 0; i < scoresFound; i++)
        {
            if (currentScore > highScores.allScoresList[i].score)
            {
                scoresFound = i;
                break;
            }
        }

       

        void SetActiveAndSetText(GameObject gameobjectToEnable, TextMeshProUGUI textfieldToUpdate, int displayIndex, int scoreListIndex, TextMeshProUGUI usernameTextfieldToUpdate = null)
        {
            gameobjectToEnable.SetActive(true);
            textfieldToUpdate.text = $"{displayIndex}. {highScores.allScoresList[scoreListIndex].score.ToString("N0")}";

            if (usernameTextfieldToUpdate != null)
            {
                usernameTextfieldToUpdate.text = $"{highScores.allScoresList[scoreListIndex].username}";
            }

            //if (timeTextfieldToUpdate != null)
            //{
            //    timeTextfieldToUpdate.text = $"{highScores.allScoresList[scoreListIndex].time.GetTimeSpanString()}";
            //}

        }

        if (scoresFound - 2 >= 0)
        {
            SetActiveAndSetText(scoreGameObjects[0], scoreTextfields[0], scoresFound - 1, scoresFound - 2, usernameTextfields[0]);
        }

        if (scoresFound - 1 >= 0)
        {
            SetActiveAndSetText(scoreGameObjects[1], scoreTextfields[1], scoresFound, scoresFound - 1, usernameTextfields[1]);
        }


        scoreGameObjects[2].SetActive(true);
        scoreTextfields[2].text = $"{(scoresFound + 1)}. {currentScore.ToString("N0")} (You)";
        usernameTextfields[2].text = $"Current Player";
        
        //timeTextfields[2].text = Time.timeSinceLevelLoad.GetTimeSpanString();

        if (scoresFound < highScores.allScoresList.Length)
        {
            SetActiveAndSetText(scoreGameObjects[3], scoreTextfields[3], scoresFound + 2, scoresFound, usernameTextfields[3]);
        }

        if (scoresFound + 1 < highScores.allScoresList.Length)
        {
            SetActiveAndSetText(scoreGameObjects[4], scoreTextfields[4], scoresFound + 3, scoresFound + 1, usernameTextfields[4]);
        }

        if (scoresFound != currentRank)
        {
            if (currentRank > 0)
            {
                MoveScore(scoresFound < currentRank);
            }
            currentRank = scoresFound;
        }
    }

    public void Bounce(GameObject _object, Vector3 originalPosition, bool _up = true)
    {
        float _scoreChangeHeight = (_up) ? scoreChangeHeight : -scoreChangeHeight;
        if (_object.activeSelf)
        {
            _object.transform.DOKill();
            _object.transform.position = originalPosition;
            _object.transform.DOJump(_object.transform.position, _scoreChangeHeight, 1, scoreChangeTime * 0.5f, false);
        }
    }

    public void Bounce(RectTransform _object, Vector3 originalPosition, bool _up = true)
    {
        float _scoreChangeHeight = (_up) ? scoreChangeHeight : -scoreChangeHeight;
        if (_object.gameObject.activeSelf)
        {
            _object.DOKill();
            _object.anchoredPosition3D = originalPosition;
            _object.DOJumpAnchorPos(_object.anchoredPosition3D, _scoreChangeHeight, 1, scoreChangeTime * 0.5f, false);
        }
    }

    void MoveScore(bool _up = true)
    {
        for (int i = 0; i < scoreGameObjects.Count; i++)
        {
            Bounce(scoreGameObjects[i].GetComponent<RectTransform>(), originalPositions[i], !_up);
        }
    }


    /// <summary>
    /// TODO: Omit or make as HelperMethods
    /// </summary>
    private Vector3 savedPosition;

    // Method to store the RectTransform position data
    public void SavePosition(RectTransform rectTransform)
    {
        savedPosition = rectTransform.anchoredPosition3D;
    }

    // Method to add back the stored position data to the RectTransform
    public void RestorePosition(RectTransform rectTransform)
    {
        rectTransform.anchoredPosition3D = savedPosition;
    }



    /// <summary>
    /// TODO: Move this and related fields to separate script - then can be used for any toggle group
    /// </summary>
    bool isClosed = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        isClosed = !isClosed;
        if (isClosed)
        {
            ToggleTextfieldVisibility(_showNames: true, _showTimes: true);
            canvasGroup.GetComponent<RectTransform>().DOKill();
            canvasGroup.GetComponent<RectTransform>().DOAnchorPos3D(canvasOpenLocation, 0.5f);
        }
        else
        {
            ToggleTextfieldVisibility(_showNames: false, _showTimes: false);
            canvasGroup.GetComponent<RectTransform>().DOKill();
            canvasGroup.GetComponent<RectTransform>().DOAnchorPos3D(canvasOpenLocation + canvasClosedLocationOffset, 0.5f);
        }

    }
}
