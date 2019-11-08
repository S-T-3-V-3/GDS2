using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DigitalRuby.Tween;

public class MapAnimator : MonoBehaviour
{
    public UnityEvent OnComplete = new UnityEvent();
    float minHeight = 9f;
    float maxHeight = 25f;
    float minTime = 0.25f;
    float maxTime = 3f;

    int numTiles;
    int numCompleted = 0;

    MapManager mapManager;

    List<Tile> allTiles;

    List<FloatTween> tweens;

    void Start()
    {
        allTiles = new List<Tile>();
        tweens = new List<FloatTween>();

        mapManager = GameManager.Instance.mapManager;
        allTiles.AddRange(mapManager.tiles);
        allTiles.AddRange(mapManager.walls);

        foreach (Tile t in allTiles) {
            float start = UnityEngine.Random.Range(minHeight,maxHeight);
            float end = t.isWall ? t.wallOffset.y : 0;
            float duration = UnityEngine.Random.Range(minTime,maxTime);

            t.transform.localPosition = new Vector3(t.transform.position.x, start, t.transform.position.z);
            
            TweenFactory.Tween(t.gameObject,start,end,duration,TweenScaleFunctions.QuinticEaseIn,
            (ITween<float> x) => {
                t.transform.localPosition = new Vector3(t.transform.localPosition.x, x.CurrentValue, t.transform.position.z);
            },(ITween<float> x) => {
                numCompleted++;
            });
        }

        numTiles = allTiles.Count;
    }

    void Update() {
        if (numCompleted >= numTiles)
            Completed();
    }

    void Completed() {
        OnComplete.Invoke();
        Destroy(this);
    }
}
