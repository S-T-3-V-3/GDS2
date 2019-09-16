using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardControl : MonoBehaviour
{
    public GameManager gameManager;
    public RawImage scoreBarPrefab;

    const int HEIGHT = 40;
    float totalTiles;
    List<float> teamScores;    
    List<RawImage> teamScoreBars;

    // Start is called before the first frame update
    void Start()
    {

        totalTiles = 0;
        gameManager.OnTilesChanged.AddListener(UpdateScore);
        gameManager.OnSessionStart.AddListener(Init);
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void UpdateScore()
    {
        totalTiles = gameManager.sessionData.score.GetTotalTiles();

        foreach (ScoreClass teamscore in gameManager.sessionData.score.currentTeams)
        {
            //Inefficient- oops
            foreach (RawImage ri in teamScoreBars)
            {
                if (ri.color == gameManager.teamManager.GetTeam(teamscore.teamID).color)
                {
                    RectTransform rt = ri.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(teamscore.numTiles / totalTiles * (GetComponent<RectTransform>().rect.width)*0.96f, HEIGHT);
                }
            }
        }
    }

    void Init()
    {
        if (teamScoreBars == null)
        {
            teamScoreBars = new List<RawImage>();
            foreach (ScoreClass teamScore in gameManager.sessionData.score.currentTeams)
            {
                //Debug.Log("Test: " + teamScore);
                teamScoreBars.Add(CreateTeamScoreBar(teamScore.teamID));
                //teamScore.score
            }
        }
    }

    public RawImage CreateTeamScoreBar(TeamID playerTeam)
    {
        RawImage newScoreBar = Instantiate(scoreBarPrefab, transform) as RawImage;
        newScoreBar.color = gameManager.teamManager.GetTeam(playerTeam).color;
        RectTransform rt = newScoreBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2((GetComponent<RectTransform>().rect.width)/(gameManager.sessionData.score.currentTeams.Count), HEIGHT);

        return newScoreBar;
    }
}
