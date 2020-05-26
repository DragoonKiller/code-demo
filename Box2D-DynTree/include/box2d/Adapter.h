// MIT License

// Copyright (c) 2020 Narellanda Karatoga

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#ifndef ADAPTER_H
#define ADAPTER_H

#include "b2_collision.h"

namespace Box2D
{
    using namespace Box2D::Internal;

    public value struct Vector2
    {
        public:
        float x;
        float y;
        static Vector2 FromB2(b2Vec2 const& x)
        {
            return { x.x, x.y };
        }
        b2Vec2 ToB2() { return { x, y }; }
    };

    public value struct AABB
    {
        public:
        Vector2 lower;
        Vector2 upper;
        static AABB FromB2(b2AABB const& aabb)
        {
            return { Vector2::FromB2(aabb.lowerBound), Vector2::FromB2(aabb.upperBound) };
        }
        b2AABB ToB2() { return { lower.ToB2(), upper.ToB2() }; }
    };

    public value struct RayCastInput
    {
        public:
        Vector2 from;
        Vector2 to;
        float maxFraction;
        static RayCastInput FromB2(b2RayCastInput const& input)
        {
            return { Vector2::FromB2(input.p1), Vector2::FromB2(input.p2), input.maxFraction };
        };

        b2RayCastInput ToB2() { return { from.ToB2(), to.ToB2(), maxFraction }; }
    };

}


#endif