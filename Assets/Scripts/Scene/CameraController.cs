using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Transform cameraTransform;
    public Camera mainCamera;
    public StateManager cameraState;

    public PlayerController focusPlayer;
    public Vector3 resetPos;

    void Start() {
        if (Instance == null)
            Instance = this;
        else
            GameObject.Destroy(this.gameObject);

        cameraState = this.gameObject.AddComponent<StateManager>();

        resetPos = new Vector3(this.transform.position.x, cameraTransform.localPosition.y, this.transform.position.z);
        SetState<CameraInactiveState>();
    }

    public void SetState<T>() where T : State {
        cameraState.AddState<T>();
    }

    public void PauseAngle(Vector2 modifier) {
        SendMessage("SetAngle",modifier);
    }

    public void PauseOffset(Vector2 modifier) {
        SendMessage("SetOffset",modifier);
    }
}

public class CameraMenuState : State {
    public override void BeginState()
    {

    }
}

public class CameraActiveState : State
{
    public float smoothTime = 0.3f;
    public float minDistance = 10f;
    public float maxDistance = 3000f;
    public float angle = -30f;
    public float xOffset = -0.8f;
    public float aspectRatio;

    GameManager gameManager;
    CameraController cameraController;
    Transform cameraTransform;
    Vector3 currentVelocity;

    List<Transform> targets;

    public override void BeginState()
    {
        cameraController = CameraController.Instance;
        gameManager = GameManager.Instance;
        cameraTransform = cameraController.mainCamera.transform;

        gameManager.OnNewCameraTarget.AddListener(SetTargets);
        gameManager.sessionData.OnRoundComplete.AddListener(RoundComplete);

        aspectRatio = cameraController.mainCamera.aspect;

        targets = new List<Transform>();
        SetTargets();
    }

    void RoundComplete() {
        cameraController.SetState<CameraResetState>();
    }

    void FixedUpdate() {
        if (!gameManager.sessionData.roundManager.isStarted) return;
        if (targets.Count == 0) return;

        Vector3 centerPosition = GetCenterPosition();

        Vector3 distanceOffset = new Vector3(0,Mathf.Clamp(GetDistance(), minDistance, maxDistance),0);

        Vector3 currentPos = new Vector3(this.gameObject.transform.position.x, cameraTransform.localPosition.y, this.gameObject.transform.position.z);
        Vector3 newPos = Vector3.SmoothDamp(currentPos, centerPosition+distanceOffset, ref currentVelocity, smoothTime);

        this.gameObject.transform.position = new Vector3(newPos.x, this.gameObject.transform.position.y, newPos.z);

        cameraController.cameraTransform.localPosition = new Vector3(xOffset,newPos.y,0);
    }

    // Runs on change in player count, sets references to player gameobjects for the camera to track
    void SetTargets() {
        if (!gameManager.sessionData.roundManager.isStarted) return;
        
        targets.Clear();

        foreach (PlayerController player in gameManager.currentPlayers.Where(x => x.hasPawn)) {
            targets.Add(player.playerModel.transform);
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
        
        return Mathf.Max(bounds.size.x,bounds.size.z * aspectRatio);
    }
}

public class CameraFocusState : State {
    public float focusAngle = -50f;
    public float focusDistance = 9f;

    GameManager gameManager;
    CameraController cameraController;
    Vector3 focusRotation;
    Vector3 focusPositionOffset = new Vector3(0,0,0);
    Vector3 currentVelocity;
    float smoothTime;

    float maxOffsetAngle = 17f;
    float maxOffsetDistance = 0.5f;

    public override void BeginState()
    {
        cameraController = CameraController.Instance;
        gameManager = GameManager.Instance;
        focusRotation = this.transform.eulerAngles;
        smoothTime = 0.15f;
    }

    void FixedUpdate() {
        Vector3 playerPosition = cameraController.focusPlayer.playerModel.transform.position;
        Vector3 focusOffset = new Vector3(0, focusDistance, 0);

        Vector3 currentPos = new Vector3(this.gameObject.transform.position.x, cameraController.cameraTransform.localPosition.y, this.gameObject.transform.position.z);
        Vector3 newPos = Vector3.SmoothDamp(currentPos, playerPosition + focusOffset, ref currentVelocity, smoothTime);

        this.gameObject.transform.position = new Vector3(newPos.x, this.gameObject.transform.position.y, newPos.z);

        cameraController.cameraTransform.localPosition = new Vector3(focusPositionOffset.x,newPos.y,focusPositionOffset.z);
    }

    void SetAngle(Vector2 modifier) {
        this.transform.eulerAngles = focusRotation + new Vector3(modifier.y * maxOffsetAngle, 0, -modifier.x * maxOffsetAngle);
    }

    void SetOffset(Vector2 offset) {
        focusPositionOffset = new Vector3(offset.x * maxOffsetDistance, 0, offset.y * maxOffsetDistance);
    }

    new void EndState() {
        this.transform.eulerAngles = focusRotation;
    }
}

public class CameraResetState : State {
    public float smoothTime = 0.4f;

    GameManager gameManager;
    CameraController cameraController;
    Transform cameraTransform;
    Vector3 currentVelocity;
    Vector3 resetPos;

    public override void BeginState()
    {
        cameraController = CameraController.Instance;
        gameManager = GameManager.Instance;

        cameraTransform = cameraController.cameraTransform;
        resetPos = cameraController.resetPos;
    }

    void FixedUpdate() {
        Vector3 currentPos = new Vector3(this.gameObject.transform.position.x, cameraTransform.localPosition.y, this.gameObject.transform.position.z);
        Vector3 newPos = Vector3.SmoothDamp(currentPos, resetPos, ref currentVelocity, smoothTime);

        this.gameObject.transform.position = new Vector3(newPos.x, this.gameObject.transform.position.y, newPos.z);
        cameraTransform.localPosition = new Vector3(0,newPos.y,0);

        if ((newPos - resetPos).magnitude < 1f)
            cameraController.SetState<CameraInactiveState>();
    }
}

public class CameraInactiveState : State {
    GameManager gameManager;
    CameraController cameraController;

    public override void BeginState()
    {
        cameraController = CameraController.Instance;
        gameManager = GameManager.Instance;
    }
}