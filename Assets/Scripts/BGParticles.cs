using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGParticles : MonoBehaviour
{
    public GameObject ParticleSystem;
    // Start is called before the first frame update
    public Color InitialColor;

    GameManager gameManager;
    List<GameObject> PSystems;

   

    void Start()
    {
        InvokeRepeating("RandomMenuColor", 12f, 16f);

        gameManager = FindObjectOfType<GameManager>();
        PSystems = new List<GameObject>();
        
        PSystems.Add(Instantiate(ParticleSystem, transform));
        var main = PSystems[0].GetComponent<ParticleSystem>().main;
        main.startColor = InitialColor;

        //GameManager.Instance.OnNewWinningTeam.AddListener(delegate{ChangeColor(GameManager.Instance.teamManager.GetWinningTeamColor()); });
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown("z"))
        {
            ChangeColor(new Color(Random.Range(0,2), Random.Range(0, 2), Random.Range(0, 2)));
        }
        */
    }

    void ChangeColor(Color color)
    {
        PSystems.Add(Instantiate(ParticleSystem, transform));
        var main = PSystems[PSystems.Count-1].GetComponent<ParticleSystem>().main;
        main.startColor = color;
        

        var oldParticle = PSystems[PSystems.Count - 2].GetComponent<ParticleSystem>().emission;
        oldParticle.enabled = false;

        Destroy(PSystems[PSystems.Count - 2],11);
        PSystems.Remove(PSystems[PSystems.Count - 2]);
    }

    void RandomMenuColor()
    {
        Color tempCol = TeamManager.Instance.GetTeamColors()[Random.Range(0, TeamManager.Instance.GetTeamColors().Count)];
        ChangeColor(tempCol);
    }


}
