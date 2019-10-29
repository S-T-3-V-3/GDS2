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
    public Image selectionBubble;
    public Image readyImage;
    public TextMeshProUGUI readyText;
    [Space]
    public TextMeshProUGUI playerNumber;
    public Image portrait;
    public RawImage playerTeam;
    public Image playerWeapon;


    GameManager gameManager;
    int currentSelection = 0;
    int numOptions;

    TeamID teamID;
    int currentModel;
    int currentWeapon;

    Vector3 defaultScale = new Vector3(1f,1f,1f);
    Vector3 selectedScale = new Vector3(1.2f,1.2f,1.2f);

    public void Init() {
        gameManager = GameManager.Instance;

        numOptions = options.Count;

        currentWeapon = owner.playerWeaponSelection;
        currentModel = owner.playerModelSelection;
        teamID = owner.teamID;

        portrait.sprite = gameManager.gameSettings.characterPortraits[currentModel];
        portrait.color = TeamManager.Instance.GetTeamColor(teamID);

        //playerTeam.text = $"{teamID}"; 
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

        /*
        if (currentSelection > 0) {
            currentSelection = 0;
        }
        else {
            currentSelection = numOptions - 1;
        }
        */

        Select();
    }

    public void Down() {
        DeSelect();
        
        /*
        if(currentSelection == 0)
        {
            currentSelection = 1;
        }
        else {
            currentSelection = 0;
        }
        */

        Select();
    }

    public void Left() {
        DeSelect();
        /*
        if(currentSelection > 0)
        {
            if (currentSelection == 1) currentSelection = 2;
            else if (currentSelection == 2) currentSelection = 1;
        }
        */
        Select();

    }

    public void Right() {
        DeSelect();

        /*
        if (currentSelection > 0)
        {
            if (currentSelection == 1) currentSelection = 2;
            else if (currentSelection == 2) currentSelection = 1;
        }
        Select();
        */

        /*
        if (currentSelection == 0) {
            if (currentWeapon < gameManager.gameSettings.guns.Count - 1)
                currentWeapon++;
            else
                currentWeapon = 0;
                
            //playerWeapon.text = $"{gameManager.gameSettings.guns[currentWeapon].gunName}";
        }
        */
    }

    public void Confirm()
    {
            if (currentWeapon < gameManager.gameSettings.guns.Count - 1)
                currentWeapon++;
            else
                currentWeapon = 0;

    }

    void Select() {
        selectionBubble.gameObject.SetActive(true);
        //Make SelectionBubble a child of the selected option
        //Stretch Position/Pivot to fit parent
        selectionBubble.transform.SetParent(options[currentSelection].transform);       
        selectionBubble.transform.position = options[currentSelection].transform.position;
        selectionBubble.rectTransform.anchorMin = new Vector2(0, 0);
        selectionBubble.rectTransform.anchorMax = new Vector2(1, 1);
        selectionBubble.rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    void DeSelect() {
        selectionBubble.gameObject.SetActive(false);
    }

}
