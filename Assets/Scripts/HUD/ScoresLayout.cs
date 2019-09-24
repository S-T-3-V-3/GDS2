using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresLayout : MonoBehaviour
{
    GridLayoutGroup glg;

    // Start is called before the first frame update
    void Start()
    {
        glg = gameObject.GetComponent<GridLayoutGroup>();
        //Debug.Log(glg);
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;  //**
        glg.constraintCount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
