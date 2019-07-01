using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGen : MonoBehaviour
{
    public float roundTimeCount;
    public float ballsPerRound;
    public GameObject ball;

    [Header("Info")]
    public float totalGen;

    public BoxCollider2D col => this.GetComponent<BoxCollider2D>();
    public Rect range => new Rect(col.offset - col.size * 0.5f + (Vector2)this.transform.position, col.size);

    const int maxGenCount = 100; // The max generation count for a frame.

    float t;

    void Start()
    {
        t = roundTimeCount;
    }

    void Update()
    {
        t -= Time.deltaTime;
        int genCount = 0;
        while(t <= 0f)
        {
            if(genCount > maxGenCount) break;
            t += roundTimeCount;
            for(int i = 0; i < ballsPerRound; i++)
            {
                float x = Random.Range(range.xMin, range.xMax);
                float y = Random.Range(range.yMin, range.yMax);
                var g = Instantiate(ball, new Vector2(x, y), Quaternion.identity);
                genCount += 1;
                totalGen += 1;
            }
        }
    }
}
