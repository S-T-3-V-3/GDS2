using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler {
    public List<TeamID> currentTiles;
    public string scoreString;

    GameManager gameManager;

    public void Calculate() {

    }

    public ScoreHandler(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Reset() {
        currentTiles = new List<TeamID>();
    }

    public void UpdateTileCount(TeamID oldTeamID, TeamID newTeamID) {
        if (currentTiles.Count == 0) {
            currentTiles.AddRange(gameManager.GetComponent<MapManager>().tiles.Select(x => x.GetTeam()));
        }

        int i = currentTiles.IndexOf(currentTiles.Where(x => x == oldTeamID).First());

        currentTiles[i] = newTeamID;
        gameManager.GetTeamManager().GetTeam(oldTeamID).numTiles--;
        gameManager.GetTeamManager().GetTeam(newTeamID).numTiles++;

        scoreString = "";           

        foreach (Team currentTeam in gameManager.GetTeamManager().currentTeams) {
            if (currentTeam.ID == TeamID.NONE) {
                if (gameManager.GetTeamManager().GetTeam(currentTeam.ID).numTiles > 0)
                    scoreString += "Tiles Unclaimed: " + gameManager.GetTeamManager().GetTeam(TeamID.NONE).numTiles + "\n";
            }
            else {
                if (gameManager.GetTeamManager().GetTeam(currentTeam.ID).numTiles > 0)
                    scoreString += "Team " + currentTeam.ID + ": " + currentTeam.numTiles + "\n";
            }
        }
    }
}
