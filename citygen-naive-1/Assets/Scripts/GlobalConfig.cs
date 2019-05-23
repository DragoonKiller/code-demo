using System;
using UnityEngine;

public class GlobalConfig : MonoBehaviour
{
    public static GlobalConfig inst;
    
    public Gen.Config genConfig;
    public GenHighway.Config highwayConfig;
    public GenTown.Config townConfig;
    
    GlobalConfig() => inst = this;
    
    
}
