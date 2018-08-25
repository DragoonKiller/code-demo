#pragma once

#include <vector>
#include <climits>
#include <random>
#include <cmath>

// ======================== Utility =========================

static float RandReal()
{
    static std::default_random_engine gen;
    static std::uniform_real_distribution<float> dis(0, 1);
    return dis(gen);
}

static int RandInt()
{
    static std::default_random_engine gen;
    static std::uniform_int_distribution<int> dis(INT_MIN, INT_MAX);
    return dis(gen);
}

static uint RandUInt()
{
    static std::default_random_engine gen;
    static std::uniform_int_distribution<unsigned> dis(0, UINT_MAX);
    return dis(gen);
}

template<typename T> struct HostArray;
template<typename T> struct KernelArray;

template<typename T>
struct KernelArray
{
    T* pointer;
    const int size;
    explicit KernelArray(int const& sz) : size(sz)
    {
        cudaMalloc((void**)(&pointer), sz * sizeof(T));
    }
    explicit KernelArray(HostArray<T> const& v) : size(v.size)
    {
        cudaMalloc((void**)(&pointer), v.size * sizeof(T));
        *this <<= v;
    }
    ~KernelArray() { cudaFree(pointer); }
};

template<typename T>
struct HostArray
{
    T* pointer;
    const int size;
    explicit HostArray(std::vector<T> const& v) : pointer(&v[0]), size(v) {  }
    explicit HostArray(KernelArray<T> const& v) : size(v.size), pointer(new T[v.size])
    {
        *this <<= v;
    }
    explicit HostArray(int const& sz) : size(sz), pointer(new T[sz]) { }
    
    ~HostArray() { delete[] pointer; }
    T& operator[](int const& k) { return pointer[k]; }
    T const& operator[](int const& k) const { return pointer[k]; }
};

template<typename T>
void operator<<=(KernelArray<T>& a, HostArray<T> const& b)
{
    cudaMemcpy(a.pointer, b.pointer, min(a.size, b.size) * sizeof(T), cudaMemcpyHostToDevice);
}

template<typename T>
void operator<<=(HostArray<T>& a, KernelArray<T> const& b)
{
    cudaMemcpy(a.pointer, b.pointer, min(a.size, b.size) * sizeof(T), cudaMemcpyDeviceToHost);
}

template<typename T>
void operator<<=(std::vector<T>& a, KernelArray<T> const& b)
{
    a.resize(b.size);
    cudaMemcpy(&a[0], b.pointer, min(a.size(), b.size * sizeof(T)), cudaMemcpyDeviceToHost);
}

template<typename T>
void operator<<=(KernelArray<T>& a, std::vector<T> const& b)
{
    a = KernelArray<T>(b.size());
    a <<= HostArray<T>(b, b.size());
}
