using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    public GameObject SpawnFX;
    public GameObject DeathFX;


    Color[] colors;
    // Start is called before the first frame update
    void Start()
    {
        colors = new Color[10];
        colors[0] = new Color(0.9f, 0, 0);
        colors[1] = new Color(0, 0.9f, 0);
        colors[2] = new Color(0, 0.2f, 1);
        colors[3] = new Color(1, 0.7f, 0);
        colors[4] = new Color(1, 0, 1);
        colors[5] = new Color(1, 1, 0);
        colors[6] = new Color(1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t"))
        {

            InstantiateFX(SpawnFX, 2);

        }
        if (Input.GetKeyDown("y"))
        {

            InstantiateFX(DeathFX, 3);

        }

    }

    void InstantiateFX(GameObject obj, float time)
    {
        Color tempColor = colors[Random.Range(0, 6)];
        GameObject tempObj = Instantiate(obj, transform);

        tempObj.transform.position = new Vector3(Random.Range(0, 12), 0, Random.Range(0, 12));
        tempObj.GetComponent<setColor>().updateColors(tempColor);

        Destroy(tempObj, time);
        print("TESTTT");
    }
}
