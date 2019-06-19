#pragma once

#define SDL_MAIN_HANDLED
#include "SDL.h"
#include "geo.h"

#include <string>
#include <set>
#include <functional>

class RenderObject;

class Canvas final
{
    SDL_Window* window;
    SDL_Renderer* renderer;
    std::set<RenderObject*>* content;

    public:
    
    std::function<point(point)> world2screen = [](point a) { return point{ a.x, -a.y }; };

    Canvas(std::string const& title, point const& size) noexcept;
    Canvas(Canvas&& x) noexcept;
    Canvas(Canvas const&) = delete;
    ~Canvas() noexcept;

    point Size() noexcept;
    void Add(RenderObject*);
    void Remove(RenderObject*);
    void Render();
    bool PollEvent(std::function<void(SDL_Event)> const& f = [](auto x){});
};

class RenderObject
{
    public:

    /// Get color rendering this object.
    virtual SDL_Color getColor() = 0;

    /// Function to render the object...
    virtual void Render(SDL_Renderer&, std::function<point(point)> const& w2s) = 0;
};

class Rect : public RenderObject
{
    public:
    point center;
    point size;
    float rotation;
    SDL_Color color;

    Rect() { }
    Rect(point const& center, point const& size, SDL_Color = SDL_Color{ 255, 255, 255, 255 }, float rotation = 0.f);
    Rect(float x, float y, float w, float h, SDL_Color = SDL_Color{ 255, 255, 255, 255 }, float rotation = 0.f);
    virtual SDL_Color getColor() override;
    virtual void Render(SDL_Renderer&, std::function<point(point)> const& w2s) override;
};

