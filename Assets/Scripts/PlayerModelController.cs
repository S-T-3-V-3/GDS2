using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelController : MonoBehaviour
{
    public GameObject SpawnFX;
    public GameObject DeathFX;

    GameManager gameManager;
    PlayerController playerController;

    Color color;
    // Start is called before the first frame update
    void Start()
    {

        gameManager = FindObjectOfType<GameManager>();
        playerController = GetComponent<PlayerController>();

        color = gameManager.teamManager.GetTeam(playerController.teamID).color;
        print(color);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {

            Spawn(color, transform);

        }
        if (Input.GetKeyDown("y"))
        {

            Death(color, transform);

        }

    }
    public void Spawn(Color col, Transform position)
    {
        color = col;
        InstantiateFX(SpawnFX, 2, position);
    } 
    public void Death(Color col, Transform position)
    {
        InstantiateFX(DeathFX, 3, position);
    }



    void InstantiateFX(GameObject obj, float duration, Transform position)
    {
        
        GameObject tempObj = Instantiate(obj, position.position, position.rotation);

        tempObj.transform.position = new Vector3(Random.Range(0, 12), 0, Random.Range(0, 12));
        tempObj.GetComponent<setColor>().updateColors(color);

        Destroy(tempObj, duration);
        print("TESTTT");
    }
}
