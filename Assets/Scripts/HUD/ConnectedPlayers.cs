using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectedPlayers : MonoBehaviour
{
    public PlayerLobbyPrefab PlayerLobbyPrefab;
    List<PlayerUI> playerDetails;
    PlayerLobbyPrefab plp;
    List<PlayerLobbyPrefab> playerLobbyPrefabList;

    Color tempcolor;

    public void UpdatePlayerList(List<PlayerController> players) {
        if (playerDetails == null)
            playerDetails = new List<PlayerUI>();

        if (playerLobbyPrefabList == null)
            playerLobbyPrefabList = new List<PlayerLobbyPrefab>();

        GameManager gameManager = FindObjectOfType<GameManager>();

        foreach (PlayerLobbyPrefab x in playerLobbyPrefabList) {
            GameObject.Destroy(x);
        }
        playerLobbyPrefabList.Clear();

        foreach (Transform child in transform)
        {
             Destroy(child.gameObject);
        }

        foreach (PlayerController pc in players.Where(x => x.hasPawn == false)) { 
            PlayerLobbyPrefab currentPrefab = GameObject.Instantiate(PlayerLobbyPrefab,transform);
            playerLobbyPrefabList.Add(currentPrefab);

            currentPrefab.playerNumber.text = "Player " + (players.IndexOf(pc) + 1);

            currentPrefab.portrait.sprite = gameManager.gameSettings.characterPortraits[pc.playerModelSelection];
            currentPrefab.portrait.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            currentPrefab.playerTeam.text = "" + pc.teamID; 
            currentPrefab.playerTeam.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            currentPrefab.playerWeapon.text = ""+pc.playerWeaponSelection;

            if(pc.currentSelection == 0){
                currentPrefab.root.transform.localScale = new Vector3(1.15f,1.15f);
                currentPrefab.playerTeam.transform.localScale = new Vector3(1f, 1f);
                currentPrefab.playerWeapon.transform.localScale = new Vector3(1, 1f);
                currentPrefab.portraitLeftArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
                currentPrefab.portraitRightArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            }
            if (pc.currentSelection == 1)
            {
                currentPrefab.root.transform.localScale = new Vector3(1f, 1f);
                currentPrefab.playerTeam.transform.localScale = new Vector3(1.15f, 1.15f);
                currentPrefab.playerWeapon.transform.localScale = new Vector3(1, 1f);
                currentPrefab.teamLeftArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
                currentPrefab.teamRightArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            }
            if (pc.currentSelection == 2)
            {
                currentPrefab.root.transform.localScale = new Vector3(1f, 1f);
                currentPrefab.playerTeam.transform.localScale = new Vector3(1f, 1f);
                currentPrefab.playerWeapon.transform.localScale = new Vector3(1.15f, 1.15f);
                currentPrefab.weaponLeftArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
                currentPrefab.weaponRightArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            }
            if (pc.currentSelection == -1)
            {
                currentPrefab.root.transform.localScale = new Vector3(1f, 1f);
                currentPrefab.playerTeam.transform.localScale = new Vector3(1f, 1f);
                currentPrefab.playerWeapon.transform.localScale = new Vector3(1f, 1f);
                currentPrefab.portraitLeftArrow.color = Color.white;
                currentPrefab.portraitRightArrow.color = Color.white;
                currentPrefab.teamLeftArrow.color = Color.white;
                currentPrefab.teamRightArrow.color = Color.white;
                currentPrefab.weaponLeftArrow.color = Color.white;
                currentPrefab.weaponRightArrow.color = Color.white;
            }

            if (pc.ready)
            {                
                tempcolor.a = .8f;
                currentPrefab.GetComponent<RawImage>().color = tempcolor;
            } else {
                tempcolor.a = 150/255f;
                currentPrefab.GetComponent<RawImage>().color = tempcolor;
            }

            /*
            PlayerUI newPlayer = new PlayerUI();
            newPlayer.playerName = "Team: " + pc.teamID;
            newPlayer.playerID = i;
            newPlayer.playerTeamID = pc.teamID;
            i++;
            playerDetails.Add(newPlayer);
            */

            //connectedPlayers.text += newPlayer.playerName + "\n";
        }
    }

}

public class PlayerUI {
    public string playerName;
    public int playerID;
    public TeamID playerTeamID;
}