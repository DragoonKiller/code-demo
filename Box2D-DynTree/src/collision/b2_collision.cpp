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

#include "box2d/b2_collision.h"

namespace Box2D
{
	namespace Internal
	{

// From Real-time Collision Detection, p179.
		bool b2AABB::RayCast(b2RayCastOutput* output, const b2RayCastInput& input) const
		{
			float tmin = -b2_maxFloat;
			float tmax = b2_maxFloat;

			b2Vec2 p = input.p1;
			b2Vec2 d = input.p2 - input.p1;
			b2Vec2 absD = b2Abs(d);

			b2Vec2 normal;

			for (int32 i = 0; i < 2; ++i)
			{
				if (absD(i) < b2_epsilon)
				{
					// Parallel.
					if (p(i) < lowerBound(i) || upperBound(i) < p(i))
					{
						return false;
					}
				}
				else
				{
					float inv_d = 1.0f / d(i);
					float t1 = (lowerBound(i) - p(i)) * inv_d;
					float t2 = (upperBound(i) - p(i)) * inv_d;

					// Sign of the normal vector.
					float s = -1.0f;

					if (t1 > t2)
					{
						b2Swap(t1, t2);
						s = 1.0f;
					}

					// Push the min up
					if (t1 > tmin)
					{
						normal.SetZero();
						normal(i) = s;
						tmin = t1;
					}

					// Pull the max down
					tmax = b2Min(tmax, t2);

					if (tmin > tmax)
					{
						return false;
					}
				}
			}

			// Does the ray start inside the box?
			// Does the ray intersect beyond the max fraction?
			if (tmin < 0.0f || input.maxFraction < tmin)
			{
				return false;
			}

			// Intersection.
			output->fraction = tmin;
			output->normal = normal;
			return true;
		}
	}
}