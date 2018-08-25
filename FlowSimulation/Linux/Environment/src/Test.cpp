#include "./World.h"

#include <assert.h>
#include <cstdio>

/// Test for environments.

int main()
{
    EnvInit();
    
    assert(heq(GetParameter("GeneratingDelay"), 0.05f));
    assert(GetParticlesCount(false) == 0);
    assert(GetMeshVertexCount() == 0);
    assert(GetGeneratingLine().x == 0);
    assert(GetGeneratingLine().y == 0);
    assert(GetGeneratingLine().h == 0);
    assert(GetGeneratingLine().w == 0);
    assert(GetLimitArea().x == 0);
    assert(GetLimitArea().y == 0);
    assert(GetLimitArea().h == 0);
    assert(GetLimitArea().w == 0);
    assert(GetInitialVelocity().x == 0);
    assert(GetInitialVelocity().y == 0);
    
    SetParameter("GeneratingDelay", 0.125f);
    assert(heq(GetParameter("GeneratingDelay"), 0.125f));
    
    SetInitialVelocity(Vector2 { 1.0, 1.25 });
    assert(heq(GetInitialVelocity().x, 1.0f));
    assert(heq(GetInitialVelocity().y, 1.25f));
    
    SetGeneratingLine(Rectangle { -1.0, -1.0, -2.0, 0.0 });
    assert(heq(GetGeneratingLine().x, -1.0f));
    assert(heq(GetGeneratingLine().y, -1.0f));
    assert(heq(GetGeneratingLine().w, -2.0f));
    assert(heq(GetGeneratingLine().h, 0.0f));
    
    SetLimitArea(Rectangle { -1.0, -1.0, 2.0, 2.0 });
    assert(heq(GetLimitArea().x, -1.0f));
    assert(heq(GetLimitArea().y, -1.0f));
    assert(heq(GetLimitArea().w, 2.0f));
    assert(heq(GetLimitArea().h, 2.0f));
    
    Step(0.16f);
    
    EnvDispose();
    
    return 0;
}
