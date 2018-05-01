## C# SIMD (Vectors) example

SIMD stands for *Single Instruction Multiple Data*.
Modern CPUs commonly supports SIMD for calculation acceleration. C# provides a standard library System.Numerics.Vectors (which is not included in standard assembly set so that you have to download it manually) for SIMD acceleration.

To make Vectors fully operational, you might
1. Set build option "Optimize code" to true, or write a label \<Optimize\> in .csproj file manually.
2. Set target platform to "x64", or write a label \<PlatformTarget>\ as "x64" in .csproj file manually. Not sure what will happen in 32 bit CPU.

A specific algorithm design in required when you come to use SIMD and C# Vectors, they are not always accelerating your program if the algorithm does not fit to SIMD. SIMD data should be transfered between oridnary registers, SIMD registers and memories (including caches), it's recommanded to minimal the transfer operation for performance, to make CPU concentrate on calculating instead of communicating with memory.

TODO:

Multi-thread with SIMD. This shall be like the ordinary multi-thread calculation, tranfering the overhead from CPU calculation to RAM access.
