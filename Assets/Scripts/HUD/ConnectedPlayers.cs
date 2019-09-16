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

        GameManager gameManager = FindObjectOfType<GameManager>();

        //Clear menu
        for(int x=0; x<playerLobbyPrefabList.Count; x++){
            Destroy(playerLobbyPrefabList[x]);
            playerLobbyPrefabList.RemoveAt(x);
            //Debug.Log("Destroyed, list size: "+playerLobbyPrefabList.Count);
            
        }       
        foreach (Transform child in transform)
        {
             Destroy(child.gameObject);
        }

        foreach (PlayerController pc in players.Where(x => x.model.activeSelf == false)) {    
            playerLobbyPrefabList.Add(Instantiate(PlayerLobbyPrefab,transform));
            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerNumber.text = "Player " + (players.IndexOf(pc) + 1);
            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.text = "" + pc.teamID; 

            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            
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