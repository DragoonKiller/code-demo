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

#include "box2d/b2_dynamic_tree.h"

using namespace System;

namespace Box2D
{
    /// <summary>
    /// Dynamic AABB Tree extracted from Box2D.
    /// The Original AABB should be held by the user of this class.
    /// </summary>
    public ref class AABBTree
    {
        public:

        AABBTree()
        {
            tree = new b2DynamicTree();
        }

        /// <summary>
        /// Create a proxy. Provide a tight fitting AABB and a userData pointer.
        /// </summary>
        int CreateProxy(AABB aabb)
        {
            return tree->CreateProxy(aabb.ToB2());
        }

        /// <summary>
        /// Destroy a proxy. This asserts if the id is invalid.
        /// </summary>
        void DestroyProxy(int proxyId)
        {
            tree->DestroyProxy(proxyId);
        }

        /// <summary>
        /// Move a proxy with a swepted AABB. If the proxy has moved outside of its fattened AABB,
        /// then the proxy is removed from the tree and re-inserted. Otherwise
        /// the function returns immediately.
        /// @return true if the proxy was re-inserted.
        /// </summary>
        bool MoveProxy(int32 proxyId, AABB aabb, Vector2 position)
        {
            return tree->MoveProxy(proxyId, aabb.ToB2(), position.ToB2());
        }

        bool WasMoved(int32 proxyId)
        {
            return tree->WasMoved(proxyId);
        }

        void ClearMoved(int32 proxyId)
        {
            tree->ClearMoved(proxyId);
        }

        /// <summary>
        /// Get the fat AABB for a proxy.
        /// </summary>
        AABB GetFatAABB(int32 proxyId)
        {
            return AABB::FromB2(tree->GetFatAABB(proxyId));
        }

        /// <summary>
        /// Query an AABB for overlapping proxies. The callback class
        /// is called for each proxy that overlaps the supplied AABB.
        /// </summary>
        void Query(Func<int, bool>^ callback, AABB aabb)
        {
            tree->Query(callback, aabb.ToB2());
        }

        /// <summary>
        /// Ray-cast against the proxies in the tree. This relies on the callback
        /// to perform a exact ray-cast in the case were the proxy contains a shape.
        /// The callback also performs the any collision filtering. This has performance
        /// roughly equal to k * log(n), where k is the number of collisions and n is the
        /// number of proxies in the tree.
        /// @param input the ray-cast input data. The ray extends from p1 to p1 + maxFraction * (p2 - p1).
        /// @param callback a callback class that is called for each proxy that is hit by the ray.
        /// </summary>
        void RayCast(Func<RayCastInput, int, float>^ callback, RayCastInput input)
        {
            tree->RayCast(callback, input.ToB2());
        }

        /// <summary>
        /// Validate this tree. For testing.
        /// </summary>
        void Validate()
        {
            tree->Validate();
        }

        /// <summary>
        /// Compute the height of the binary tree in O(N) time. Should not be
        /// called often.
        /// </summary>
        int32 GetHeight()
        {
            return tree->GetHeight();
        }

        /// <summary>
        /// Get the maximum balance of an node in the tree. The balance is the difference
        /// in height of the two children of a node.
        /// </summary>
        int32 GetMaxBalance()
        {
            return tree->GetMaxBalance();
        }

        /// <summary>
        /// Get the ratio of the sum of the node areas to the root area.
        /// </summary>
        float GetAreaRatio()
        {
            return tree->GetAreaRatio();
        }

        /// <summary>
        /// Build an optimal tree. Very expensive. For testing.
        /// </summary>
        void RebuildBottomUp()
        {
            tree->RebuildBottomUp();
        }

        /// <summary>
        /// Shift the world origin. Useful for large worlds.
        /// The shift formula is: position -= newOrigin
        /// </summary>
        void ShiftOrigin(Vector2 newOrigin)
        {
            tree->ShiftOrigin(newOrigin.ToB2());
        }

        /// <summary>
        /// Destructor.
        /// Equal to Dispose() in C# and will be called automatically just like in C++.
        /// Used to clear everything.
        /// </summary>
        ~AABBTree()
        {
            this->!AABBTree();
        }

        /// <summary>
        /// Finalizer.
        /// Equal to Destructor in C#, and operative with GC.
        /// Used to clear unmanaged contents.
        /// </summary>
        !AABBTree()
        {
            if (tree != nullptr)
            {
                delete tree;
                tree = nullptr;
            }
        }

        private:
        b2DynamicTree* tree;

    };
}