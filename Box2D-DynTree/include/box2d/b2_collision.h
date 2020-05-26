// MIT License

// Copyright (c) 2019 Erin Catto

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

#ifndef B2_COLLISION_H
#define B2_COLLISION_H

#include "b2_math.h"
#include <limits.h>

namespace Box2D
{
    namespace Internal
    {

        /// Ray-cast input data. The ray extends from p1 to p1 + maxFraction * (p2 - p1).
        struct b2RayCastInput
        {
            b2Vec2 p1, p2;
            float maxFraction;
        };

        /// Ray-cast output data. The ray hits at p1 + fraction * (p2 - p1), where p1 and p2
        /// come from b2RayCastInput.
        struct b2RayCastOutput
        {
            b2Vec2 normal;
            float fraction;
        };

        /// An axis aligned bounding box.
        struct b2AABB
        {
            /// Verify that the bounds are sorted.
            bool IsValid() const;

            /// Get the center of the AABB.
            b2Vec2 GetCenter() const
            {
                return 0.5f * (lowerBound + upperBound);
            }

            /// Get the extents of the AABB (half-widths).
            b2Vec2 GetExtents() const
            {
                return 0.5f * (upperBound - lowerBound);
            }

            /// Get the perimeter length
            float GetPerimeter() const
            {
                float wx = upperBound.x - lowerBound.x;
                float wy = upperBound.y - lowerBound.y;
                return 2.0f * (wx + wy);
            }

            /// Combine an AABB into this one.
            void Combine(const b2AABB& aabb)
            {
                lowerBound = b2Min(lowerBound, aabb.lowerBound);
                upperBound = b2Max(upperBound, aabb.upperBound);
            }

            /// Combine two AABBs into this one.
            void Combine(const b2AABB& aabb1, const b2AABB& aabb2)
            {
                lowerBound = b2Min(aabb1.lowerBound, aabb2.lowerBound);
                upperBound = b2Max(aabb1.upperBound, aabb2.upperBound);
            }

            /// Does this aabb contain the provided AABB.
            bool Contains(const b2AABB& aabb) const
            {
                bool result = true;
                result = result && lowerBound.x <= aabb.lowerBound.x;
                result = result && lowerBound.y <= aabb.lowerBound.y;
                result = result && aabb.upperBound.x <= upperBound.x;
                result = result && aabb.upperBound.y <= upperBound.y;
                return result;
            }

            bool RayCast(b2RayCastOutput* output, const b2RayCastInput& input) const;

            b2Vec2 lowerBound;	///< the lower vertex
            b2Vec2 upperBound;	///< the upper vertex
        };

        // ---------------- Inline Functions ------------------------------------------

        inline bool b2AABB::IsValid() const
        {
            b2Vec2 d = upperBound - lowerBound;
            bool valid = d.x >= 0.0f && d.y >= 0.0f;
            valid = valid && lowerBound.IsValid() && upperBound.IsValid();
            return valid;
        }

        inline bool b2TestOverlap(const b2AABB& a, const b2AABB& b)
        {
            b2Vec2 d1, d2;
            d1 = b.lowerBound - a.upperBound;
            d2 = a.lowerBound - b.upperBound;

            if (d1.x > 0.0f || d1.y > 0.0f)
                return false;

            if (d2.x > 0.0f || d2.y > 0.0f)
                return false;

            return true;
        }
    }
}

#endif
