using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
namespace Anim
{

public class AnimMeshEffectorCreator : MonoBehaviour
{
    public AnimMeshEffector effectorSource;
    public float lifeTime;
    public Vector2 amplitude;
    public Vector2 frequency;
    public Vector2 speed;
    public AnimationCurve curve;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var g = Instantiate(effectorSource.gameObject).GetComponent<AnimMeshEffector>();
            g.transform.position = ExCursor.worldPos;
            g.lifeTime = lifeTime;
            g.amplitude = Random.Range(amplitude.x, amplitude.y);
            g.frequency = Random.Range(frequency.x, frequency.y);
            g.speed = Random.Range(speed.x, speed.y);
            g.curve = curve;
        }
    }
}

}
