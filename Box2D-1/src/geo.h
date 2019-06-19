#pragma once

#include "Box2D/Box2D.h"

#include <cmath>
#include <initializer_list>
#include <exception>

static float eps = 1e-7f;
static bool eq(float a, float b) { return abs(a - b) < eps; }
static bool sqr(float const& x) { return x * x; }

template<class T> static T sgn(T const& v) { return v < 0 ? -1 : v > 0 ? 1 : 0; }

const float pi = 3.1415926535897932384626433832795f;

struct point
{
    float x, y;
    point(float x, float y) : x(x), y(y) { }
    point() : x(0), y(0) { }
    template<typename T> point(std::initializer_list<T> a) : x(*a.begin()), y(*std::next(a.begin()))
    { if ((int)(a.end() - a.begin()) != 2) throw new std::exception("wrong initalizer list length"); }
    float len2() const { return x * x + y * y; }
    float len() const { return sqrt(len2()); }
    float a() const { return atan2(y, x); }
    point norm() const { if (eq(len(), 0)) return { 0, 0 }; return { x / len(), y / len() }; }
    point rot(float a) const { return { x * cos(a) - y * sin(a), x * sin(a) + y * cos(a) }; }
    point rot90() const { return { -y, x }; }
    point operator+(point const& b) const { return { x + b.x, y + b.y }; }
    point operator-(point const& b) const { return { x - b.x, y - b.y }; }
    point operator()(point const& b) const { return { b.x - x, b.y - y }; }
    float operator^(point const& b) const { return x * b.y - y * b.x; }
    float operator&(point const& b) const { return x * b.x + y * b.y; }
    point operator/(float const& t) const { return { x / t, y / t }; }
    bool operator==(point const& t) const { return eq(x, t.x) && eq(y, t.y); }
    bool operator!=(point const& t) const { return !(*this == t); }
    float afrom(point const& t) const { return atan2(t ^ *this, t & *this); }
    point& operator-=(point const& t) { return *this = *this - t; }
    point& operator+=(point const& t) { return *this = *this + t; }
    point operator*=(float const& t) { return *this = point{ x * t, y * t }; }
    point operator/=(float const& t) { return *this = point{ x / t, y / t }; }
    
    b2Vec2 tob2() const { return b2Vec2(x, y); }
    point(b2Vec2 const& v) : x(v.x), y(v.y) { }
};
inline static point operator*(point const& a, float t) { return { a.x * t, a.y * t }; }
inline static point operator*(float t, point const& a) { return { a.x * t, a.y * t }; }



struct Coord final
{
    point x;
    point y;
    point wrap(point const& a) const
    {
        float ax = a & (x.norm() / x.len());
        float ay = a & (y.norm() / y.len());
        return point{ ax, ay };
    }
    point recover(point const& a) const { return a.x * x + a.y * y; }
};