using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScorecard : MonoBehaviour
{
    public Image playerPortrait;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI teamText;
    public TextMeshProUGUI pointsText;
    public RawImage playerHP;
    public TextMeshProUGUI playerHPText;
    public int model;
    public int playerNumber;
    public TeamID team;
    
    public void Init() {
        GameManager gameManager = GameManager.Instance;

        Color teamColor = TeamManager.Instance.GetTeamColor(team);
        playerPortrait.color = new Color(teamColor.r,teamColor.g,teamColor.b,0.5f);

        playerPortrait.sprite = gameManager.gameSettings.characterPortraits[model];
        playerText.text = "Player " + playerNumber;

        teamText.text = "" + team;
        teamText.color = TeamManager.Instance.GetTeamColor(team);
        pointsText.color = TeamManager.Instance.GetTeamColor(team);
        
        playerHP.uvRect = new Rect(0, 0, 1, 1);
        transform.localScale = Vector3.one;
    }
    
}
