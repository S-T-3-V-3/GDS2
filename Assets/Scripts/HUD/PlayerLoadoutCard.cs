using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLoadoutCard : MonoBehaviour
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
    public Image equipBubble;
    [Space]
    public Animator bubbleAnimator;


    GameManager gameManager;
    int currentSelection = 0;
    int numOptions = 1;

    TeamID teamID;
    int currentModel = 0;
    int currentWeapon = 0;

    Vector3 defaultScale = new Vector3(1f,1f,1f);
    Vector3 selectedScale = new Vector3(1.2f,1.2f,1.2f);

    public void Init() {
        gameManager = GameManager.Instance;

        currentModel = owner.playerModelSelection;
        currentWeapon = owner.playerWeaponSelection;
        teamID = owner.teamID;

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

        Select();
    }

    public void Down() {
        DeSelect();               

        Select();
    }

    public void Left() {
        DeSelect();
        Select();
        bubbleAnimator.Play("SelectBubbleClick");

        if (currentWeapon == 0)
            currentWeapon = gameManager.gameSettings.guns.Count - 1;
        else
            currentWeapon--;

        playerWeapon.sprite = gameManager.gameSettings.weaponIcons[currentWeapon];
    }

    public void Right() {
        DeSelect();
        Select();
        bubbleAnimator.Play("SelectBubbleClick");

        if (currentWeapon < gameManager.gameSettings.guns.Count - 1)
            currentWeapon++;
        else
            currentWeapon = 0;

        playerWeapon.sprite = gameManager.gameSettings.weaponIcons[currentWeapon];
    }

    void Select() {
        equipBubble.gameObject.SetActive(true);
        bubbleAnimator = equipBubble.GetComponent<Animator>();
        bubbleAnimator.Play("SelectBubbleIdle");
        //bubbleAnimator.ResetTrigger("SelectbubbleClick");
    }

    void DeSelect() {
        //Destroy(bubbleAnimator);
        equipBubble.gameObject.SetActive(false);
    }

}
