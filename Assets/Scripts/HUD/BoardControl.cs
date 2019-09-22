using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardControl : MonoBehaviour
{
    public GameManager gameManager;
    public RawImage scoreBarPrefab;

    //const int HEIGHT = 50;
    float totalTiles;
    //List<float> teamScores;    
    List<RawImage> teamScoreBars;

    // Start is called before the first frame update
    void Start()
    {

        totalTiles = 0;
        gameManager.OnTilesChanged.AddListener(UpdateScore);
        gameManager.OnSessionStart.AddListener(Init);
        gameManager.OnMapLoaded.AddListener(Clear);
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void UpdateScore()
    {
        gameObject.SetActive(true);
        totalTiles = gameManager.sessionData.score.GetTotalTiles();
        Debug.Log("Total Tiles: " + totalTiles);
        foreach (ScoreClass teamscore in gameManager.sessionData.score.currentTeams)
        {
            //Inefficient- oops
            foreach (RawImage ri in teamScoreBars)
            {
                if (ri.color == gameManager.teamManager.GetTeam(teamscore.teamID).color)
                {
                    if(teamscore.numTiles > 0) { Debug.Log("Team " + teamscore.teamID + " tiles: " + teamscore.numTiles); }                    
                    RectTransform rt = ri.GetComponent<RectTransform>();                    
                    rt.sizeDelta = new Vector2(teamscore.numTiles / totalTiles * (GetComponent<RectTransform>().rect.width)*0.96f, GetComponent<RectTransform>().rect.height*0.7f);
                }
            }
        }
    }

    void Init()
    {
            teamScoreBars = new List<RawImage>();
            foreach (ScoreClass teamScore in gameManager.sessionData.score.currentTeams)
            {
                //Debug.Log("Test: " + teamScore);
                teamScoreBars.Add(CreateTeamScoreBar(teamScore.teamID));
                //teamScore.score
            }
        gameObject.SetActive(false);
    }

    public RawImage CreateTeamScoreBar(TeamID playerTeam)
    {
        RawImage newScoreBar = Instantiate(scoreBarPrefab, transform) as RawImage;
        newScoreBar.color = gameManager.teamManager.GetTeam(playerTeam).color;
        RectTransform rt = newScoreBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2((GetComponent<RectTransform>().rect.width)/(gameManager.sessionData.score.currentTeams.Count), GetComponent<RectTransform>().rect.height*0.7f);

        return newScoreBar;
    }

    void Clear()
    {
        if (teamScoreBars != null)
        {
            foreach (RawImage ri in teamScoreBars)
            {
                Destroy(ri.gameObject);
            }

            teamScoreBars.Clear();
            //teamScoreBars = null;
            Init();
        }
    }

}
