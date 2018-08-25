
// ====================================================================================================================
// ====================================================================================================================
// ====================================================================================================================

template<typename T>
__global__ void Add(T* a, T* b, T* c)
{
    c[blockIdx.x] = a[blockIdx.x] + b[blockIdx.x];
}

void TestSumOfArray()
{
    const int N = 1000;
    HostArray<int> A(N);
    HostArray<int> B(N);
    HostArray<int> C(N);
    KernelArray<int> KA(N);
    KernelArray<int> KB(N);
    KernelArray<int> KC(N);
    for(int i=0; i<N; i++) A[i] = i + 1;
    for(int i=0; i<N; i++) B[i] = A[i] * A[i];
    KA <<= A;
    KB <<= B;
    Add<<<N, 1>>>(KA.pointer, KB.pointer, KC.pointer);
    C <<= KC;
    for(int i=0; i<N; i++) Log("%d + %d = %d\n", A[i], B[i], C[i]);
}

// ====================================================================================================================
// ====================================================================================================================
// ====================================================================================================================


template<typename T>
__global__ void PrefixSum(T* s, T* a)
{
    int tid = threadIdx.x;
    
    s[tid] = 0;
    for(int i=0; i<5; i++)
    {
        s[tid] += a[(tid + i) % blockDim.x];
    }
}

void TestThreadAccess()
{
    const int N = 1000;
    HostArray<int> A(N);
    HostArray<int> B(N);
    KernelArray<int> KA(N);
    KernelArray<int> KB(N);
    for(int i=0; i<N; i++) A[i] = i + 2;
    KA <<= A;
    PrefixSum<<<1, N>>>(KB.pointer, KA.pointer);
    B <<= KB;
    for(int i=0; i<N; i++) Log("sum of 5 elements: %d : %d\n", A[i], B[i]);
}
