using System.Collections.Generic;
using UnityEngine;

public class Score {
    public List<int> Tiles;
    public List<int> currentScores;
    public int unclaimedTiles = 0;
    public string scoreString;

    public void AddTile() {
        Tiles.Add(-1);
        unclaimedTiles++;
    }

    public void UpdateScore(int oldTeam, int newTeam) {
        if (Tiles.Count == 0) return;

        bool success = false;
        int i = 0;

        while (!success) {
            if (Tiles[i] == oldTeam)
                success = true;
            else
            {
                if (i+1 < Tiles.Count)
                    i++;
                else
                    return;
            }
        }

        if (success) {
            Tiles[i] = newTeam;

            if (oldTeam >= 0)
                currentScores[oldTeam]--;
            else
                unclaimedTiles--;

            currentScores[newTeam]++;
        }

        scoreString = "";

        if (unclaimedTiles > 0)
            scoreString += "Tiles Unclaimed: " + unclaimedTiles + "\n";

        for (int j = 0; j < currentScores.Count; j++) {
             scoreString += "Team " + j + ": " + currentScores[j] + "\n";
        }
    }
}
