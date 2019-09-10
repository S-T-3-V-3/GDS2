using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTHZone : MonoBehaviour
{   
    public List<PlayerController> currentPlayers;
    public float moveSpeed = 10f;

    GameManager gameManager;
    Renderer rend;    
    Color col = Color.white;
    Vector3 currentVelocity;
    Vector3 targetPos = Vector3.zero;

    float elapsedTime = 0f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rend = GetComponent<Renderer>(); 
        targetPos = gameManager.GetRandomPosition();
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.sessionData.roundManager.roundIsStarted == false) return;

        ScoreUpdate();

        if(currentPlayers.Count > 0){            
            col.a = .1f;
            rend.material.SetColor("_TintColor", col);
            //Debug.Log("LIT");
        } else {
            col.a = .05f;
            rend.material.SetColor("_TintColor", col);
            //Debug.Log("UNLIT");
        }

        SeekTarget();
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

    void SeekTarget() {
        if ((this.transform.position - targetPos).magnitude > 0.5f) {
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref currentVelocity, moveSpeed);
        }
        else {
            targetPos = gameManager.GetRandomPosition();
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref currentVelocity, moveSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {   
        if (gameManager.sessionData.roundManager.roundIsStarted == false) return;        
        if (other.transform.parent.GetComponent<PlayerActiveState>() == null) return;

        PlayerController overlappingPlayer = other.transform.parent.GetComponent<PlayerController>();
        currentPlayers.Add(overlappingPlayer);

        //Change colour
        //this.gameObject.GetComponent<MeshRenderer>().material = gameManager.GetTeamManager().GetTeamColor(overlappingPlayer.teamID);
    }

    void OnTriggerExit (Collider other)
    {
        if (gameManager.sessionData.roundManager.roundIsStarted == false) return;        

        PlayerController overlappingPlayer = other.transform.parent.GetComponent<PlayerController>();
        currentPlayers.Remove(overlappingPlayer);
    }
}
