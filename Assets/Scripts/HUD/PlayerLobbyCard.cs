using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLobbyCard : MonoBehaviour
{
    public List<GameObject> options;
    public PlayerController owner;
    public TextMeshProUGUI playerNumber;
    public Image readyImage;
    public TextMeshProUGUI readyText;
    [Space]
    public Image portrait;
    [Space]
    public RawImage playerTeam;
    [Space]
    public Image playerWeapon;
    [Space]
    public Image modelBubble, teamBubble, equipBubble;
    [Space]
    public Animator bubbleAnimator;


    GameManager gameManager;
    int currentSelection = 0;
    int numOptions = 3;

    TeamID teamID;
    int currentModel = 0;
    int currentWeapon = 0;

    Vector3 defaultScale = new Vector3(1f,1f,1f);
    Vector3 selectedScale = new Vector3(1.2f,1.2f,1.2f);

    public void Init() {
        gameManager = GameManager.Instance;

        portrait.sprite = gameManager.gameSettings.characterPortraits[currentModel];
        portrait.color = TeamManager.Instance.GetTeamColor(teamID);

        playerTeam.color = TeamManager.Instance.GetTeamColor(teamID);

        //playerWeapon.text = $"{gameManager.gameSettings.guns[currentWeapon].gunName}";
        playerWeapon.sprite = gameManager.gameSettings.weaponIcons[currentWeapon];
        playerNumber.text = "Player " + owner.playerID;

        Select();
    }

    public void Ready() {
        if (teamID == TeamID.NONE) return;

        owner.ready = !owner.ready;

        if (owner.ready)
        {
            DeSelect();
            
            owner.playerModelSelection = currentModel;
            owner.playerWeaponSelection = currentWeapon;
            owner.teamID = teamID;

            readyImage.gameObject.SetActive(true);
            readyText.color = TeamManager.Instance.GetTeamColor(owner.teamID);

            gameManager.ReadyCheck();
        }
        else {
            Select();
            readyImage.gameObject.SetActive(false);
        }

        gameManager.OnPlayersChanged.Invoke();
    }

    public void Up() {
        DeSelect();
        if (currentSelection == 0) {
            currentSelection = numOptions-1;
        }
        else {
            currentSelection--;
        }
        Select();
    }

    public void Down() {
        DeSelect();               
        if (currentSelection == numOptions-1) {
            currentSelection = 0;
        }
        else {
            currentSelection++;
        }
        Select();
    }

    public void Left() {
        DeSelect();
        Select();
        bubbleAnimator.Play("SelectBubbleClick");
        if (currentSelection == 0)
        {
            if (currentModel == 0)
                currentModel = gameManager.gameSettings.characterModels.Count - 1;
            else
                currentModel--;
            portrait.sprite = gameManager.gameSettings.characterPortraits[currentModel];
        }
        else if (currentSelection == 1)
        {
            if (teamID == 0)
                teamID = (TeamID)gameManager.teamSettings.teams.Count - 1;
            else
                teamID--;
            portrait.color = TeamManager.Instance.GetTeamColor(teamID);
            playerTeam.color = TeamManager.Instance.GetTeamColor(teamID);
        }
        else if (currentSelection == 2)
        {
            if (currentWeapon == 0)
                currentWeapon = gameManager.gameSettings.guns.Count - 1;
            else
                currentWeapon--;
            playerWeapon.sprite = gameManager.gameSettings.weaponIcons[currentWeapon];
        }
    }

    public void Right() {
        DeSelect();
        Select();
        bubbleAnimator.Play("SelectBubbleClick");
        if (currentSelection == 0)
        {
            if (currentModel < gameManager.gameSettings.characterModels.Count - 1)
                currentModel++;
            else
                currentModel = 0;

            portrait.sprite = gameManager.gameSettings.characterPortraits[currentModel];
        }
        else if (currentSelection == 1)
        {
            if (teamID < (TeamID)gameManager.teamSettings.teams.Count - 1)
                teamID++;
            else
                teamID = 0;

            portrait.color = TeamManager.Instance.GetTeamColor(teamID);
            playerTeam.color = TeamManager.Instance.GetTeamColor(teamID);

            Select();
        }
        else if (currentSelection == 2)
        {
            if (currentWeapon < gameManager.gameSettings.guns.Count - 1)
                currentWeapon++;
            else
                currentWeapon = 0;

            playerWeapon.sprite = gameManager.gameSettings.weaponIcons[currentWeapon];
        }
    }

    void Select() {
        if (currentSelection == 0)
        {
            modelBubble.gameObject.SetActive(true);
            bubbleAnimator = modelBubble.GetComponent<Animator>();
        }
        else if (currentSelection == 1)
        {
            teamBubble.gameObject.SetActive(true);
            bubbleAnimator = teamBubble.GetComponent<Animator>();
        }
        else if (currentSelection == 2)
        {
            equipBubble.gameObject.SetActive(true);
            bubbleAnimator = equipBubble.GetComponent<Animator>();
        }
        bubbleAnimator.Play("SelectBubbleIdle");
        //bubbleAnimator.ResetTrigger("SelectbubbleClick");
    }

    void DeSelect() {
        //Destroy(bubbleAnimator);
        modelBubble.gameObject.SetActive(false);
        teamBubble.gameObject.SetActive(false);
        equipBubble.gameObject.SetActive(false);
    }

}
