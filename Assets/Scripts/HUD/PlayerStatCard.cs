using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatCard : MonoBehaviour
{
    public List<GameObject> options;
    public PlayerController owner;
    public TextMeshProUGUI playerNumber;
    public Image readyImage;
    public TextMeshProUGUI readyText;
    [Space]
    public GameObject root;
    public Image portrait;
    [Space]
    public TextMeshProUGUI playerTeam;
    [Space]
    public TextMeshProUGUI playerWeapon;

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

        playerTeam.text = $"{teamID}"; 
        playerTeam.color = TeamManager.Instance.GetTeamColor(teamID);

        playerWeapon.text = $"{gameManager.gameSettings.guns[currentWeapon].gunName}";
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

    void Select() {/*
        options[currentSelection].transform.localScale = selectedScale;
        List<TextMeshProUGUI> arrows = options[currentSelection].GetComponentsInChildren<TextMeshProUGUI>().Where(x => x.name.ToLower().Contains("arrow")).ToList();

        foreach (TextMeshProUGUI arrow in arrows) {
            arrow.color = TeamManager.Instance.GetTeamColor(teamID);
        }*/
    }

    void DeSelect() {/*
        options[currentSelection].transform.localScale = defaultScale;
        List<TextMeshProUGUI> arrows = options[currentSelection].GetComponentsInChildren<TextMeshProUGUI>().Where(x => x.name.ToLower().Contains("arrow")).ToList();

        foreach (TextMeshProUGUI arrow in arrows) {
            arrow.color = Color.white;
        }*/
    }
}
