using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDScript : MonoBehaviour
{

    public GameObject SpawnExplosionFX;
    public Color FXColor;
    bool doOnce;
    // Start is called before the first frame update
    void Start()
    {
        PlayerParticle test = SpawnExplosionFX.GetComponent<PlayerParticle>();
        test.SetColor(FXColor);
    }

    // Update is called once per frame
    void Update()
    {
        //print(transform.position.y);
        if(!doOnce && transform.position.y <0)
        {
            doOnce = !doOnce;
            Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
            
            GameObject test = Instantiate(SpawnExplosionFX, pos, new Quaternion());
            test.transform.localScale = new Vector3(3, 3, 3);
        }
            
    }

    //void OnTriggerEnter(Collider collision)
    //{
    //    
    //    
    //    print("WTFDAFADF");
    //}
}
