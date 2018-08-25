/// The main simulator.
/// Dragoonkiller : 2018.08.02
///
/// Main simulator of the system.
/// Provides a single mesh, generating particles as time passing by,
///   calculate the collision and simulate partiles forces properly.
/// Simply using Lennard-Jones potential. This might be changed in the future...
/// 

#ifndef WORLD
#define WORLD

#include <vector>
#include <map>
#include <string>

#include "Math.hpp"

struct Vector2 { float x, y; };

struct Rectangle { float x, y, w, h; };

struct Particle
{
    Point location;
    Point velocity;
    bool shown;
    char _pad[3];
};

struct Data
{
    std::vector<Particle> particles;
    int shownCount;
    
    std::vector<Point> mesh;
    std::vector<Vector2> vec2Output;
    
    Point initVelocity;
    
    Point genFrom;
    Point genTo;
    
    Point limitLB;
    Point limitRT;
    
    std::map<std::string, float> parameters;
    
    int selective; // A posibility of 1 / selective of witch points are not shown.
    
    float freeTime; // Time step we shall proceed the world.
};

extern Data data;

#define PreventOverload(f) static_assert(!std::is_void<decltype(f)>::value, "Function is not allowed to be overloaded.")

#define Export(type) extern "C" type __stdcall 

#ifndef WINDOWS
#define __stdcall
#endif

Export(void) EnvInit();
PreventOverload(EnvInit);

Export(void) EnvDispose();
PreventOverload(EnvDispose);

// ======================= Simulation =======================

/// Time in seconds.
Export(void) Step(float t);
PreventOverload(Step);

Export(void) SetParameter(const char* t, float value);
PreventOverload(SetParameter);

Export(float) GetParameter(const char* t);
PreventOverload(GetParameter);

// ========================== Mesh ==========================

Export(int) GetMeshVertexCount();
PreventOverload(GetMeshVertexCount);

Export(Vector2*) GetMesh();
PreventOverload(GetMesh);

Export(void) SetMesh(Vector2* arr, int count);
PreventOverload(SetMesh);

// ======================== Particles =======================

Export(void) SetSelectiveConstant(int x);
PreventOverload(SetSelectiveConstant);

Export(int) GetSelectiveConstant();
PreventOverload(GetSelectiveConstant);

Export(int) GetParticlesCount(bool selective);
PreventOverload(GetParticlesCount);

Export(Vector2*) GetParticles(bool selective);
PreventOverload(GetParticles);

Export(void) RemoveAllParticles();
PreventOverload(RemoveAllParticles);

Export(Vector2) GetInitialVelocity();
PreventOverload(GetInitialVelocity);

Export(void) SetInitialVelocity(Vector2 v);
PreventOverload(GetParticlesCount);

/// Particles will automatically generated on this line, in a specific way.
/// Take a vector4 about (xMin, yMin, xMax, yMax).
Export(void) SetGeneratingLine(Rectangle area);
PreventOverload(SetGeneratingLine);

Export(Rectangle) GetGeneratingLine();
PreventOverload(GetGeneratingLine);

/// Particles will be pushed back when colliding with the boundary limit area.
Export(void) SetLimitArea(Rectangle area);
PreventOverload(SetLimitArea);

Export(Rectangle) GetLimitArea();
PreventOverload(GetLimitArea);

#undef PreventOverload

#endif // WORLD
