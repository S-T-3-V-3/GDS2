using UnityEngine;

public class NewGameOnClick : MonoBehaviour
{
    public void StartGame()
    {
        FindObjectOfType<GameManager>().LoadGame();
    }
}