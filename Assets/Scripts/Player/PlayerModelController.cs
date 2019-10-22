using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelController : MonoBehaviour
{
    public List<Light> lights;
    public List<GunComponent> guns;
    public PlayerModelEyeController eyeController;
    public PlayerController owner;
    public Color playerColor;

<<<<<<< HEAD

    private void Start()
    {
        SetPlayerColor(playerColor);
    }
=======
    void Awake() {
        guns = GetComponentsInChildren<GunComponent>().ToList();
    }

    /*
>>>>>>> 36cf176a5fe1085b6aecafa675bee494845e981c
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            setExpression(1);
        }
        if (Input.GetKeyDown("x"))
        {
            setExpression(2);
        }
        if (Input.GetKeyDown("c"))
        {
            setExpression(5);
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
    */

    public void SetPlayerColor(Color color)
    {
        foreach (Light currentLight in lights) {
            currentLight.color = color;
        }
        
        eyeController.SetEyeColor(color);
    }

    public void SetExpression(int expression)
    {
        switch (expression)
        {
            case 1:
                eyeController.SetExpressionWake();
                break;
            case 2:
                eyeController.SetExpressionDie();
                break;
            case 3:
                eyeController.SetExpressionAngry();
                break;
            case 4:
                eyeController.SetExpressionScared();
                break;
            case 5:
                eyeController.SetExpressionSquint();
                break;
            default:
                eyeController.SetExpressionNormal();
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