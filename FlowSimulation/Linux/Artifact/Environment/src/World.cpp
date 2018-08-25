#include "./World.h"
#include "./Log.h"
#include "./Math.hpp"

#include <cstdio>
#include <exception>
#include <random>
#include <cmath>
#include <algorithm>

// ====================== Definitions =======================

Data data;

// ======================== Utility =========================

static auto RandDouble()
{
    static std::default_random_engine gen;
    static std::uniform_real_distribution<double> dis(0, 1);
    return dis(gen);
}

static auto RandInt()
{
    static std::default_random_engine gen;
    static std::uniform_int_distribution<int> dis(INT_MIN, INT_MAX);
    return dis(gen);
}

static auto RandUInt()
{
    static std::default_random_engine gen;
    static std::uniform_int_distribution<unsigned> dis(0, UINT_MAX);
    return dis(gen);
}

static auto InsideLimitArea(Point const& v)
{
    return data.limitLB.x < v.x
        && data.limitLB.y < v.y
        && v.x < data.limitRT.x 
        && v.y < data.limitRT.y;
};

// ======================= Simulation =======================

void Step(float dt)
{
    static bool tooFastLogged = false;
    if(GetParameter("GeneratingDelay") <= 0.000001)
    {
        if(!tooFastLogged) Log("Generating too fast!\n");
        tooFastLogged = true;
    }
    else tooFastLogged = false;
    
    data.freeTime += dt;
    
    // ========== Create ==========
    {
        float delay = GetParameter("GeneratingDelay");
        while(data.freeTime >= delay)
        {
            data.freeTime -= delay;
            data.particles.emplace_back(Particle {
                data.genFrom + RandDouble() * (data.genTo - data.genFrom) + data.freeTime * data.initVelocity,
                data.initVelocity,
                RandUInt() % std::max(1, data.selective) == 0 });
        }
    }
    
    // ========== Move ==========
    {
        // magic constants.
        const double mass = GetParameter("Mass");
        const double replCoinc = GetParameter("RepulsionCoincident");
        const double replLevel = GetParameter("RepulsionLevel");
        const double attrCoinc = GetParameter("AttractionCoincident");
        const double attrLevel = GetParameter("AttractionLevel");
        const double limitCoinc = GetParameter("LimitCoincident");
        const double limitLevel = GetParameter("LimitLevel");
        const double maxForce = GetParameter("MaxForce");
        
        auto& ptcs = data.particles;
        
        for(int i = 0; i < ptcs.size(); i++)
        {
            auto& cur = ptcs[i];
            
            // acceleration.
            Point acc = Point {0.0, 0.0};
            
            // effect of other particles.
            for(int j = 0; j < ptcs.size(); j++)
            {
                if(i != j)
                {
                    Point dir = cur.location(ptcs[j].location);
                    // repulsion.
                    double rep = replCoinc / std::pow(dir.len2(), 0.5 * replLevel);
                    // attraction.
                    double att = attrCoinc / std::pow(dir.len2(), 0.5 * attrLevel);
                    
                    acc += Norm(dir) * (att - rep);
                }
            }
            
            // effect of top and buttom limits.
            double tDist = data.limitRT.y - cur.location.y;
            acc += down * (limitCoinc / std::pow(tDist, limitLevel));
            double bDist = cur.location.y - data.limitLB.y;
            acc += up * (limitCoinc / std::pow(bDist, limitLevel));
            
            // final acceleration clamps.
            if(acc.len2() > maxForce * maxForce) acc = Norm(acc) * maxForce;
            acc *= 1.0 / mass;
            
            cur.location += cur.velocity * dt + 0.5 * acc * dt * dt;
            cur.velocity += acc * dt;
            
            // bump if touching the limit area.
            if(cur.location.y > data.limitRT.y)
            {
                cur.location.y = 2.0 * data.limitRT.y - cur.location.y;
                cur.velocity.y = - cur.velocity.y;
            }
            
            if(cur.location.y < data.limitLB.y)
            {
                cur.location.y = 2.0 * data.limitLB.y - cur.location.y;
                cur.velocity.y = - cur.velocity.y;
            }
        }
    }
    
    // ========== Clear ==========
    {
        
        for(int i=0; i<data.particles.size(); i++)
        {
            if(data.particles[i].location.x < data.limitLB.x || data.limitRT.x < data.particles[i].location.x)
            {
                data.particles[i] = data.particles.back();
                data.particles.pop_back();
            }
        }
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
}

void EnvDispose()
{
    // do nothing...
}

int GetMeshVertexCount()
{
    return data.mesh.size();
}

Vector2* GetMesh()
{
    data.vec2Output.clear();
    for(int i=0; i<data.mesh.size(); i++)
    {
        Vector2 val;
        val.x = data.mesh[i].x;
        val.y = data.mesh[i].y;
        data.vec2Output.push_back(val);
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
    if(selective)
    {
        int c = 0;
        for(auto& i : data.particles)
        {
            if(i.shown) c++;
        }
        return c;
    }
    return data.particles.size();
}

Vector2* GetParticles(bool selective)
{
    data.vec2Output.clear();
    for(auto& i : data.particles)
    {
        if(!selective || i.shown)
        {
            Vector2 val;
            val.x = i.location.x;
            val.y = i.location.y;
            data.vec2Output.push_back(val);
        }
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
