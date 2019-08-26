using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ConnectedPlayers : MonoBehaviour
{
    public TextMeshProUGUI connectedPlayers;
    public string connectedPlayersText {
        get {
            return connectedPlayers.text;
        }
        set {
            connectedPlayers.text = value;
        }
    }

    List<PlayerUI> playerDetails;

    public void UpdatePlayerList(List<PlayerController> players) {
        if (playerDetails == null)
            playerDetails = new List<PlayerUI>();

        int i = 0;
        connectedPlayers.text = "";

        foreach (PlayerController pc in players.Where(x => x.model.activeSelf == false)) {
            PlayerUI newPlayer = new PlayerUI();
            newPlayer.playerName = "Team: " + pc.teamID + "\n" + "Character: " + pc.character.name + "\n";
            newPlayer.playerID = i;
            newPlayer.playerTeamID = pc.teamID;
            i++;

            playerDetails.Add(newPlayer);

            connectedPlayers.text += newPlayer.playerName + "\n";
        }
    }
}

public class PlayerUI {
    public string playerName;
    public int playerID;
    public TeamID playerTeamID;
}