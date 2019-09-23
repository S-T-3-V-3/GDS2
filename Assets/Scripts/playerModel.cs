using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    // Start is called before the first frame update
    Light underLight;
    playerModelEyes eyeScript;



    Color[] colors;
    void Start()
    {
        underLight = GetComponentInChildren<Light>();
        eyeScript = GetComponentInChildren<playerModelEyes>();

        colors = new Color[10];
        colors[0] = new Color(0.9f, 0, 0);
        colors[1] = new Color(0, 0.9f, 0);
        colors[2] = new Color(0, 0.2f, 1);
        colors[3] = new Color(1, 0.7f, 0);
        colors[4] = new Color(1, 0, 1);
        colors[5] = new Color(1, 1, 0);
        colors[6] = new Color(1, 1, 0);

    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Color tempColor = colors[Random.Range(0,6)];
            SetPlayerColor(tempColor);
            eyeScript.SetEyeColor(tempColor);
        }


        if (Input.GetKeyDown("z"))
        {
            setExpression(0);
        }
        if (Input.GetKeyDown("x"))
        {
            setExpression(1);
        }
        if (Input.GetKeyDown("c"))
        {
            setExpression(2);
        }
        if (Input.GetKeyDown("v"))
        {
            setExpression(3);
        }
        if (Input.GetKeyDown("b"))
        {
            setExpression(4);
        }
        if (Input.GetKeyDown("n"))
        {
            setExpression(5);
        }

    }

    public void SetPlayerColor(Color color)
    {
        underLight.color = color;
    }



    public void setExpression(int expression)
    {
        switch (expression)
        {
            case 1:
                eyeScript.SetExpressionWake();
                break;
            case 2:
                eyeScript.SetExpressionDie();
                break;
            case 3:
                eyeScript.SetExpressionAngry();
                break;
            case 4:
                eyeScript.SetExpressionScared();
                break;
            case 5:
                eyeScript.SetExpressionSquint();
                break;
            default:
                eyeScript.SetExpressionNormal();
                break;
        }
    }

    public void fireLeft()
    {

    }

    public void fireRight()
    {

    }
}
public enum Expression
{
    NORMAL,
    WAKE,
    DIE,
    ANGRY,
    SCARED,
    SQUINT
}