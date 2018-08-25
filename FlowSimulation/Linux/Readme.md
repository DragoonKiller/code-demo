# Flow Simulation (Experimental)

## What's this?

An experiment that does:
1. Dynamically load and unload library in Unity. Notice The P/Invoke does not support unloading, especially as Unity holds all compiled objects preserved and not re-generate when game starts.
1. Use CUDA toolchain.
1. Profile a CUDA program with unity.

This is a program that simulate particles (not particle system, anyway) with their force interation.

## How to have a try?

1. `cd Environment`
1. `make`
1. Open Unity and run the scene in editor.

## Other useful information?

In this experiment I've tried to calculate particles' movement by a naive algorithm:
```cpp
for(int i=0; i<n; i++)
{
    Vector2 force = {0, 0};
    for(int j=0; j<n; j++)
    {
        if(i != j)
        {
            force += CalcForce(particles[i].location, particles[j].location);
        }
    }
    
    Vector2 acceleration = force / particles[i].mass;
    particles[i].velocity += acceleration * deltaTime;
    particles[i].location += particles[i].velocity * deltaTime + 0.5 * acceleration * deltaTime^2;
}
```

This is `O(n^2)` where n indicates number of particles. This is test result on DK's Computer with I7-7700K and GTX 1060.

|impl.|iteration per frame|ms per frame|particle Count|compute power
|-|-|-|-|-|
|C++|1|105|1000|0.1G/s
|CUDA|10|75|3800|3G/s

This should not be the best CUDA performance, however I have no idea how to optimize though.

Other information are record into DocLocal/notes.md.
