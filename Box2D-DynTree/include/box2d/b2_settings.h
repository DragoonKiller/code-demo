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

#ifndef B2_SETTINGS_H
#define B2_SETTINGS_H

#include <stddef.h>
#include <assert.h>
#include <float.h>

namespace Box2D
{
	namespace Internal
	{

		#if defined(_DEBUG)
		#define b2DEBUG
		#endif

		#define B2_NOT_USED(x) ((void)(x))
		#define b2Assert(A) assert(A)

		typedef signed char	int8;
		typedef signed short int16;
		typedef signed int int32;
		typedef unsigned char uint8;
		typedef unsigned short uint16;
		typedef unsigned int uint32;

		#define	b2_maxFloat		FLT_MAX
		#define	b2_epsilon		FLT_EPSILON
		#define b2_pi			3.14159265359f

		/// @file
		/// Global tuning constants based on meters-kilograms-seconds (MKS) units.
		///

		// Collision

		/// This is used to fatten AABBs in the dynamic tree. This allows proxies
		/// to move by a small amount without triggering a tree adjustment.
		/// This is in meters.
		#define b2_aabbExtension		0.1f

		/// This is used to fatten AABBs in the dynamic tree. This is used to predict
		/// the future position based on the current displacement.
		/// This is a dimensionless multiplier.
		#define b2_aabbMultiplier		4.0f

		// Memory Allocation

		/// Implement this function to use your own memory allocator.
		void* b2Alloc(int32 size);

		/// If you implement b2Alloc, you should also implement this function.
		void b2Free(void* mem);

		/// Version numbering scheme.
		/// See http://en.wikipedia.org/wiki/Software_versioning
		struct b2Version
		{
			int32 major;		///< significant changes
			int32 minor;		///< incremental changes
			int32 revision;		///< bug fixes
		};

		/// Current version.
		extern b2Version b2_version;
	}
}
#endif
