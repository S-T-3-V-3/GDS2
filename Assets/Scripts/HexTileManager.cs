using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileManager : MonoBehaviour
{
    int currentTeam = -1;
    GameManager gameManager;

    public void Init(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    void OnTriggerEnter(Collider other)
    {   
        if (other.GetComponent<PlayerController>() == null) return;

        PlayerController overlappingPlayer = other.GetComponent<PlayerController>();

        if (currentTeam != overlappingPlayer.teamID) {
            gameManager.sessionData.score.UpdateScore(currentTeam,overlappingPlayer.teamID);
            currentTeam = overlappingPlayer.teamID;
            gameManager.OnScoreUpdated.Invoke();
        }

        this.gameObject.GetComponent<MeshRenderer>().material = gameManager.gameObject.GetComponent<TeamManager>().getTeamColor(overlappingPlayer.teamID);
    }
}
