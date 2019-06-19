
#include "Box2D/Box2D.h"

#include <iostream>
#include <thread>
#include <chrono>
#include <algorithm>

#include "SimpleDraw.hpp"

int main(int argc, char** argv)
{
    auto canvas = new Canvas("x", point{ 800, 600 });
    auto world = new b2World(b2Vec2(0.f, -22));

    auto origin = canvas->Size() * 0.5f;
    auto coord = Coord{ point(1, 0), point(0, -1) };
    canvas->world2screen = [=](point a) { return coord.recover(a) + origin; };

    auto ground = [&]
    {
        auto def = b2BodyDef();
        def.type = b2BodyType::b2_staticBody;
        def.position.Set(0.0f, -150.0f);
        auto ground = world->CreateBody(&def);

        b2PolygonShape shape;
        shape.SetAsBox(500, 100);

        ground->CreateFixture(&shape, 100.0f);

        return ground;
    }();

    canvas->Add(new Rect(0, -150, 1000, 200));

    point poses[] = {
        {20, 0},
        {100, 20},
        {80, 200},
        {-60, 120},
        {-30, 40}
    };

    const int cubeCount = sizeof(poses) / sizeof(point);

    b2Body* cubes[cubeCount];
    for(int i=0; i< cubeCount; i++) cubes[i] = [&]
    {
        auto def = b2BodyDef();
        def.type = b2_dynamicBody;
        def.position = poses[i].tob2();
        b2Body* body = world->CreateBody(&def);

        b2PolygonShape shape;
        shape.SetAsBox(20, 20);

        auto mass = b2MassData();
        mass.center = point(0, 0).tob2();
        mass.I = 1250;
        mass.mass = 1;

        body->SetMassData(&mass);

        b2FixtureDef fixture;
        fixture.shape = &shape;
        fixture.density = 1.0f;
        fixture.friction = 0.1f;

        body->CreateFixture(&fixture);

        return body;
    }();

    Rect* cubedraws[cubeCount];
    for (int i = 0; i < cubeCount; i++)
    {
        cubedraws[i] = new Rect(0, 0, 40, 40);
        canvas->Add(cubedraws[i]);
    }

    const int frameLimits = 50;
    const long long expectedFrameTimeMicrosec = 1000000 / frameLimits;
    while (true)
    {
        if(!canvas->PollEvent()) break;

        auto recBegin = std::chrono::system_clock::now();

        // Physics time step should be fixed!
        world->Step(expectedFrameTimeMicrosec * 0.000001f, 8, 6);
        
        for (int i = 0; i < cubeCount; i++)
        {
            cubedraws[i]->center = point(cubes[i]->GetPosition());
            cubedraws[i]->rotation = cubes[i]->GetAngle();
        }
        
        canvas->Render();

        auto rec = std::chrono::system_clock::now() - recBegin;
        auto usedTime = std::chrono::duration_cast<std::chrono::microseconds>(rec).count();

        std::this_thread::sleep_for(std::chrono::microseconds(std::max(0LL, expectedFrameTimeMicrosec - usedTime)));
    }

    return 0;
}
