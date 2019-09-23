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
        print("Awake!");
        StartCoroutine(AnimatedSequence(8, 1, false, 1));

        //eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 6));
    }

    public void SetExpressionDie()
    {
        //eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 7));
        print("Die!");
        StartCoroutine(AnimatedSequence(8,1,true,1));
    }

    public void SetExpressionAngry()
    {
        print("Angry!");
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * -1));
    }

    public void SetExpressionScared()
    {
        print("Scared");
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * -2));
    }
    public void SetExpressionNormal()
    {
        print("Normal");
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 0));
    }
    public void SetExpressionSquint()
    {
        print("Normal");
        eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * -3));
    }

    IEnumerator AnimatedSequence(int frameLength,int startFrame,bool reverse,float speed)
    {
        int direction;
        if (reverse) direction = -1;
        else direction = 1;

        print("coroutine");

        for (int currentFrame = startFrame; currentFrame <= frameLength; currentFrame++)
        {
            //Color c = renderer.material.color;
            //c.a = ft;
            //renderer.material.color = c;

            
            if (reverse)
            {
                eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * currentFrame* direction + (.0625f * frameLength)));
            }
            else
            {
                eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * currentFrame* direction));
            }

            print("anim frame " + currentFrame);

            yield return null;
        }
    }

}
