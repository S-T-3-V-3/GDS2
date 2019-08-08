using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<Material> teamColors;
    public List<Team> currentTeams;

    public void newTeam(PlayerController player) {
        Team newTeam = new Team();
        newTeam.players.Add(player);
        newTeam.teamID = currentTeams.Count;
        newTeam.teamColor = teamColors[newTeam.teamID];
    }

    public Material getTeamColor(int teamID) {
        return teamColors[teamID];
    }
}

public class Team {
    public List<PlayerController> players;
    public int teamID;
    public Material teamColor;
}