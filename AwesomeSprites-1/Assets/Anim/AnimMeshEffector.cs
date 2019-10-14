using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
namespace Anim
{

// 一个 effector 是一道从对象中心发出的波.
public class AnimMeshEffector : MonoBehaviour
{
    public float lifeTime;
    
    // 自从波发出之后, 经过了多少时间.
    public float time;
    
    public float amplitude;
    public float frequency;
    public float speed;
    
    [Tooltip("震荡曲线.")]
    public AnimationCurve curve;
    
    public Vector2 position => this.transform.position;
    
    protected void Start()
    {
        time = 0;
        foreach(var ef in Component.FindObjectsOfType<AnimEffectorReceiver>()) ef.effectors.Add(this);
    }
    
    protected void Update()
    {
        time += Time.deltaTime;
        if(time >= lifeTime) DestroyImmediate(this.gameObject);
    }
    
    protected void OnDestroy()
    {
        foreach(var ef in Component.FindObjectsOfType<AnimEffectorReceiver>()) ef.effectors.Remove(this);
    }
    
    public virtual Vector2 GetOffset(Vector2 worldPos)
    {
        // 波长 : waveLength = 1.0f / frequency.
        // 算出从开始到现在移动了多少个波长.
        var baseMove = speed * time * frequency - 1.0f;
        // 算出受影响点距离自己多少个波长.
        var x = (worldPos - this.transform.position.ToVec2()).magnitude * frequency;
        // 受影响点在波的哪个位置.
        var r = x - baseMove;
        // 如果当前波并没有影响到这点, 退出.
        if(r < 0 || r > 1) return Vector2.zero;
        
        // 波的影响方向.
        var dir = (worldPos - this.transform.position.ToVec2()).normalized;
        return dir * amplitude * curve.Evaluate(r);
    }
    
    protected virtual void OnDrawGizmos()
    {
        
    }
}


}
