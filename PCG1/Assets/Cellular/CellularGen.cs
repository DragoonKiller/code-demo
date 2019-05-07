using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Cellular automaton generator
/// Use it in an environment script.
/// Need something representing the map.
public class CellularGen : MonoBehaviour
{
    public GameObject source;
    SpriteRenderer[,] targets;
    bool[,] map;
    
    public int width;
    public int height;
    public int seed;
    public int shrinkThreashold;
    
    /// size of target grids.
    public Vector2 size;
    
    struct Data
    {
        public int width;
        public int height;
        public int seed;
        public Vector2 size;
    }
    Data data;
    
    public float delay;
    
    void Start()
    {
        Init();
    }
    
    void Update()
    {
        if(data.width != width || data.height != height || data.seed != seed || data.size != size)
        {
            data.width = width;
            data.height = height;
            data.seed = seed;
            data.size = size;
            Init();
        }
        Evolution();
        if(Input.GetKeyDown(KeyCode.K)) Shrink();
        Sync();
    }
    
    public int genCount;
    
    void Init()
    {
        genCount = 0;
        Random.InitState(seed);
        targets = new SpriteRenderer[height, width];
        map = new bool[height, width];
        
        for(int y=0; y<height; y++) for(int x=0; x<width; x++)
        {
            Vector2 pos = new Vector2(
                size.x * 0.5f + x * size.x,
                size.y * 0.5f + y * size.y
            );
            
            // initiate targets.
            targets[y, x] = Instantiate(source).GetComponent<SpriteRenderer>();
            targets[y, x].transform.position = pos;
            targets[y, x].transform.localScale = new Vector3(size.x, size.y, 1.0f);
            
            // randomize.
            map[y, x] = Random.Range(0, 2) == 0;
        }
    }
    
    void Evolution()
    {
        // Rule:
        // edges are always counted as alives.
        // sum up all adjacent alive grids.
        // use it to determine whether it die, revive, or stay.
        
        var nxtmap = (bool[,])map.Clone();
        
        for(int y=0; y<height; y++) for(int x=0; x<width; x++)
        {
            if(x == 0 || x == width - 1 || y == 0 || y == height - 1) { nxtmap[y, x] = true; continue; }
            
            int cnt = 0;
            for(int dx = -1; dx <= 1; dx++) for(int dy = -1; dy <= 1; dy++)
            {
                if(dx == 0 && dy == 0) continue;
                int cx = x + dx;
                int cy = y + dy;
                if(map[cy, cx]) { cnt += 1; }
            }
            
            // Use a *convergence* formula.
            // Notice the number *4* is the *ONLY* magic number to make everything good.
            //   For this kernel function only though.
            // But I don't know how to draw another kernel function to get this work.
            if(cnt > 4) nxtmap[y, x] = true;
            else if(cnt < 4) nxtmap[y, x] = false;
            else nxtmap[y, x] = map[y, x];
        }
        
        bool gt = false;
        for(int y=0; y<height && !gt; y++) for(int x=0; x<width && !gt; x++) if(map[y, x] != nxtmap[y, x]) gt = true;
        if(gt) genCount += 1;
        
        map = nxtmap;
        
    }
    
    void Shrink()
    {
        var nxtmap = (bool[,])map.Clone();
        
        for(int y=0; y<height; y++) for(int x=0; x<width; x++)
        {
            if(x == 0 || x == width - 1 || y == 0 || y == height - 1) { nxtmap[y, x] = true; continue; }
            
            int cnt = 0;
            for(int dx = -1; dx <= 1; dx++) for(int dy = -1; dy <= 1; dy++)
            {
                if(dx == 0 && dy == 0) continue;
                int cx = x + dx;
                int cy = y + dy;
                if(map[cy, cx]) { cnt += 1; }
            }
            nxtmap[y, x] = cnt >= shrinkThreashold;
        }
        map = nxtmap;
    }
    
    void Sync()
    {
        for(int y=0; y<height; y++) for(int x=0; x<width; x++)
        {
            targets[y, x].color = ColorHW(map[y, x]);
        }
    }
    
    Color ColorHW(bool v) => v ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1);
    
}
