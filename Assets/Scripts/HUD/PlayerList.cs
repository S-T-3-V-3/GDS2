using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour
{
    PlayerLobbyCard plp;
    List<GameObject> playerCardList;
    Color tempcolor;
    GameManager gameManager;

    void Awake() {
        playerCardList = new List<GameObject>();

        gameManager = GameManager.Instance;
    }

    public void Reset() {
        if (playerCardList == null)
            playerCardList = new List<GameObject>();
        else {
            foreach(GameObject currentCard in playerCardList) {
                GameObject.Destroy(currentCard);
                playerCardList.Clear();
            }
        }
    }

    public bool HasPlayer(PlayerController pc) {
        return playerCardList.Where(x => x.GetComponent<PlayerLobbyCard>().owner == pc).Count() > 0;
    }

    public GameObject AddPlayer(PlayerController pc, CardType type) {
        GameObject card;

        if (type == CardType.LOADOUT) {
            card = GameObject.Instantiate(gameManager.hud.LoadoutCardPrefab,this.transform);
            card.GetComponent<PlayerLoadoutCard>().owner = pc;
            card.GetComponent<PlayerLoadoutCard>().Init();
        }
        else if (type == CardType.LOBBY) {
            card = GameObject.Instantiate(gameManager.hud.LobbyCardPrefab,this.transform);
            card.GetComponent<PlayerLobbyCard>().owner = pc;
            card.GetComponent<PlayerLobbyCard>().Init();
        }
        else {
            card = GameObject.Instantiate(gameManager.hud.StatCardPrefab,this.transform);
            card.GetComponent<PlayerStatCard>().owner = pc;
            card.GetComponent<PlayerStatCard>().Init();
        }

        playerCardList.Add(card);

        return card;
    }

    public void RemovePlayer(PlayerController pc) {
        GameObject playerCard = playerCardList.Where(x => x.GetComponent<PlayerLobbyCard>().owner == pc).First();

        if (playerCard == null)
            playerCard = playerCardList.Where(x => x.GetComponent<PlayerLoadoutCard>().owner == pc).First();
            
        playerCardList.Remove(playerCard);
        GameObject.Destroy(playerCard);
    }
}

public enum CardType {
    LOBBY,
    LOADOUT,
    STAT
}