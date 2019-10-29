using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;
    public List<Team> currentTeams;

    void Awake() {
        Instance = this;
        GameManager gameManager = GameManager.Instance;

        currentTeams = new List<Team>();

        foreach (Team team in gameManager.teamSettings.teams) {
            currentTeams.Add(team);
        }
    }

    public void JoinTeam(PlayerController player, TeamID teamID) {
        currentTeams.Where(x => x.ID == teamID).First().AddPlayer(player);
    }

    public void LeaveTeam(PlayerController player, TeamID teamID) {
        currentTeams.Where(x => x.ID == teamID).First().RemovePlayer(player);
    }

    public Team GetTeam(TeamID teamID) {
        return currentTeams.Where(x => x.ID == teamID).First();
    }

    public Color GetTeamColor(TeamID teamID) {
        return GetTeam(teamID).color;
    }

    public string GetTeamHexColor(TeamID teamID) {
        return GetTeam(teamID).hexColor;
    }

    public Color GetWinningTeamColor() {
        return GetTeamColor(GameManager.Instance.sessionData.score.winningTeam.teamID);
    }

    public List<Color> GetTeamColors()
    {
        List<Color> colors = new List<Color>();
        foreach (Team team in currentTeams)
        {
            colors.Add(team.color);
        }
        return colors;
    }
}

[System.Serializable]
public class Team {
    public List<PlayerController> players;
    public TeamID ID;
    public Color color;
    public string hexColor;
    public Material tileMat;
    public Material playerMat;
    public Material projectileMat;

    public void AddPlayer(PlayerController player) {
        players.Add(player);
    }

    public void RemovePlayer(PlayerController player) {
        players.Remove(player);
    }
}

public enum TeamID {
    NONE = 0,
    BLUE,
    GREEN,
    RED,
    YELLOW,
    PURPLE,
    ORANGE,
    CYAN,
    PINK
}