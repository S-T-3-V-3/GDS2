using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardControl : MonoBehaviour
{
    public GameManager gameManager;
    public RawImage scoreBarPrefab;

    float totalTiles;
    List<RawImage> teamScoreBars;

    void Start()
    {
        totalTiles = 0;
        gameManager.OnTilesChanged.AddListener(UpdateScore);
        gameManager.OnSessionStart.AddListener(Init);
        gameManager.OnMapLoaded.AddListener(Clear);       
    }

    void UpdateScore()
    {
        gameObject.SetActive(true);
        totalTiles = gameManager.sessionData.score.GetTotalTiles();
        foreach (ScoreClass teamscore in gameManager.sessionData.score.currentTeams)
        {
            if (teamscore.teamID != TeamID.NONE) {
                //Inefficient- oops
                foreach (RawImage ri in teamScoreBars)
                {
                    if (ri.color == TeamManager.Instance.GetTeamColor(teamscore.teamID))
                    {                 
                        RectTransform rt = ri.GetComponent<RectTransform>();                    
                        rt.sizeDelta = new Vector2(teamscore.numTiles / totalTiles * (GetComponent<RectTransform>().rect.width)*0.96f, GetComponent<RectTransform>().rect.height*0.7f);
                    }
                }
            }
        }
    }

    void Init()
    {
            teamScoreBars = new List<RawImage>();
            foreach (ScoreClass teamScore in gameManager.sessionData.score.currentTeams)
            {
                if (teamScore.teamID != TeamID.NONE) {
                    teamScoreBars.Add(CreateTeamScoreBar(teamScore.teamID));
                }
            }
        gameObject.SetActive(false);
    }

    public RawImage CreateTeamScoreBar(TeamID playerTeam)
    {
        RawImage newScoreBar = Instantiate(scoreBarPrefab, transform) as RawImage;
        newScoreBar.color = TeamManager.Instance.GetTeamColor(playerTeam);
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
