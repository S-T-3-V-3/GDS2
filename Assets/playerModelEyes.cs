using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerModelEyes : MonoBehaviour
{
    // Start is called before the first frame update
    Material eyeMat;
    Renderer eyeRenderer;
    Color currentColor;

    float eyeFrame;
    float frameSize;

    void Start()
    {
        eyeRenderer = gameObject.GetComponent<Renderer>();

        eyeFrame = 0;

        frameSize = 1 / 16;
    }

    // Update is called once per frame
    void Update()
    {
        //myRend.material.color = currentColor;

        //myRend.material.mainTextureOffset = new Vector2(Time.time, Time.time*10);
        eyeFrame+=.0625f;
        //print (eyeFrame);
        //eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, eyeFrame));
    }
    public void SetEyeColor(Color color)
    {
        currentColor = color;
        print(color);
        print(eyeRenderer.name);
        //myRend.material.shader = Shader.Find("HDRP/Lit");
        eyeRenderer.material.SetColor("_BaseColor", color);


        //myRend.material.mainTextureOffset = new Vector2(.1f, 0);

    }
    public void SetExpressionWake()
    {
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 6));
    }

    public void SetExpressionDie()
    {
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 7));
    }

    public void SetExpressionAngry()
    {
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 2));
    }

    public void SetExpressionScared()
    {
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 4));
    }
    public void SetExpressionNormal()
    {
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 8));
    }

}
