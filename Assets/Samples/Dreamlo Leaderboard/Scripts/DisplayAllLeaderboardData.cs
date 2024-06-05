using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Threading.Tasks;

public class DisplayAllLeaderboardData : MonoBehaviour
{
    [SerializeField] private HighScores highScores;

    private CanvasGroup canvasGroup;

    private List<TextMeshProUGUI> rankingTextfields;
    private List<TextMeshProUGUI> scoreTextfields;
    private List<TextMeshProUGUI> usernameTextfields;
    private List<GameObject> scoreGameObjects;


    private void Awake()
    {
        // Vertical Layout group caused issues with DoTween
        //if (GetComponent<VerticalLayoutGroup>() != null)
        //{
        //    Destroy(GetComponent<VerticalLayoutGroup>());
        //}

        //Use some helper extension methods to grab the objects/components automatically
        scoreGameObjects = gameObject.GetChildrenList();
        rankingTextfields = gameObject.GetGrandChildrenList<TextMeshProUGUI>(0);
        usernameTextfields = gameObject.GetGrandChildrenList<TextMeshProUGUI>(1);
        scoreTextfields = gameObject.GetGrandChildrenList<TextMeshProUGUI>(2);

    }

    private void Start()
    {
        if (highScores == null)
        {
            Debug.LogError($"Need to link to Highscores component. {this.name} on {this.gameObject.name} has been disabled.");
            this.enabled = false;
            return;
        }
    }

    public void UpdateValues()
    {
        for (int i = 0; i < highScores.allScoresList.Length; i++)
        //for (int i = 0; i < scoreGameObjects.Count; i++)
        {
            int offsetDueToHeaders = 1;

            if (i + offsetDueToHeaders >= scoreGameObjects.Count)
            {
                return;
            }

            scoreTextfields[i + offsetDueToHeaders].text = highScores.allScoresList[i].score.ToString("N0");
            usernameTextfields[i + offsetDueToHeaders].text = highScores.allScoresList[i].username.ToString();
            rankingTextfields[i + offsetDueToHeaders].text = $"{i + 1}";
            //timeTextfields[i + offsetDueToHeaders].text = highScores.allScoresList[i].time.GetTimeSpanString();
        }
    }
}
