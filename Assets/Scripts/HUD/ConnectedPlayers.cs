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

        int i = 0;       

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

        foreach (PlayerController pc in players.Where(x => x.hasPawn == false)) {    
            playerLobbyPrefabList.Add(Instantiate(PlayerLobbyPrefab,transform));

            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerNumber.text = "Player " + (players.IndexOf(pc) + 1);

            playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].portrait.sprite = gameManager.gameSettings.characterPortraits[pc.playerModelSelection];
            playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].portrait.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.text = "" + pc.teamID; 
            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerTeam.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            playerLobbyPrefabList[playerLobbyPrefabList.Count-1].playerWeapon.text = ""+pc.playerWeaponSelection;

            if(pc.currentSelection == 0){
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].root.transform.localScale = new Vector3(1.15f,1.15f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerTeam.transform.localScale = new Vector3(1f, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerWeapon.transform.localScale = new Vector3(1, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].portraitLeftArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].portraitRightArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            }
            if (pc.currentSelection == 1)
            {
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].root.transform.localScale = new Vector3(1f, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerTeam.transform.localScale = new Vector3(1.15f, 1.15f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerWeapon.transform.localScale = new Vector3(1, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].teamLeftArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].teamRightArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            }
            if (pc.currentSelection == 2)
            {
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].root.transform.localScale = new Vector3(1f, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerTeam.transform.localScale = new Vector3(1f, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerWeapon.transform.localScale = new Vector3(1.15f, 1.15f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].weaponLeftArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].weaponRightArrow.color = gameManager.teamManager.GetTeam(pc.teamID).color;
            }
            if (pc.currentSelection == -1)
            {
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].root.transform.localScale = new Vector3(1f, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerTeam.transform.localScale = new Vector3(1f, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].playerWeapon.transform.localScale = new Vector3(1f, 1f);
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].portraitLeftArrow.color = Color.white;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].portraitRightArrow.color = Color.white;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].teamLeftArrow.color = Color.white;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].teamRightArrow.color = Color.white;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].weaponLeftArrow.color = Color.white;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].weaponRightArrow.color = Color.white;
            }

            if (pc.ready)
            {                
                tempcolor.a = .8f;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].GetComponent<RawImage>().color = tempcolor;
            } else {
                tempcolor.a = 150/255f;
                playerLobbyPrefabList[playerLobbyPrefabList.Count - 1].GetComponent<RawImage>().color = tempcolor;
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