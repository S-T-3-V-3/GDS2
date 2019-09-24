using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KOTHZone : MonoBehaviour
{   
    public List<PlayerController> currentPlayers;
    public float moveSpeed = 10f;

    UnityEvent OnPlayersChanged;

    GameManager gameManager;
    Renderer rend;    
    Color baseColor = Color.white;
    Color targetColor, currentColor;
    Vector3 currentVelocity;
    Vector3 targetPos = Vector3.zero;

    float elapsedTime = 0f;
    float activeAlpha = 0.15f;
    float inactiveAlpha = 0.05f;
    float targetAlpha;
    bool isChangingColor = false;
    int numPlayers = 0;

    float colorChangeTransitionTime = 0.2f;
    float colorChangeElapsedTime = 0f;

    void Start()
    {
        OnPlayersChanged = new UnityEvent();
        OnPlayersChanged.AddListener(SetNewColor);
        
        gameManager = FindObjectOfType<GameManager>();
        rend = GetComponent<Renderer>(); 
        targetPos = gameManager.GetRandomPosition();
    }

    // Update is called once per frame
    void Update() {
        if (gameManager.sessionData.roundManager.isStarted == false) return;

        if (numPlayers > 0)
            DoScoreUpdate();

        if (isChangingColor)
            DoColorUpdate();

        SeekTarget();
    }

    public void DoScoreUpdate() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= gameManager.gameSettings.KOTHInterval) {
            foreach (PlayerController player in currentPlayers) {
                gameManager.sessionData.score.currentTeams.Find(x => x.teamID == player.teamID).AddScore(gameManager.gameSettings.KOTHPoints);

                TextPopupHandler textPopup = GameObject.Instantiate(gameManager.TextPopupPrefab).GetComponent<TextPopupHandler>();
                string textValue = "+" + gameManager.gameSettings.KOTHPoints.ToString();
                textPopup.Init(player.pawnPosition, textValue, gameManager.teamManager.GetTeam(player.teamID).color, 0.7f);
                textPopup.lifetime = 0.75f;
            }

            // Reset time + inform listeners of score change
            elapsedTime = 0f;
            gameManager.OnScoreUpdated.Invoke();
        }
    }

    void SetNewColor()
    {
        numPlayers = currentPlayers.Count;
        currentColor = rend.material.GetColor("_TintColor");
        targetAlpha = currentPlayers.Count > 0 ? activeAlpha : inactiveAlpha;
        targetColor = baseColor;

        foreach (PlayerController p in currentPlayers) {
            targetColor += gameManager.teamManager.GetTeam(p.teamID).color;
        }

        targetColor /= numPlayers+1;
        targetColor.a = targetAlpha;

        isChangingColor = true;
        colorChangeElapsedTime = 0f;
    }

    void DoColorUpdate() {
        rend.material.SetColor("_TintColor", Color.Lerp(currentColor,targetColor,colorChangeElapsedTime/colorChangeTransitionTime));

        if (colorChangeElapsedTime > colorChangeTransitionTime)
            isChangingColor = false;
        else
            colorChangeElapsedTime += Time.deltaTime;
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
        if (gameManager.sessionData.roundManager.isStarted == false) return;

        if (other.tag == "Player") {
            PlayerController overlappingPlayer = other.GetComponent<PlayerModelController>().owner;
            currentPlayers.Add(overlappingPlayer);

            OnPlayersChanged.Invoke();
        }        
    }

    void OnTriggerExit (Collider other)
    {
        if (gameManager.sessionData.roundManager.isStarted == false) return;

        if (other.tag == "Player") {
            PlayerController overlappingPlayer = other.GetComponent<PlayerModelController>().owner;
            currentPlayers.Remove(overlappingPlayer);

            OnPlayersChanged.Invoke();
        }
    }
}
