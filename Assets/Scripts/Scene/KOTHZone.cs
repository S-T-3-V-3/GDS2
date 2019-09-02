using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTHZone : MonoBehaviour
{   
    GameManager gameManager;
    Renderer rend;
    public List<PlayerController> currentPlayers;
    Color col = Color.white;

    float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rend = GetComponent<Renderer>();        
    }

    // Update is called once per frame
    void Update() {
        ScoreUpdate();
        if(currentPlayers.Count > 0){            
            col.a = .75f;
            rend.material.SetColor("_BaseColor", col);
            //Debug.Log("LIT");
        } else {
            col.a = .2f;
            rend.material.SetColor("_BaseColor", col);
            //Debug.Log("UNLIT");
        }
    }

    public void ScoreUpdate() {
        // Only update if the round is in progress AND there are players inside the area
        if ((gameManager.sessionData.roundManager.roundIsStarted && currentPlayers.Count > 0) == false) return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= gameManager.gameSettings.KOTHInterval) {
            //ADD SCORE HERE
            foreach(PlayerController player in currentPlayers){
                gameManager.sessionData.score.currentTeams.Find(x => x.teamID == player.teamID).AddScore(gameManager.gameSettings.KOTHPoints);
            }

            // Reset time + inform listeners of score change
            elapsedTime = 0f;
            gameManager.OnScoreUpdated.Invoke();
        }
    } 

    void OnTriggerEnter(Collider other)
    {   
        if (gameManager.sessionData.roundManager.roundIsStarted == false) return;        
        if (other.transform.parent.GetComponent<PlayerController>() == null) return;

        PlayerController overlappingPlayer = other.transform.parent.GetComponent<PlayerController>();
        currentPlayers.Add(overlappingPlayer);

        //Change colour
        //this.gameObject.GetComponent<MeshRenderer>().material = gameManager.GetTeamManager().GetTeamColor(overlappingPlayer.teamID);
    }

    void OnTriggerExit (Collider other)
    {
        if (gameManager.sessionData.roundManager.roundIsStarted == false) return;        
        if (other.transform.parent.GetComponent<PlayerController>() == null) return;

        PlayerController overlappingPlayer = other.transform.parent.GetComponent<PlayerController>();
        currentPlayers.Remove(overlappingPlayer);
    }
}
