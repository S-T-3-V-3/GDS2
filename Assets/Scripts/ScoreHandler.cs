using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler {
    public List<TeamID> currentTiles;
    public string scoreString;

    GameManager gameManager;

    public ScoreHandler(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Reset() {
        currentTiles = new List<TeamID>();
        currentTiles.AddRange(gameManager.GetComponent<TileManager>().tiles.Select(x => x.GetTeam()));
    }

    public void UpdateScore(TeamID oldTeamID, TeamID newTeamID) {
        if (currentTiles.Count == 0) return;

        bool success = false;
        int i = 0;

        // Fix this
        //currentTiles.Where(x => x.)
        while (!success) {
            if (currentTiles[i] == oldTeamID)
                success = true;
            else
            {
                if (i+1 < currentTiles.Count)
                    i++;
                else
                    return;
            }
        }

        if (success) {
            currentTiles[i] = newTeamID;
            gameManager.GetTeamManager().GetTeam(oldTeamID).currentScore--;
            gameManager.GetTeamManager().GetTeam(newTeamID).currentScore++;
        }

        scoreString = "";            

        foreach (Team currentTeam in gameManager.GetTeamManager().currentTeams) {
            if (currentTeam.ID == TeamID.NONE) {
                if (gameManager.GetTeamManager().GetTeam(currentTeam.ID).currentScore > 0)
                    scoreString += "Tiles Unclaimed: " + gameManager.GetTeamManager().GetTeam(TeamID.NONE).currentScore + "\n";
            }
            else {
                if (gameManager.GetTeamManager().GetTeam(currentTeam.ID).currentScore > 0)
                    scoreString += "Team " + currentTeam.ID + ": " + currentTeam.currentScore + "\n";
            }
        }
    }
}
