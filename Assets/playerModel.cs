using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerModel : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Color tempColor = colors[Random.Range(0,6)];
            SetColor(tempColor);
            eyeScript.SetEyeColor(tempColor);
        }


        if (Input.GetKeyDown("z"))
        {
            setExpression(Random.Range(0, 5));
            
        }

    }

    public void SetColor(Color color)
    {
        underLight.color = color;
        //eyeMat.color = new Color(0.5f, 0.2f, 0.3f);
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
            default:
                eyeScript.SetExpressionNormal();
                break;
        }
    }

}
