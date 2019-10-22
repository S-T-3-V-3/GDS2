using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI title;

    public void SetWinner(TeamID team) {
        title.text = $"<color={team.ToString().ToLower()}>{team}</color> TEAM WINS!";
    }
}
