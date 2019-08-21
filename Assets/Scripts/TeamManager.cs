using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<Team> currentTeams;

    public void JoinTeam(PlayerController player, TeamID teamID) {
        currentTeams.Where(x => x.ID == teamID).First().AddPlayer(player);
    }

    public void LeaveTeam(PlayerController player, TeamID teamID) {
        currentTeams.Where(x => x.ID == teamID).First().RemovePlayer(player);
    }


    public Material GetTeamColor(TeamID teamID) {
        return currentTeams.Where(x => x.ID == teamID).First().teamColor;
    }

    public Team GetTeam(TeamID teamID) {
        return currentTeams.Where(x => x.ID == teamID).First();
    }
}

[System.Serializable]
public class Team {
    public List<PlayerController> players;
    public TeamID ID;
    public Material teamColor;

    public int numTiles;

    public void AddPlayer(PlayerController player) {
        players.Add(player);
    }

    public void RemovePlayer(PlayerController player) {
        players.Remove(player);
    }
}

public enum TeamID {
    BLUE,
    GREEN,
    RED,
    YELLOW,
    PURPLE,
    ORANGE,
    NONE
}