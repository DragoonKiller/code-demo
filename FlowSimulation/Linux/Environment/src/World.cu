#include "./World.h"
#include "./Log.hpp"
#include "./Util.hpp"

#include <exception>
#include <cmath>
#include <utility>
#include <algorithm>

#include "math.h"

// ====================== Definitions =======================

Data data;

// ======================== Kernels =========================

#define tid (threadIdx)
#define bid (blockIdx)
#define dim (blockDim)

struct KernelParticle
{
    Point location;
    Point velocity;
};

const int MaxParticleCount = 4096;
__constant__ KernelParticle kernelParticleSrc[MaxParticleCount];

__global__ void GForceEffect(
    float dt, int size,
    KernelParticle* src, KernelParticle* dst,
    float attrC, float attrP, float replC, float replP,
    float maxF, float mass, bool assign)
{
    int cid = tid.x + bid.x * dim.x;
    Point c = src[cid].location;
    
    Point acc = Point{0.0f, 0.0f};
    
    for(int i=0; i<size; i++)
    {
        if(i == cid) continue;
        Point p = kernelParticleSrc[i].location;
        Point dir = p(c);
        float dist = dir.len();
        acc = acc + (1.0f / dist) * (replC / pow(dist, replP) - attrC / pow(dist, attrP)) * (1.0f / mass) * dir;
    }
    
    if(assign)
    {
        dst[cid].location = src[cid].velocity * dt + 0.5f * acc * dt * dt;
        dst[cid].velocity = acc * dt;
    }
    else
    {
        dst[cid].location = dst[cid].location + src[cid].velocity * dt + 0.5f * acc * dt * dt;
        dst[cid].velocity = dst[cid].velocity + acc * dt;
    }
}

// ========================= Tests ==========================

// ======================= Simulation =======================

void Step(float dt)
{
    const float delay = GetParameter("GeneratingDelay");
    const float mass = GetParameter("Mass");
    const float replCoinc = GetParameter("RepulsionCoincident");
    const float replLevel = GetParameter("RepulsionLevel");
    const float attrCoinc = GetParameter("AttractionCoincident");
    const float attrLevel = GetParameter("AttractionLevel");
    const float limitCoinc = GetParameter("LimitCoincident");
    const float limitLevel = GetParameter("LimitLevel");
    const float maxForce = GetParameter("MaxForce");
    
    // generate.
    if(delay >= 1e-4f)
    {
        float usedTime = 0;
        data.freeTime += dt;
        while(data.freeTime >= 0.0f)
        {
            data.freeTime -= delay;
            usedTime += delay;
            data.particles.emplace_back(Particle {
                data.genFrom + RandReal() * (data.genTo - data.genFrom)
                    + data.initVelocity * usedTime,
                data.initVelocity,
                RandInt() % data.selective == 0 });
        }
    }
    
    // move.
    {
        const int n = data.particles.size();
        
        HostArray<KernelParticle> hdata(n);
        for(int i=0; i<n; i++)
        {
            hdata[i].location = data.particles[i].location;
            hdata[i].velocity = data.particles[i].velocity;
        }
        
        KernelArray<KernelParticle> kdata(hdata);
        KernelArray<KernelParticle> gdata(hdata);
        
        for(int base = 0; base < n; base += MaxParticleCount)
        {
            int m = min(MaxParticleCount, n - base) * sizeof(KernelParticle);
            cudaMemcpyToSymbol(kernelParticleSrc, hdata.pointer + base,m);
            const int threadCount = 256;
            GForceEffect<<<n / threadCount + 1, threadCount>>>(
                dt, m,
                kdata.pointer, gdata.pointer,
                attrCoinc, attrLevel, replCoinc, replLevel,
                maxForce, mass, base == 0);
        }
        
        while(true)
        {
            cudaError_t err = cudaGetLastError();
            if(cudaSuccess != err) Log("CUDA error: %s\n", cudaGetErrorString(err));
            else break;
        }
        
        hdata <<= gdata;
        for(int i=0; i<n; i++)
        {
            data.particles[i].location = hdata[i].location;
            data.particles[i].velocity = hdata[i].velocity;
        }
    }
    
    // remove.
    {
        data.particles.erase(
            std::remove_if(
                data.particles.begin(),
                data.particles.end(),
                [](Particle const& p)
                {
                    return p.location.x < data.limitLB.x
                        || data.limitRT.x < p.location.x
                        || p.location.y < data.limitLB.y
                        || data.limitRT.y < p.location.y;
                }),
            data.particles.end());
    }
    
    // re-arrange array.
    {
        // move shown particles to the left side of array.
        auto const& pos = std::partition(data.particles.begin(), data.particles.end(), [](Particle const& p)
        {
            return p.shown;
        });
        
        // assign value here so that the count needs no re-calculation.
        data.shownCount = (int)(pos - data.particles.begin());
    }
}

#pragma region InterfaceFunctions

void SetParameter(const char* s, float value)
{
    if(data.parameters.find(s) == data.parameters.end())
    {
        Log("Dynamically create new parameter: %s\n", s);
    }
    data.parameters[s] = value;
}

float GetParameter(const char* s)
{
    if(data.parameters.find(s) != data.parameters.end())
    {
        return data.parameters[s];
    }
    Log("Cannot find value of: %s\n", s);
    return NAN;
}

void EnvInit()
{
    data = Data();
    // Physics parameters.
    data.parameters["Mass"] = 1.0f;
    data.parameters["RepulsionCoincident"] = 1.0f;
    data.parameters["RepulsionLevel"] = 3.0f;
    data.parameters["AttractionCoincident"] = 1.0f;
    data.parameters["AttractionLevel"] = 2.0f;
    data.parameters["MaxForce"] = 100.0f;
    data.parameters["LimitCoincident"] = 1.0f;
    data.parameters["LimitLevel"] = 1.0f;
    // System parameters.
    data.parameters["GeneratingDelay"] = 0.05f;
    // System states.
    data.shownCount = 0;
    data.freeTime = 0;
}

void EnvDispose()
{
    
}

int GetMeshVertexCount()
{
    return data.mesh.size();
}

Vector2* GetMesh()
{
    if(data.mesh.size() == 0) return nullptr;
    data.vec2Output.clear();
    for(int i=0; i<data.mesh.size(); i++)
    {
        data.vec2Output.push_back(Vector2 {
            data.mesh[i].x,
            data.mesh[i].y });
    }
    return &data.vec2Output[0];
}

void SetMesh(Vector2* arr, int count)
{
    data.mesh.clear();
    for(int i=0; i<count; i++)
    {
        Point v;
        v.x = arr[i].x;
        v.y = arr[i].y;
        data.mesh.push_back(v);
    }
}

void SetSelectiveConstant(int x)
{
    if(x < 1) return;
    data.selective = x;
}

int GetSelectiveConstant()
{
    return data.selective;
}


int GetParticlesCount(bool selective)
{
    return selective ? data.shownCount : data.particles.size();
}

Vector2* GetParticles(bool selective)
{
    data.vec2Output.clear();
    for(int i = 0; i < data.shownCount; i++)
    {
        data.vec2Output.emplace_back(Vector2 {
            data.particles[i].location.x,
            data.particles[i].location.y });
    }
    return &data.vec2Output[0];
}

void RemoveAllParticles()
{
    data.particles.clear();
}

void SetInitialVelocity(Vector2 t)
{
    data.initVelocity.x = t.x;
    data.initVelocity.y = t.y;
}

Vector2 GetInitialVelocity()
{
    Vector2 val;
    val.x = data.initVelocity.x;
    val.y = data.initVelocity.y;
    return val;
}

void SetGeneratingLine(Rectangle area)
{
    data.genFrom.x = area.x;
    data.genFrom.y = area.y;
    data.genTo.x = area.x + area.w;
    data.genTo.y = area.y + area.h;
}

Rectangle GetGeneratingLine()
{
    Rectangle area;
    area.x = data.genFrom.x;
    area.y = data.genFrom.y;
    area.w = data.genTo.x - data.genFrom.x;
    area.h = data.genTo.y - data.genFrom.y;
    return area;
}

void SetLimitArea(Rectangle area)
{
    data.limitLB.x = area.x;
    data.limitLB.y = area.y;
    data.limitRT.x = area.x + area.w;
    data.limitRT.y = area.y + area.h;
}

Rectangle GetLimitArea()
{
    Rectangle area;
    area.x = data.limitLB.x;
    area.y = data.limitLB.y;
    area.w = data.limitRT.x - data.limitLB.x;
    area.h = data.limitRT.y - data.limitLB.y;
    return area;
}

#pragma endregion InterfaceFunctions
