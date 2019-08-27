using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour {
    public int roundNumber = 0;
    public float elapsedTime = 0f;
    public bool roundIsStarted = false;
    GameManager gameManager;

    StateManager roundState;

    void Start() {
        gameManager = this.GetComponent<GameManager>();

        gameManager.sessionData.OnRoundComplete.AddListener(OnRoundComplete);

        this.SetState<RoundInctiveState>();
    }

    public void Reset() {
        roundNumber = 0;
        elapsedTime = 0f;
        roundIsStarted = false;
        this.SetState<RoundInctiveState>();
    }

    public void OnRoundComplete() {
        roundNumber++;
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
        gameManager = this.GetComponent<GameManager>();
        manager = this.GetComponent<RoundManager>();

        manager.roundIsStarted = true;
        manager.elapsedTime = 0f;
        gameManager.OnNewCameraTarget.Invoke();

        gameManager.sessionData.OnRoundBegin.Invoke();
        gameManager.hud.Announcement("GO!",2);

        gameManager.sessionData.StartSession();
    }

    void Update() {
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
        gameManager = this.GetComponent<GameManager>();
        manager = this.GetComponent<RoundManager>();

        manager.roundIsStarted = false;
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
        gameManager = this.GetComponent<GameManager>();
        manager = this.GetComponent<RoundManager>();

        manager.roundIsStarted = false;

        gameManager.sessionData.StopRound();
        gameManager.sessionData.OnRoundComplete.Invoke();

        if (manager.roundNumber == gameManager.gameSettings.numRounds) {
            gameManager.hud.Announcement("Game Over!",transitionTime);
        }
    }

    void Update() {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= transitionTime) {
            if (manager.roundNumber < gameManager.gameSettings.numRounds)
                manager.SetState<CarouselRoundState>();
            else {
                gameManager.Reset();
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
    bool isActive = false;

    public override void BeginState()
    {
        gameManager = this.GetComponent<GameManager>();
        manager = this.GetComponent<RoundManager>();

        manager.roundIsStarted = false;
        gameManager.hud.RoundTimer.gameObject.SetActive(true);

        gameManager.sessionData.OnRoundPrepare.Invoke();

        gameManager.OnMapLoaded.AddListener(() => {isActive = true;});
    }

    void Update() {
        if (!isActive) return;

        elapsedTime -= Time.deltaTime;

        if (elapsedTime <= 0) {            
            elapsedTime = 1f;

            if (countDown <= 0)
                manager.SetState<RoundActiveState>();
            else {
                gameManager.hud.Announcement(countDown.ToString(),1);
            }
            countDown--;
        }
    }
}

public class CarouselRoundState : State
{
    GameManager gameManager;
    RoundManager manager;

    public override void BeginState()
    {
        gameManager = this.GetComponent<GameManager>();
        manager = this.GetComponent<RoundManager>();

        gameManager.UnloadMap();

        foreach(PlayerController player in gameManager.currentPlayers) {
            player.SetState<BuffSelectState>();
        }

        gameManager.sessionData.OnCarouselBegin.Invoke();
    }

    public override void EndState() {
        gameManager.sessionData.OnCarouselEnd.Invoke();
    }
}