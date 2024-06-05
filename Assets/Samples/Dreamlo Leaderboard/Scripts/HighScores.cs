using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// Using the <see href="https://dreamlo.com/">https://dreamlo.com/</see> site - by Carmine T. Guida
/// <para>
/// i.e. https://dreamlo.com/lb/ privateCode
/// <code>
/// e.g. https://dreamlo.com/lb/cduv_DONOTSTORELIKETHIS_66Y3BrFwILtMepzjDAaDcw
/// </code>
/// </para>
/// <para>
/// Original Author: John Abel Sem1 2023.
/// Modified by: Mark Hoey.
/// </para>
/// <para>
/// Current to: 2023.11.11
/// <code>
/// == Adding and deleting scores ==
/// Changes and updates to your leaderboard are made through simple http get requests using your private url.
/// A player named Carmine got a score of 100. If the same name is added twice, we use the higher score.
/// http://dreamlo.com/lb/cduv_DONOTSTORELIKETHIS_66Y3BrFwILtMepzjDAaDcw/add/Carmine/100
/// A player named Carmine got a score of 1000 in 90 seconds.
/// http://dreamlo.com/lb/cduv_DONOTSTORELIKETHIS_6Y3BrFwILtMepzjDAaDcw/add/Carmine/1000/90
/// A player named Carmine got a score of 1000 in 90 seconds and is Awesome.
/// http://dreamlo.com/lb/cduv_DONOTSTORELIKETHIS_66Y3BrFwILtMepzjDAaDcw/add/Carmine/1000/90/Awesome
/// Delete Carmine's score
/// http://dreamlo.com/lb/cduv_DONOTSTORELIKETHIS_66Y3BrFwILtMepzjDAaDcw/delete/Carmine
/// Clear all scores
/// http://dreamlo.com/lb/cduv_DONOTSTORELIKETHIS_66Y3BrFwILtMepzjDAaDcw/clear
/// 
/// == Getting your scores ==
/// Reading of data is performed by using your public url.
/// Get your data as XML:
/// http://dreamlo.com/lb/637_DONOTSTORELIKETHIS_044797aa/xml
/// Get your data as json:
/// http://dreamlo.com/lb/637_DONOTSTORELIKETHIS_044797aa/json
/// Get your data as pipe delimited:
/// http://dreamlo.com/lb/637_DONOTSTORELIKETHIS_044797aa/pipe
/// </code>
/// </para>
/// **** Should implement profanity filter but be aware of the Scunthorpe problem ****
/// </summary>
public class HighScores : FastBehaviour
{
    // Secret Settings to use the external API
    [SerializeField] string privateCode = "GET IT FROM DREAMLO.COM - ENTER IN EDITOR - DO NOT SAVE IN CODE!";   //Key to Upload New Info
    [SerializeField] string publicCode = "GET IT FROM DREAMLO.COM - ENTER IN EDITOR - DO NOT SAVE IN CODE!";    //Key to download
    const string webURL = "http://dreamlo.com/lb/"; //  Website the keys are for

    [Header("Events")]
    [SerializeField] private UnityEvent OnTopScoresUpdate;
    [SerializeField] private UnityEvent OnAllScoresUpdate;
    [SerializeField] private UnityEvent OnRankUpdate;

    [Header("Settings")]
    [SerializeField] private int displayAmount = 5;

    //Score Lists
    [HideInInspector] public PlayerDataForLeaderboard[] topScoresList;
    public PlayerDataForLeaderboard[] allScoresList;

    //Rank
    [HideInInspector] public int scoreRank = 0;
    [HideInInspector] public bool requestingRank = false;
    private int requestRankScore = 0;

    
    void Awake()
    {
        requestingRank = false;
    }

    string SanitizeURI(string _string)
    {
        string[] input = _string.Split("/");
        List<string> output = new List<string>();
        for (int i = 0; i < input.Length; i++)
        {
            output.Add(UnityWebRequest.EscapeURL(input[i]));
        }
        return String.Join("/", output);
    }

    string PrivateURI(string _arguments = "")
    {
        string _sanitizedArguments = SanitizeURI(_arguments);
        return webURL + privateCode + _sanitizedArguments;
    }

    string PublicURI(string _arguments = "")
    {
        string _sanitizedArguments = SanitizeURI(_arguments);
        return webURL + publicCode + _sanitizedArguments;
    }

    /// <summary>
    /// Get the data based on a Uniform Resource Identifier (URI) 
    /// <para>
    /// Note: Free use of Dreamlo uses HTTP:// not HTTPS://
    /// <para>
    /// In, Project Settings Window, Player tab, make sure to "Allow Downloads over HTTP" is set to "Always allowed".
    /// </para>
    /// </para>
    /// <para>
    /// For the beginner:
    /// HTTP stands for Hypertext Transfer Protocol.
    /// HTTPS stands for Hypertext Transfer Protocol Secure - uses SSL (secure sockets layer) certificate.
    /// </para>
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="successCallback"></param>
    /// <returns></returns>
    IEnumerator WebRequest_URI(string uri, System.Action<string> successCallback = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string responseText = webRequest.downloadHandler.text;
                    if (responseText.Contains("ERROR SAME REQUEST WITHIN A SECOND"))
                    {
                        yield return new WaitForSeconds(3f);
                        StartCoroutine(WebRequest_URI(uri, successCallback));
                        break;
                    }
                    else
                    {
                        if (successCallback != null) successCallback(responseText);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Upload the score to Dreamlo server - just name and score. 
    /// <para>Normally Dreamlo overrides a record if the username is that same, it only stores the highest score by that username. If wanting a user to have multiple records then append a random string to the username (or in this case a time stamp).
    /// </para>
    /// <para>
    /// <code> Console.WriteLine(DateTime.UtcNow.ToString("%K")); </code>
    /// <code>DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK")</code>
    /// Gives 2009-06-11T16:11:10Z
    /// <code>DateTime.Now.ToUniversalTime().ToString("o")</code>
    /// Gives 2009-06-11T16:11:10.5312500Z
    /// <code>System.DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")</code>
    /// Gives 2009-06-11T16:26:47Z'
    /// <para>
    /// AFAIK the end result here is the same as just doing. DateTime.UtcNow.ToString(YOUR_FORMAT). That is: DateTime.Now.ToUniversalTime() == DateTime.UtcNow.
    /// </para>
    ///  <para>
    /// I prefer to use:
    /// <code>DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");</code>
    /// </para>
    /// </para>
    /// </summary>
    /// <param name="username">name for leaderboard</param>
    /// <param name="score">associated score</param>
    /// <param name="overrideSamePlayer">create non-unique records based on username - only stores highest score version</param>
    //https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings?redirectedfrom=MSDN#KSpecifier

    public void UploadScore(string username, int score, bool overrideSamePlayer = false)
    {
        if (overrideSamePlayer)
        {
            StartCoroutine(WebRequest_URI(PrivateURI($"/add-pipe/{username}/{score}"), OrganizeTopScores));
            return;
        }

        string UtcTimeSavedIso8601 = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
        StartCoroutine(WebRequest_URI(PrivateURI($"/add-pipe/{username}_{UtcTimeSavedIso8601}/{score}"), OrganizeTopScores));
    }


    /// <summary>
    /// Upload the score to Dreamlo server - name, score, time, comment (that is all Dreamlo allows for)
    /// </summary>
    /// <param name="username"></param>
    /// <param name="score"></param>
    /// <param name="time"></param>
    /// <param name="comment"></param>
    /// <param name="overrideSamePlayer"></param>
    public void UploadScoreTimeComment(string username, int score, int time, string comment, bool overrideSamePlayer = false)
    {
        if (overrideSamePlayer)
        {
            StartCoroutine(WebRequest_URI(PrivateURI($"/add-pipe/{username}/{score}/{time}/{comment}"), OrganizeTopScores));
            return;
        }

        string UtcTimeSavedIso8601 = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffZ");
        StartCoroutine(WebRequest_URI(PrivateURI($"/add-pipe/{username}_{UtcTimeSavedIso8601}/{score}/{time}/{comment}"), OrganizeTopScores));
    }

    /// <summary>
    /// Remove a specific username (and associated data) from Dreamlo server
    /// </summary>
    /// <param name="username"></param>
    public void RemoveScore(string username)
    {
        StartCoroutine(WebRequest_URI(PrivateURI($"/delete/{username}")));
    }

    /// <summary>
    /// Remove all records from the Dreamlo server leaderboard
    /// </summary>
    public void RemoveAllScores()
    {
        StartCoroutine(WebRequest_URI(PrivateURI($"/clear")));
        
    }

  

    /// <summary>
    /// Get nTh number of scores from the Dreamlo server. 
    /// e.g. Top 3 scores, top 5 scores. Max 25 stored on Dreamlo servers
    /// </summary>
    /// <param name="_null"></param>
    public void GetTopScores(string _null = "")
    {
        StartCoroutine(WebRequest_URI(PublicURI($"/pipe/0/{displayAmount}"), OrganizeTopScores));
    }

    /// <summary>
    /// Download data from Dreamlo server and establish rank based on supplied score
    /// </summary>
    /// <param name="score"></param>
    public void GetRank(int score)
    {
        requestingRank = true;
        requestRankScore = score;
        StartCoroutine(WebRequest_URI(PublicURI($"/pipe"), OrganizeCurrentRank));
    }

    /// <summary>
    /// Download all data from Dreamlo server (max 25 records stored)
    /// </summary>
    /// <param name="_null"></param>
    public void GetAllScores(string _null = "")
    {
        StartCoroutine(WebRequest_URI(PublicURI($"/pipe"), OrganizeAllScores));
    }

    public void RefreshScoresDelayed(float waitTime = 3f)
    {
        StartCoroutine(GetAllScoresDelayed(waitTime));
    }
    /// <summary>
    /// Cannot be exposed by UnityEvents so uses <see cref="RefreshScoresDelayed"/>
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public IEnumerator GetAllScoresDelayed(float waitTime = 3f)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
#if UNITY_EDITOR
            print("Updated Data from online: " + Time.time);
#endif
            GetAllScores();
        }
    }

    /// <summary>
    /// Load, and parse, all scores from web string
    /// TODO: Check for nulls
    /// </summary>
    /// <param name="responseText"></param>
    void OrganizeAllScores(string responseText)
    {
        string[] entries = responseText.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        allScoresList = new PlayerDataForLeaderboard[entries.Length];
        for (int i = 0; i < entries.Length; i++) //For each entry in the string array
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0].Split("_")[0];
            int score = int.Parse(entryInfo[1]);
            int time = int.Parse(entryInfo[2]);
            string comment = entryInfo[3];
            allScoresList[i] = new PlayerDataForLeaderboard(username, score, time, comment, _fullUsername: entryInfo[0]);
        }
        if (OnAllScoresUpdate != null)
        {
            OnAllScoresUpdate?.Invoke();
        }
    }

    /// <summary>
    /// Divides Leaderboard info by new lines
    /// </summary>
    /// <param name="rawData"></param>
    void OrganizeTopScores(string rawData) 
    {
        string[] entries = rawData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        topScoresList = new PlayerDataForLeaderboard[entries.Length];
        for (int i = 0; i < entries.Length; i++) //For each entry in the string array
        {
            if (i > displayAmount)
            {
                break;
            }
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0].Split("_")[0];
            int score = int.Parse(entryInfo[1]);
            topScoresList[i] = new PlayerDataForLeaderboard(username, score, _fullUsername: entryInfo[0]);
        }
        if (OnTopScoresUpdate != null)
        {
            OnTopScoresUpdate?.Invoke();
        }
    }

    /// <summary>
    /// Put current score within loaded data - and order correctly
    /// </summary>
    /// <param name="responseText"></param>
    void OrganizeCurrentRank(string responseText)
    {
        string[] entries = responseText.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        scoreRank = entries.Length + 1;
        for (int i = 0; i < entries.Length; i++) //For each entry in the string array
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            int _score = int.Parse(entryInfo[1]);
            if (_score < requestRankScore)
            {
                scoreRank = i + 1;
                break;
            }
        }
        if (OnRankUpdate != null)
        {
            OnRankUpdate?.Invoke();
        }
    }

    /// <summary>
    /// This could be used to create local version of pipe-delimited version 
    /// similar to what Dreamlo.com outputs. 
    /// <para>
    /// Note: it does timestamp and index by default
    /// </para>
    /// <para>
    /// Note: it only stores 25 records
    /// </para>
    /// <para>
    /// e.g.
    /// <code>
    /// Ethan_???|3549|0||12/1/3097 7:30:35 AM|0
    /// Jesse_???|2864|0||12/1/3097 8:03:28 AM|1
    /// Josh_???|2320|0||12/1/3097 8:12:51 AM|2
    /// Bob_???|1978|0||12/1/3097 7:48:00 AM|3
    /// Josh_???|1880|0||12/1/3097 8:08:26 AM|4
    /// Isame_???|1839|0||12/3/3097 4:01:47 PM|5
    /// Isame_???|1769|0||6/12/3097 5:48:32 AM|6
    /// ...
    /// Isame_???|241|0||3/21/3097 11:32:50 PM|19
    /// Isame_???|136|0||2/11/3097 1:58:14 PM|20
    /// Isame_???|100|0||11/26/3097 11:56:52 PM|21
    /// Isame_???|25|0||11/26/3097 11:56:59 PM|22
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string FormatAsPipeDelimited(PlayerDataForLeaderboard[] data)
    {
        // Using StringBuilder for efficient string concatenation
        System.Text.StringBuilder formattedString = new System.Text.StringBuilder();

        foreach (var score in data)
        {
            // Append each field with a pipe delimiter
            formattedString.Append(score.fullUsername);
            formattedString.Append("|");
            //formattedString.Append(score.username);
            //formattedString.Append("|");
            formattedString.Append(score.score);
            formattedString.Append("|");
            formattedString.Append(score.time);
            formattedString.Append("|");
            formattedString.Append(score.comment);
            formattedString.Append(Environment.NewLine); // Optional: add a new line for each record
        }

        return formattedString.ToString();
    }
}
