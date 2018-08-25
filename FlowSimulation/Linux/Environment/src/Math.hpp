#pragma once

#include <cmath>

typedef float real; // might be changed to long double for precision...

#define CudaDef __host__ __device__

struct Point
{
    real x, y;
    CudaDef real len2() const { return x * x + y * y; }
    CudaDef real len() const { return sqrt(len2()); }
    CudaDef real a() const { return atan2(y, x); }
    CudaDef Point operator()(Point const& v) const { return Point { v.x - x, v.y - y }; }
};

static const Point zero = {0, 0};
static const Point one = {1, 1};
static const Point right = {1, 0};
static const Point left = {-1, 0};
static const Point up = {0, 1};
static const Point down = {0, -1};

/// Epsilon for accuracy controlling.
const real eps = 1e-6;
const real heps = 1e-12;

/// Real number equality.
template<typename T> CudaDef 
static bool eq(T const& a, T const& b) { return std::abs(a - b) < eps; }
template<typename T> CudaDef 
static bool heq(T const& a, T const& b) { return std::abs(a - b) < heps; }

/// Add operator.
CudaDef inline static Point operator+(Point const& a, Point const& b) { return Point { a.x + b.x, a.y + b.y }; }

/// Substract operator.
CudaDef inline static Point operator-(Point const& a, Point const& b) { return b(a); }

/// Cross product operator.
CudaDef inline static real operator^(Point const& a, Point const& b) { return a.x * b.y - a.y * b.x; }

/// Dot product operator.
CudaDef inline static real operator&(Point const& a, Point const& b) { return a.x * b.x + a.y * b.y; }

/// Scaler scale operator.
CudaDef inline static Point operator*(Point const& a, real const& b) { return Point { a.x * b, a.y * b }; }
CudaDef inline static Point operator*(real const& a, Point const& b) { return b * a; }

/// Vector scale operator.
CudaDef inline static Point operator*(Point const& a, Point const& b) { return Point { a.x * b.x, a.y * b.y }; }

/// Negative opeartor.
CudaDef inline static Point operator-(Point const& a) { return Point { -a.x, -a.y }; }

/// Rotate operator.
CudaDef inline static Point operator<<(Point const& c, real a) { return Point { c.x * cos(a) - c.y * sin(a), c.x * sin(a) + c.y * cos(a) }; }
CudaDef inline static Point operator>>(Point const& c, real a) { return c << (-a); }

/// Equality.
CudaDef inline static bool operator==(Point const& a, Point const& b) { return eq(a.x, b.x) && eq(a.y, b.y); }
CudaDef inline static bool operator!=(Point const& a, Point const& b) { return !(a == b); }

/// Compare operator. X first component, Y second.
/// Notice here we use operator==(...) instead of eq(...) since close X coords should not be considered equal.
CudaDef inline static bool operator<(Point const& a, Point const& b) { return a.x == b.x ? a.y < b.y : a.x < b.x; }
CudaDef inline static bool operator>(Point const& a, Point const& b) { return a.x == b.x ? a.y > b.y : a.x > b.x; }
CudaDef inline static bool operator>=(Point const& a, Point const& b) { return !(a < b); }
CudaDef inline static bool operator<=(Point const& a, Point const& b) { return !(a > b); }

/// Assign operator.
CudaDef inline static Point& operator+=(Point& a, Point const& b) { a = a + b; return a; }
CudaDef inline static Point& operator-=(Point& a, Point const& b) { a = a - b; return a; }
CudaDef inline static Point& operator*=(Point& a, Point const& b) { a = a * b; return a; }
CudaDef inline static Point& operator*=(Point& a, real const& b) { a = a * b; return a; }

/// Normalize.
CudaDef inline static Point Norm(Point const& v) { return eq(v.len2(), 0.0f) ? zero : (v * (1 / v.len())); }

CudaDef inline static float clamp(float x, float b, float t) { return x < b ? b : x > t ? t : x; }

#undef CudaDef
