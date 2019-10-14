using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anim
{

public class AnimEffectorReceiver : MonoBehaviour
{
    public readonly HashSet<AnimMeshEffector> effectors = new HashSet<AnimMeshEffector>();
}

}
