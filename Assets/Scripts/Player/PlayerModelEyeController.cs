using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelEyeController : MonoBehaviour
{
    Material eyeMat;
    public Renderer modelRenderer;
    public Color currentColor;
    float eyeFrame;
    //float frameSize;

    void Awake()
    {
        //if (this.GetComponent<MeshRenderer>() != null)
        //   modelRenderer = this.GetComponent<MeshRenderer>();
        // else
        //   modelRenderer = this.GetComponent<SkinnedMeshRenderer>();
        //    
        // eyeFrame = 0;
        //frameSize = 1 / 16;
        
    }
    void Start()
    {
        SetEyeColor(currentColor);
        //setExpression(1);
        
    }

    private void OnEnable()
    {
        Invoke("SetExpressionWake", 0);
        Invoke("SetExpressionSquint", 4);
        Invoke("SetExpressionAngry", 6);
    }
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

    public void SetEyeColor(Color color)
    {
        currentColor = color;
        //myRend.material.shader = Shader.Find("HDRP/Lit");
        try {
            modelRenderer.materials[1].SetColor("_BaseColor", color);
        }
        catch {

        }


        //myRend.material.mainTextureOffset = new Vector2(.1f, 0);

    }
    public void SetExpressionWake()
    {
        print("Awake!");
        StartCoroutine(AnimatedSequence(8, 1, true, 1));

        //eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 6));
    }

    public void SetExpressionDie()
    {
        //eyeRenderer.materials[0].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 7));
        print("Die!");
        StartCoroutine(AnimatedSequence(8,1,false,1));
    }

    public void SetExpressionAngry()
    {
        print("Angry!");
        modelRenderer.materials[1].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * -1));
    }

    public void SetExpressionScared()
    {
        print("Scared");
        modelRenderer.materials[1].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * -2));
    }
    public void SetExpressionNormal()
    {
        print("Normal");
        modelRenderer.materials[1].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * 0));
    }
    public void SetExpressionSquint()
    {
        print("Normal");
        modelRenderer.materials[1].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * -3));
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
                modelRenderer.materials[1].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * currentFrame* direction + (.0625f * frameLength)));
            }
            else
            {
                modelRenderer.materials[1].SetTextureOffset("_BaseColorMap", new Vector2(0, .0625f * currentFrame* direction));
            }

            print("anim frame " + currentFrame);

            yield return new WaitForSeconds(0.05f);
        }
    }
    public void setExpression(int expression)
    {
        switch (expression)
        {
            case 1:
                SetExpressionWake();
                break;
            case 2:
                SetExpressionDie();
                break;
            case 3:
                SetExpressionAngry();
                break;
            case 4:
                SetExpressionScared();
                break;
            case 5:
                SetExpressionSquint();
                break;
            default:
                SetExpressionNormal();
                break;
        }
    }

}
