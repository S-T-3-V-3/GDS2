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
    public GameObject root;
    public Image portrait;
    public TextMeshProUGUI portraitLeftArrow;
    public TextMeshProUGUI portraitRightArrow;
    [Space]
    public TextMeshProUGUI playerTeam;
    public TextMeshProUGUI teamLeftArrow;
    public TextMeshProUGUI teamRightArrow;
    [Space]
    public TextMeshProUGUI playerWeapon;
    public TextMeshProUGUI weaponLeftArrow;
    public TextMeshProUGUI weaponRightArrow;

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

        playerTeam.text = $"{teamID}"; 
        playerTeam.color = TeamManager.Instance.GetTeamColor(teamID);

        playerWeapon.text = $"{gameManager.gameSettings.guns[currentWeapon].gunName}";
        playerNumber.text = "Player " + (gameManager.currentPlayers.IndexOf(owner) + 1);

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

        if (currentSelection > 0) {
            currentSelection--;
        }
        else {
            currentSelection = numOptions - 1;
        }

        Select();
    }

    public void Down() {
        DeSelect();
               
        if (currentSelection < numOptions - 1) {
            currentSelection++;
        }
        else {
            currentSelection = 0;
        }

        Select();
    }

    public void Left() {
        if (currentSelection == 0) {
            if (currentModel > 0)
                currentModel--;
            else
                currentModel = gameManager.gameSettings.characterModels.Count - 1;

            portrait.sprite = gameManager.gameSettings.characterPortraits[currentModel];
        }
        else if (currentSelection == 1) {
            if (teamID > 0)
                teamID--;
            else
                teamID = (TeamID)gameManager.teamSettings.teams.Count - 1;

            portrait.color = TeamManager.Instance.GetTeamColor(teamID);
            playerTeam.text = $"{teamID}"; 
            playerTeam.color = TeamManager.Instance.GetTeamColor(teamID);

            Select();
        }
        else if (currentSelection == 2) {
            if (currentWeapon > 0)
                currentWeapon--;
            else
                currentWeapon = gameManager.gameSettings.guns.Count - 1;

            playerWeapon.text = $"{gameManager.gameSettings.guns[currentWeapon].gunName}";
        }
    }

    public void Right() {
        if (currentSelection == 0) {
            if (currentModel < gameManager.gameSettings.characterModels.Count - 1)
                currentModel++;
            else
                currentModel = 0;

            portrait.sprite = gameManager.gameSettings.characterPortraits[currentModel];
        }
        else if (currentSelection == 1) {
            if (teamID < (TeamID)gameManager.teamSettings.teams.Count - 1)
                teamID++;
            else
                teamID = 0;

            portrait.color = TeamManager.Instance.GetTeamColor(teamID);
            playerTeam.text = $"{teamID}"; 
            playerTeam.color = TeamManager.Instance.GetTeamColor(teamID);

            Select();
        }
        else if (currentSelection == 2) {
            if (currentWeapon < gameManager.gameSettings.guns.Count - 1)
                currentWeapon++;
            else
                currentWeapon = 0;
                
            playerWeapon.text = $"{gameManager.gameSettings.guns[currentWeapon].gunName}";
        }
    }

    void Select() {
        options[currentSelection].transform.localScale = selectedScale;
        List<TextMeshProUGUI> arrows = options[currentSelection].GetComponentsInChildren<TextMeshProUGUI>().Where(x => x.name.ToLower().Contains("arrow")).ToList();

        foreach (TextMeshProUGUI arrow in arrows) {
            arrow.color = TeamManager.Instance.GetTeamColor(teamID);
        }
    }

    void DeSelect() {
        options[currentSelection].transform.localScale = defaultScale;

        List<TextMeshProUGUI> arrows = options[currentSelection].GetComponentsInChildren<TextMeshProUGUI>().Where(x => x.name.ToLower().Contains("arrow")).ToList();

        foreach (TextMeshProUGUI arrow in arrows) {
            arrow.color = Color.white;
        }
    }
}
