using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace TexToMesh
{

    [RequireComponent(typeof(TexRender))]
    public class TexToMesh : MonoBehaviour
    {
        public MeshFilter[] target;
        public Vector2 scale;
        
        
        // To simply simulate a Linked list, without using Collections.Generic.LinkedList.
        // LinkedListNode.Next and LinkedListNode.Previous is readonly.
        class Node
        {
            public Vector2Int value;
            public Node next;
            public Node prev;
            public Node(Vector2Int val)
            {
                value = val;
                next = prev = null;
            }
            public void Connect(Node to)
            {
                this.next = to;
                to.prev = this;
            }
        }
        
        struct Path
        {
            public Node from, to;
            public Path AppendFrom(Node x)
            {
                x.Connect(from);
                return new Path() { from = x, to = to };
            }
            public Path AppendTo(Node x) 
            {
                to.Connect(x);
                return new Path() { from = from, to = x };
            }
            public Path Append(Node x)
            {
                if(x.value == from.value) return AppendFrom(x);
                else if(x.value == to.value) return AppendTo(x);
                else throw new ArgumentException();
            }
        }
        
        
        void Start()
        {
            // should run AFTER refresh.
            GetComponent<TexRender>().AfterRefresh += () =>
            {
                // Debug.Log("Run refresh!");
                
                var tex = this.gameObject.GetComponent<TexRender>().tex;
                int width = tex.width;
                int height = tex.height;
                var data = tex.GetPixels(0, 0, width, height);
                
                int Id(int x, int y) => y * width + x;
                bool Filled(int x, int y) =>  x < 0 || x >= width || y < 0 || y >= height ? false : data[Id(x, y)].r < 0.5;
                
                // Create detailed mesh.
                if(target.Length <= 0 || target[0] == null) return;
                
                // Stores final outlining linked lists.
                var links = new List<Node>();
                
                // Stores not-closed outlines.
                var heads = new List<Path>();
                
                Dictionary<Vector2Int, int> GenerateIndexTable()
                {
                    var headIndex = new Dictionary<Vector2Int, int>();
                    for(int i=0; i<heads.Count; i++)
                    {
                        headIndex.Add(heads[i].from.value, i);
                        headIndex.Add(heads[i].to.value, i);
                    }
                    return headIndex;
                }   
                
                for(int x=0; x<=width; x++)
                {
                    // Map heads to index.
                    var headIndex = GenerateIndexTable();
                    
                    foreach(var i in headIndex) if(i.Key.x != x) throw new Exception();
                    
                    // Shall not overlap.
                    // Scan all segments on the column.
                    var segments = new List<Path>();
                    Node begin = null, end = null;
                    for(int y=0; y<=height; y++)
                    {
                        void Submit(bool leftFilled)
                        {
                            if(begin == null) return;
                            var newSeg = new Path(){ from = begin, to = end };
                            if(!leftFilled) newSeg = new Path(){ from = newSeg.to, to = newSeg.from };
                            newSeg.from.Connect(newSeg.to);
                            segments.Add(newSeg);
                            begin = end = null;
                        }
                        
                        // Setup a new segment, always target up.
                        // Direction will be considered when submitted.
                        void SetupNew() => (begin, end) = (new Node(new Vector2Int(x, y)), new Node(new Vector2Int(x, y + 1)));
                        
                        // There's no segment here.
                        if(Filled(x - 1, y) == Filled(x, y))
                        {
                            Submit(Filled(x - 1, y - 1) && !Filled(x, y - 1));
                            continue;
                        }
                        
                        // Force split if there is an endpoint.
                        // And because there's still a segment, we need to setup a new segment.
                        if(headIndex.ContainsKey(new Vector2Int(x, y)))
                        {
                            Submit(Filled(x - 1, y - 1) && !Filled(x, y - 1));
                            SetupNew();
                            continue;
                        }
                        
                        // Create a segment if there isn't.
                        if(begin == null) SetupNew();
                        // ... or extend current segment.
                        else end.value = new Vector2Int(x, y + 1);
                    }
                    
                    foreach(var seg in segments)
                    {
                        bool fromConnected = headIndex.TryGetValue(seg.from.value, out int fromChain);
                        bool toConnected = headIndex.TryGetValue(seg.to.value, out int toChain);
                        
                        if(fromConnected && toConnected)
                        {
                            var fromNode = heads[fromChain].to;
                            var toNode = heads[toChain].from;
                            
                            // This segment does not belong to this outline.
                            if(fromNode.value != seg.from.value || toNode.value != seg.to.value) throw new Exception();
                            
                            fromNode.Connect(toNode);
                            if(fromChain == toChain)
                            {
                                // Tag them as null so that we can remove them later.
                                headIndex.Remove(fromNode.value);
                                headIndex.Remove(toNode.value);
                                heads[fromChain] = new Path() { from = null, to = null };
                                heads[toChain] = new Path() { from = null, to = null };
                            }
                            else
                            {
                                // Remove index.
                                headIndex.Remove(heads[toChain].from.value);
                                headIndex.Remove(heads[fromChain].to.value);
                                // Remove one, modify another one.
                                heads[fromChain] = new Path() { from = heads[fromChain].from, to = heads[toChain].to };
                                // This coordinate is still in use.
                                headIndex[heads[toChain].to.value] = fromChain;
                                // Finally tag it as remove.
                                heads[toChain] = new Path() { from = null, to = null };
                            }
                            
                            // submit to final result.
                            links.Add(fromNode);
                        }
                        else if(fromConnected)
                        {
                            headIndex.Remove(heads[fromChain].to.value);
                            heads[fromChain] = heads[fromChain].AppendTo(seg.to);
                        }
                        else if(toConnected)
                        {
                            headIndex.Remove(heads[toChain].from.value);
                            heads[toChain] = heads[toChain].AppendFrom(seg.from);
                        }
                        else heads.Add(seg);
                    }
                    
                    heads.RemoveAll((segx) => segx.from == null && segx.to == null);
                    
                    // Heads are changed after vertical connection.
                    // Index table should be re-generated.
                    headIndex = GenerateIndexTable();
                    
                    foreach(var i in headIndex) if(i.Key.x != x) throw new Exception();
                    
                    for(int y=0; y<=height; y++)
                    {
                        if(Filled(x, y - 1) == Filled(x, y)) continue;
                        
                        if(headIndex.TryGetValue(new Vector2Int(x, y), out int ind))
                        {
                            var newNode = new Node(new Vector2Int(x + 1, y));
                            if(heads[ind].from.value == new Vector2Int(x, y))  heads[ind] = heads[ind].AppendFrom(newNode);
                            else if(heads[ind].to.value == new Vector2Int(x, y)) heads[ind] = heads[ind].AppendTo(newNode);
                        }
                    }
                }
                
                var verts = new List<Vector3>();
                var edges = new List<int>();
                
                Debug.Log("curve count " + links.Count);
                
                foreach(var head in links)
                {
                    var cur = head;
                    do
                    {
                        if( verts.Count >= 2
                        && (int)verts[verts.Count - 2].y == cur.next.value.y
                        && (int)verts[verts.Count - 1].y == cur.next.value.y )
                        {
                            cur = cur.next;
                            verts[verts.Count - 1] = (Vector2)cur.value;
                        }
                        else
                        {
                            verts.Add((Vector2)cur.value);
                            cur = cur.next;
                            verts.Add((Vector2)cur.value);
                            edges.Add(verts.Count - 2);
                            edges.Add(verts.Count - 1);
                        }
                    }
                    while(cur != head);
                }
                
                target[0].mesh = new Mesh().SubmitData(verts, edges, MeshTopology.Lines);
            };
        }
        
        void Update()
        {
        }
    }
    
} // namespace TexToMesh
