using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ConnectedPlayers : MonoBehaviour
{
    public PlayerLobbyPrefab PlayerLobbyPrefab;
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
    PlayerLobbyPrefab plp;
    List<PlayerLobbyPrefab> playerLobbyPrefabList;

    public void UpdatePlayerList(List<PlayerController> players) {
        if (playerDetails == null)
            playerDetails = new List<PlayerUI>();

        if (playerLobbyPrefabList == null)
            playerLobbyPrefabList = new List<PlayerLobbyPrefab>();

        int i = 0;
        connectedPlayers.text = "";

        //Clear menu
        for(int x=0; x<playerLobbyPrefabList.Count; x++){
            Destroy(playerLobbyPrefabList[x]);
            playerLobbyPrefabList.RemoveAt(x);
            Debug.Log("Destroyed, list size: "+playerLobbyPrefabList.Count);
            
        }       
        foreach (Transform child in transform)
        {
             Destroy(child.gameObject);
        }

        foreach (PlayerController pc in players.Where(x => x.model.activeSelf == false)) {    
            playerLobbyPrefabList.Add(Instantiate(PlayerLobbyPrefab,transform));
            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerNumber.text = "Player " + players.IndexOf(pc) + 1;
            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.text = "" + pc.teamID; 

            if(pc.teamID == TeamID.BLUE){ playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = Color.blue; }
            else if (pc.teamID == TeamID.GREEN){ playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = Color.green; }
            else if (pc.teamID == TeamID.RED){ playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = Color.red; }
            else if (pc.teamID == TeamID.YELLOW){ playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = Color.yellow; }
            else if (pc.teamID == TeamID.PURPLE){ playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = new Color(1,0,1,1); }
            else if (pc.teamID == TeamID.ORANGE){ playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = new Color(1,0.65f,0,1); }
            else if (pc.teamID == TeamID.NONE){ playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = Color.white; }
            
            
            PlayerUI newPlayer = new PlayerUI();
            newPlayer.playerName = "Team: " + pc.teamID + "\n" + "Character: " + pc.character.name + "\n";
            newPlayer.playerID = i;
            newPlayer.playerTeamID = pc.teamID;
            i++;

            playerDetails.Add(newPlayer);

            //connectedPlayers.text += newPlayer.playerName + "\n";
        }
    }

}

public class PlayerUI {
    public string playerName;
    public int playerID;
    public TeamID playerTeamID;
}