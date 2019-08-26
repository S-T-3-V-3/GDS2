using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float smoothTime = 0.3f;
    public float minDistance = 10f;
    public float maxDistance = 3000f;
    
    GameManager gameManager;
    List<Transform> targets;
    Vector3 currentVelocity;

    Vector3 resetPos;
    float aspectRatio;
    bool isResetting = false;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnNewCameraTarget.AddListener(SetTargets);
        gameManager.sessionData.OnRoundComplete.AddListener(Reset);

        aspectRatio = Camera.main.aspect;
        targets = new List<Transform>();
        resetPos = this.transform.position;

        if (gameManager.sessionData == null)
            this.gameObject.SetActive(false);
    }

    void FixedUpdate() {
        if (isResetting) doReset();
        if (!gameManager.sessionData.roundManager.roundIsStarted) return;
        if (targets.Count == 0) return;

        Vector3 centerPosition = GetCenterPosition();
        Vector3 distanceOffset = new Vector3(0,Mathf.Clamp(GetDistance(), minDistance, maxDistance),0);

        this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, centerPosition+distanceOffset, ref currentVelocity, smoothTime);
    }

    public void Reset() {
        isResetting = true;
    }

    void doReset() {
        this.gameObject.transform.position = Vector3.SmoothDamp(this.gameObject.transform.position, resetPos, ref currentVelocity, smoothTime);

        if ((this.gameObject.transform.position - resetPos).magnitude < 1f)
            isResetting = false;
    }

    // Runs on change in player count, sets references to player gameobjects for the camera to track
    void SetTargets() {
        if (!gameManager.sessionData.roundManager.roundIsStarted) return;
        
        targets.Clear();

        foreach (PlayerController player in gameManager.currentPlayers.Where(x => x.currentStats.isAlive)) {
            targets.Add(player.model.gameObject.transform);
        }
    }

    // Returns center position of current players
    Vector3 GetCenterPosition() {
        if (targets.Count == 1)
            return targets[0].transform.position;

        // Note: Bounds constructor adds (0,0,0) if no arguments given
        Bounds bounds = new Bounds(targets[0].transform.position,Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            bounds.Encapsulate(targets[i].transform.position);
        }

        return bounds.center;
    }

    // Retreives max distance between two players, scaled to screen aspect ratio
    float GetDistance() {
        Bounds bounds = new Bounds(targets[0].transform.position,Vector3.zero);
        for (int i = 0; i < targets.Count; i++) {
            bounds.Encapsulate(targets[i].transform.position);
        }
        
        return Mathf.Max(bounds.size.x,bounds.size.z*aspectRatio);
    }
}