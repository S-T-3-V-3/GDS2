using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour {
    public int roundNumber = 0;
    public float elapsedTime = 0f;
    public bool isStarted = false;
    GameManager gameManager;

    StateManager roundState;

    void Start() {
        gameManager = GameManager.Instance;

        gameManager.sessionData.OnRoundComplete.AddListener(OnRoundComplete);

        this.SetState<RoundInctiveState>();
    }

    public void Reset() {
        roundNumber = 0;
        elapsedTime = 0f;
        isStarted = false;
        this.SetState<RoundInctiveState>();
    }

    public void OnRoundComplete() {
        roundNumber++;
        SoundManager.Instance.Play("round over");
    }

    public void SetState<T>() where T : State {
        if (roundState == null)
            roundState = this.gameObject.AddComponent<StateManager>();

        roundState.AddState<T>();
    }
}

public class RoundActiveState : State
{
    GameManager gameManager;
    RoundManager manager;

    public override void BeginState()
    {
        gameManager = GameManager.Instance;
        manager = this.GetComponent<RoundManager>();

        manager.isStarted = true;
        manager.elapsedTime = 0f;
        gameManager.OnNewCameraTarget.Invoke();

        CameraController.Instance.SetState<CameraActiveState>();

        gameManager.sessionData.OnRoundBegin.Invoke();
        gameManager.hud.Announcement("GO!",2);

        gameManager.sessionData.StartSession();
    }

    void Update() {
        if (GameManager.Instance.sessionData.isPaused) return;

        manager.elapsedTime += Time.deltaTime;

        if (manager.elapsedTime > gameManager.gameSettings.roundTime) {
            manager.SetState<RoundCompleteState>();
        }
    }
}

public class RoundInctiveState : State
{
    GameManager gameManager;
    RoundManager manager;

    public override void BeginState()
    {
        gameManager = GameManager.Instance;
        manager = this.GetComponent<RoundManager>();

        manager.isStarted = false;
    }
}

public class RoundCompleteState : State
{
    GameManager gameManager;
    RoundManager manager;
    
    float transitionTime = 3f;
    float timeElapsed = 0f;

    public override void BeginState()
    {
        gameManager = GameManager.Instance;
        manager = this.GetComponent<RoundManager>();

        SoundManager.Instance.Play("round over");

        manager.isStarted = false;

        gameManager.sessionData.StopRound();
        gameManager.sessionData.OnRoundComplete.Invoke();

        CameraController.Instance.SetState<CameraResetState>();

        if (manager.roundNumber == gameManager.gameSettings.numRounds) {
            gameManager.hud.Announcement("Game Over!",transitionTime);
        }
    }

    void Update() {
        if (GameManager.Instance.sessionData.isPaused) return;

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= transitionTime) {
            if (manager.roundNumber < gameManager.gameSettings.numRounds)
                manager.SetState<LoadoutRoundState>();
            else {
                manager.SetState<EndGameState>();
            }
        }
    }
}

public class RoundCountdownState : State
{
    GameManager gameManager;
    RoundManager manager;

    int countDown = 3;
    float elapsedTime = 0f;

    public override void BeginState()
    {
        gameManager = GameManager.Instance;
        manager = this.GetComponent<RoundManager>();

        manager.isStarted = false;
        gameManager.hud.RoundTimer.gameObject.SetActive(true);

        CameraController.Instance.SetState<CameraResetState>();
    }

    void Update() {
        if (GameManager.Instance.sessionData.isPaused) return;

        elapsedTime -= Time.deltaTime;

        if (elapsedTime <= 0) {            
            elapsedTime = 1f;

            if (countDown <= 0)
                manager.SetState<RoundActiveState>();
            else {
                gameManager.hud.Announcement(countDown.ToString(),1);

                if (countDown == 3)
                    SoundManager.Instance.Play("countdown");
            }
            countDown--;
        }
    }
}

public class LoadoutRoundState : State
{
    GameManager gameManager;
    RoundManager manager;

    public override void BeginState()
    {
        gameManager = GameManager.Instance;
        manager = this.GetComponent<RoundManager>();

        gameManager.UnloadMap();

        gameManager.sessionData.OnLoadoutBegin.Invoke();

        foreach(PlayerController player in gameManager.currentPlayers) {
            if (player.isPlaying) player.SetState<LoadoutState>();
        }
    }

    public override void EndState() {
        gameManager.sessionData.OnLoadoutEnd.Invoke();
    }
}

public class EndGameState : State
{
    GameManager gameManager;
    RoundManager manager;

    public override void BeginState()
    {
        gameManager = GameManager.Instance;
        manager = this.GetComponent<RoundManager>();

        gameManager.UnloadMap();

        gameManager.sessionData.isStarted = false;
        gameManager.sessionData.isComplete = true;
        manager.isStarted = false;

        gameManager.sessionData.OnEndGame.Invoke();

        EndScreen endScreen = FindObjectOfType<EndScreen>();
        endScreen.SetWinner(gameManager.sessionData.score.winningTeam.teamID);

        foreach(PlayerController player in gameManager.currentPlayers) {
            if (player.isPlaying) player.SetState<EndGamePlayerState>();
        }
    }
}