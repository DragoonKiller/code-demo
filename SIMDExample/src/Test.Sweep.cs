using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

partial class Test
{
    /// <summary>
    /// Given 2 long vectors A and B, calculate (A+B) * (A-B).
    /// Compile option "Optimize code" or "Optimize" will reduce the time usage of ordinary method by half,
    /// and this shall not affect SIMD options.
    /// Also, on DragoonKiller's I7-7700HQ, set settingLength = 12500000, several performance test data are:
    /// optimization:   off         on
    /// SIMD lambda :   90.2535ms   62.2670ms
    /// SIMD raw    :   28.2238ms   29.8181ms
    /// Ordinary    :   103.929ms   36.0449ms
    /// so conclusion comes to that lambdas and functions are extremely slow.
    /// empirically, the more complex calculation done in SIMD without conversion,
    ///     the better performance it can brings.
    /// </summary>
    public static void Sweep()
    {
        void PrepareArray(out float[] A, int length, int seed)
        {
            Random rd = new Random(seed);
            A = new float[length];
            for(int i=0; i<A.Length; i++) A[i] = rd.Next();
        }
        
        // ============================================================================================================
        
        void RunSIMDWithLambda(out float[] ans, float[] A, float[] B)
        {
            /// <summary>
            /// Notice that parameter *target* can be the same as *H*.
            /// </summary>
            void SIMDCommand(
                float[] target,
                float[] H,
                float[] K,
                int length,
                Func<Vector<float>, Vector<float>, Vector<float>> vecs,
                Func<float,float,float> nums)
            {
                if(K == target || K == H) throw new ArgumentException("Second operand cannot be the resault array.");
                int cnt = Vector<float>.Count;
                int begin = 0;
                while(true)
                {
                    if(begin + cnt <= length)
                    {
                        Vector<float> a = new Vector<float>(H, begin);
                        Vector<float> b = new Vector<float>(K, begin);
                        Vector<float> c = vecs(a, b);
                        c.CopyTo(target, begin);
                    }
                    else break;
                    begin += cnt;
                }
                for(int i=begin; i<length; i++) target[i] = nums(H[i], K[i]);
            }
            
            void Add(float[] target, float[] H, float[] K, int length)
                => SIMDCommand(target, H, K, length, (a, b) => a + b, (a, b) => a + b);
            
            void Sub(float[] target, float[] H, float[] K, int length)
                => SIMDCommand(target, H, K, length, (a, b) => a - b, (a, b) => a - b);
            
            void Mult(float[] target, float[] H, float[] K, int length)
                => SIMDCommand(target, H, K, length, (a, b) => a * b, (a, b) => a * b);
            
            int len = A.Length;
            ans = new float[len];
            var temp = new float[len];
            Add(temp, A, B, len);
            Sub(ans, A, B, len);
            Mult(ans, ans, temp, len);
        }
        
        // ============================================================================================================
        
        void RunSIMDRaw(out float[] ans, float[] A, float[] B)
        {
            int len = A.Length;
            ans = new float[len];
            int cnt = Vector<float>.Count;
            int begin = 0;
            while(true)
            {
                if(begin + cnt <= len)
                {
                    Vector<float> a = new Vector<float>(A, begin);
                    Vector<float> b = new Vector<float>(B, begin);
                    ((a + b) * (a - b)).CopyTo(ans, begin);
                }
                else break;
                begin += cnt;
            }
            for(int i=begin; i<len; i++) ans[i] = ((A[i] + B[i]) * (A[i] - B[i]));
        }
        
        // ============================================================================================================
        
        void RunOrdinary(out float[] ans, float[] A, float[] B)
        {
            ans = new float[A.Length];
            for(int i=0; i<A.Length; i++)
            {
                ans[i] = (A[i] - B[i]) * (A[i] + B[i]);
            }
        }
        
        // ============================================================================================================
        
        Stopwatch watch = new Stopwatch();
        void TestTime(string name, Action f)
        { 
            watch.Reset();
            watch.Start();
            f();
            Console.WriteLine("time usage of {0} : {1}ms", name, watch.Elapsed.TotalMilliseconds);
        }
        
        // ============================================================================================================
        const int settingLength = 25000000;
        
        float[] L, R, T, G, P;
        PrepareArray(out L, settingLength, 1);
        PrepareArray(out R, settingLength, -1);
        
        TestTime("RunSIMD with lambda", () => RunSIMDWithLambda(out T, L, R));
        TestTime("RunSIMD raw", () => RunSIMDRaw(out P, L, R));
        TestTime("RunOrdinary", () => RunOrdinary(out G, L, R));
        
        for(int i=0; i<settingLength; i++)
        {
            Debug.Assert(T[i] == P[i]);
            Debug.Assert(P[i] == G[i]);
        }
    }
    
    
    
    
    
}
