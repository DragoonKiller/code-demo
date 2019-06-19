#include "SimpleDraw.hpp"

#include <iostream>

Canvas::Canvas(std::string const& title, point const& size) noexcept
{
    window = SDL_CreateWindow(title.c_str(), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, (int)size.x, (int)size.y, SDL_WINDOW_SHOWN);
    renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
    content = new std::set<RenderObject*>();

    SDL_SetMainReady();
    SDL_Init(SDL_INIT_VIDEO);
    window = SDL_CreateWindow("x", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 800, 600, SDL_WINDOW_SHOWN);
    renderer = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED);
}

Canvas::Canvas(Canvas&& x) noexcept
{
    window = x.window;
    renderer = x.renderer;
    content = x.content;
    x.window = nullptr;
    x.renderer = nullptr;
    x.content = nullptr;
}

Canvas::~Canvas() noexcept
{
    if (renderer) SDL_DestroyRenderer(renderer);
    if (window) SDL_DestroyWindow(window);
    delete content;
}

point Canvas::Size() noexcept
{
    int x, y;
    SDL_GetWindowSize(window, &x, &y);
    return point{ x, y };
}

void Canvas::Add(RenderObject* x)
{
    content->insert(x);
}

void Canvas::Remove(RenderObject* x)
{
    content->erase(x);
}

void Canvas::Render()
{
    SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
    SDL_RenderClear(renderer);
    for (auto i : *content) i->Render(*renderer, world2screen);
    SDL_RenderPresent(renderer);
}

bool Canvas::PollEvent(std::function<void(SDL_Event)> const& f)
{
    SDL_Event event;
    bool res = SDL_PollEvent(&event);
    if(res) f(event);
    if (event.type == SDL_QUIT) return false;
    return true;
}

Rect::Rect(point const& center, point const& size, SDL_Color color, float rot)
    : center(center), size(size), color(color), rotation(rot) { }

Rect::Rect(float x, float y, float w, float h, SDL_Color color, float rot)
    : center{ x, y }, size{ w, h }, color(color), rotation(rot) { }

SDL_Color Rect::getColor() { return color; }

void Rect::Render(SDL_Renderer& rd, std::function<point(point)> const& w2s)
{
    auto rsize = point{ size.x, -size.y };
    //  3  0
    //  1  2
    point rects[] = { size * 0.5f, size * -0.5f, rsize * 0.5f, rsize * -0.5f };
 
    for (auto& r : rects) r = w2s(center + r.rot(rotation));

    SDL_SetRenderDrawColor(&rd, color.r, color.g, color.b, color.a);
    SDL_RenderDrawLine(&rd, rects[0].x, rects[0].y, rects[2].x, rects[2].y);
    SDL_RenderDrawLine(&rd, rects[2].x, rects[2].y, rects[1].x, rects[1].y);
    SDL_RenderDrawLine(&rd, rects[1].x, rects[1].y, rects[3].x, rects[3].y);
    SDL_RenderDrawLine(&rd, rects[3].x, rects[3].y, rects[0].x, rects[0].y);
}
