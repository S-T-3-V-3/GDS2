using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler {
    public List<ScoreClass> currentTeams;
    public ScoreClass winningTeam;
    GameManager gameManager;

    float elapsedTime = 0f;

    public void ScoreUpdate() {
        if (GameManager.Instance.sessionData.isPaused) return;
        
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= gameManager.gameSettings.pointDistributionInterval) {
            CalculateScore();
            gameManager.OnScoreUpdated.Invoke();
            elapsedTime = 0f;
        }
    }

    public void PlayerKilled(TeamID killerTeam) {
        currentTeams.Where(x => x.teamID == killerTeam).First().AddScore(gameManager.gameSettings.pointsPerKill);
        gameManager.OnScoreUpdated.Invoke();
    }

    void CalculateScore() {
        int numTilesCaptured = 0;
        
        if (this.winningTeam == null)
            this.winningTeam = currentTeams[0];

        ScoreClass winningTeam = currentTeams[0];

        foreach(ScoreClass team in currentTeams) {
            if (team.teamID != TeamID.NONE)
                numTilesCaptured += team.numTiles;
        }

        foreach(ScoreClass team in currentTeams) {
            if (team.teamID != TeamID.NONE) {
                if (team.numTiles > 0)
                    team.score += ((float)team.numTiles / (float)numTilesCaptured) * gameManager.gameSettings.pointsPerInterval;
                
                if (team.score > winningTeam.score) {
                    winningTeam = team;
                }
            }
        }

        if (winningTeam.teamID != this.winningTeam.teamID) {
            this.winningTeam = winningTeam;
            gameManager.OnNewWinningTeam.Invoke();
        }
    }

    public ScoreHandler(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Reset() {
        if (currentTeams != null)
            currentTeams.Clear();

        currentTeams = new List<ScoreClass>();

        foreach(TeamID teamID in gameManager.teamManager.currentTeams.Select(x => x.ID)) {
            ScoreClass newTeam = new ScoreClass();
            newTeam.teamID = teamID;
            currentTeams.Add(newTeam);
        }

        currentTeams.Where(x => x.teamID == TeamID.NONE).First().numTiles = gameManager.GetComponent<MapManager>().tiles.Select(x => x.GetTeam()).Where(x => x == TeamID.NONE).Count();
    }
    
    public void ResetTileCount() {
        if (currentTeams == null) return;
        
        foreach(ScoreClass team in currentTeams) {
            team.numTiles = 0;
        }
    }

    public void UpdateTileCount(TeamID oldTeamID, TeamID newTeamID) {
        currentTeams.Where(x => x.teamID == oldTeamID).First().numTiles--;
        currentTeams.Where(x => x.teamID == newTeamID).First().numTiles++;
    }

    public float GetTotalTiles()
    {
        int numTilesCaptured = 0;

        foreach (ScoreClass team in currentTeams)
        {
            if (team.teamID != TeamID.NONE)
                numTilesCaptured += team.numTiles;
        }
        return numTilesCaptured;
    }

    public float GetTotalScore()
    {
        float numTotalScore = 0;

        foreach (ScoreClass team in currentTeams)
        {
            if (team.teamID != TeamID.NONE)
                numTotalScore += team.score;
        }
        return numTotalScore;
    }
}

public class ScoreClass {
    public TeamID teamID = TeamID.NONE;
    public int numTiles = 0;
    public float score = 0;

    public void AddScore(float value) {
        score += value;
    }
}