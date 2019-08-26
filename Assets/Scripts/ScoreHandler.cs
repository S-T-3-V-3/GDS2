using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler {
    public List<ScoreClass> currentTeams;
    GameManager gameManager;

    float elapsedTime = 0f;

    public void ScoreUpdate() {
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

        foreach(ScoreClass team in currentTeams) {
            if (team.teamID != TeamID.NONE)
                numTilesCaptured += team.numTiles;
        }

        foreach(ScoreClass team in currentTeams) {
            if (team.teamID != TeamID.NONE) {
                if (team.numTiles > 0)
                    team.score += ((float)team.numTiles / (float)numTilesCaptured) * gameManager.gameSettings.pointsPerInterval;
            }
        }
    }

    public ScoreHandler(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Reset() {
        if (currentTeams != null)
            currentTeams.Clear();

        currentTeams = new List<ScoreClass>();

        foreach(TeamID teamID in gameManager.GetTeamManager().currentTeams.Select(x => x.ID)) {
            ScoreClass newTeam = new ScoreClass();
            newTeam.teamID = teamID;
            currentTeams.Add(newTeam);
        }

        currentTeams.Where(x => x.teamID == TeamID.NONE).First().numTiles = gameManager.GetComponent<MapManager>().tiles.Select(x => x.GetTeam()).Where(x => x == TeamID.NONE).Count();
    }

    public void UpdateTileCount(TeamID oldTeamID, TeamID newTeamID) {
        currentTeams.Where(x => x.teamID == oldTeamID).First().numTiles--;
        currentTeams.Where(x => x.teamID == newTeamID).First().numTiles++;
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