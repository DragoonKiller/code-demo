#include <bits/stdc++.h>

#include <ft2build.h>


#include FT_FREETYPE_H

// for outlines and outline decomposion definitions and functions.
#include FT_OUTLINE_H

#include <FreeImagePlus.h>

using namespace std;

typedef unsigned char byte;

// ==================================================================================
// ==================================================================================
// ==================================================================================

const int w = 2000;
const int h = 2000;

struct color
{
    // not RGBA but BGRA though.
    byte b;
    byte g;
    byte r;
    byte a;
};

color c[w + 1][h + 1];

// ==================================================================================
// ==================================================================================
// ==================================================================================

void TypeSectionBitmap()
{
    FT_Library lib;
    FT_Init_FreeType(&lib);
    
    FT_Face face;
    int erx = FT_New_Face(lib, "./res/LiberationMono-Regular.ttf", 0, &face);
    if(erx) { cout<<"Load face failed."; } else { cout<<"Load face succeed."<<endl; }
    
    // Specify a font size.
    // Different size my reffer to different glyph.
    // For vectorial fonts, this may not be important though.
    // second and third param: 32 bit fixed point number stored in 32 bit format.
    // so we have this scale numbered 12, which means we want a character of 12/72 inch width and height.
    // fourth and fifth param: ppi (Pixel per Inch) for screen's width and height.
    // so now this character takes 300 * (12 / 72) = 50 pixels width and height.
    FT_Set_Char_Size(face, 12<<6, 12<<6, 300, 300);
    
    auto indexA = FT_Get_Char_Index(face, 'A');
    
    cout<<"The glyph index of A is: "<<indexA<<endl;
    
    FT_Load_Glyph(face, indexA, FT_LOAD_DEFAULT);
    
    char* fmt = (char*)&(face->glyph->format);
    cout<<"Get a glyph of type: "<<fmt[3]<<fmt[2]<<fmt[1]<<fmt[0]<<endl;
    
    // The rendered bitmap glyph size.
    // rendered object will be stored into face->glyph->buffer.
    FT_Set_Pixel_Sizes(face, 200, 200);
    
    FT_Render_Glyph(face->glyph, FT_RENDER_MODE_NORMAL);
    
    FT_Bitmap& pic = face->glyph->bitmap;
    
    cout<<"Get a rendered picture with width: "<<face->glyph->bitmap.width<<" height: "<<face->glyph->bitmap.rows<<endl;
    
    cout<<"Bitmap store to an "<<w<<" * "<<h<<" array."<<endl;
    for(int i=0; i<h; i++) for(int j=0; j<w; j++)
    {
        int mi = (int)floor((double) i / h * pic.rows);
        int mj = (int)floor((double) j / w * pic.width);
        c[i][j].r = face->glyph->bitmap.buffer[mi * pic.width + mj];
        c[i][j].g = face->glyph->bitmap.buffer[mi * pic.width + mj];
        c[i][j].a = 255;
    }
    
    cout<<"Glyph count: "<<face->num_glyphs<<endl;
}

void ImageSectionBitmap()
{
    FreeImage_Initialise();
    
    // The last param: bits per pixel. No idea how it works...
    // 32 bits represents BGRA each in 8 bits.
    shared_ptr<fipImage> x = make_shared<fipImage>(FIT_BITMAP, w, h, 32);
    
    cout<<"Created is validate: "<<x->isValid()<<endl;
    
    for(int i=0; i<h; i++) for(int j=0; j<w; j++)
    {
        // The coordinate is exactly x and y,
        // where x representing columns from left to right, y representing rows from top to bottom.
        x->setPixelColor(j, i, (RGBQUAD*)&c[i][j]);
    }
    
    // The coordinate system of array c takes bottom-left corner as origin,
    // while the picture picks the top-left corner.
    x->flipVertical();
    
    cout<<"Image output validate: "<<x->save("./res/output.bmp")<<endl;
    
    FreeImage_DeInitialise();
}

// ==================================================================================
// ==================================================================================
// ==================================================================================

FT_Vector lastVec;

int MoveTo(const FT_Vector* v, void*)
{
    // they are storing as long.
    // same as below.
    printf("Move to (%ld,%ld)\n", v->x, v->y);
    lastVec = *v;
    return 0;
};

int LineTo(const FT_Vector* v, void*)
{
    printf("Line from (%ld,%ld) to (%ld,%ld)\n", lastVec.x, lastVec.y, v->x, v->y);
    
    double ax = lastVec.x;
    double ay = lastVec.y;
    double bx = v->x;
    double by = v->y;
    
    for(int i=0; i<10000; i++)
    {
        double x = (bx - ax) * i / 9999. + ax;
        double y = (by - ay) * i / 9999. + ay;
        c[int(y)][int(x)].r = c[int(y)][int(x)].a = 255;
        c[int(y+1)][int(x)].r = c[int(y+1)][int(x)].a = 255;
        c[int(y)][int(x+1)].r = c[int(y)][int(x+1)].a = 255;
        c[int(y+1)][int(x+1)].r = c[int(y+1)][int(x+1)].a = 255;
    }
    
    lastVec = *v;
    return 0;
};

int ConicTo(const FT_Vector* g, const FT_Vector* v, void*)
{
    printf("Conic from (%ld,%ld) control: (%ld,%ld), destination: (%ld,%ld)\n", lastVec.x, lastVec.y, g->x, g->y, v->x, v->y);
    
    double ax = lastVec.x;
    double ay = lastVec.y;
    double cx = g->x;
    double cy = g->y;
    double bx = v->x;
    double by = v->y;
    
    for(int i=0; i<10000; i++)
    {
        double r = i / 9999.;
        double p = 1.-r;
        double x = ax * p * p + 2. * cx * p * r + bx * r * r;
        double y = ay * p * p + 2. * cy * p * r + by * r * r;
        c[int(y)][int(x)].r = c[int(y)][int(x)].a = 255;
        c[int(y+1)][int(x)].r = c[int(y+1)][int(x)].a = 255;
        c[int(y)][int(x+1)].r = c[int(y)][int(x+1)].a = 255;
        c[int(y+1)][int(x+1)].r = c[int(y+1)][int(x+1)].a = 255;
    }
    
    lastVec = *v;
    return 0;
};

int CubicTo(const FT_Vector* cf, const FT_Vector* ct, const FT_Vector* t, void*)
{
    printf("Cubic from (%ld,%ld) control: (%ld,%ld) (%ld,%ld), destination (%ld,%ld)\n", lastVec.x, lastVec.y, cf->x, cf->y, ct->x, ct->y, t->x, t->y);
    lastVec = *t;
    return 0;
};

void TypeSectionOutline()
{
    FT_Library lib;
    FT_Init_FreeType(&lib);
    
    FT_Face face;
    FT_New_Face(lib, "./res/LiberationMono-Regular.ttf", 0, &face);
    
    // Don't use just FT_Load_Glyph.
    // Or it will not load contours.
    FT_Load_Char(face, 'A', FT_LOAD_NO_SCALE);
    
    printf("Begin:\n");
    
    auto& outline = face->glyph->outline;
    
    FT_Outline_Funcs fc;
    fc.delta = -200;
    fc.shift = 0;
    fc.move_to = MoveTo;
    fc.line_to = LineTo;
    fc.conic_to = ConicTo;
    fc.cubic_to = CubicTo;
    
    FT_Outline_Decompose(&outline, &fc, (void*)1);
    
    printf("End.\n");
}

// The only difference from Bitmap is not flipping vertical.
void ImageSectionOutline()
{
    FreeImage_Initialise();
    
    // The last param: bits per pixel. No idea how it works...
    // 32 bits represents BGRA each in 8 bits.
    shared_ptr<fipImage> x = make_shared<fipImage>(FIT_BITMAP, w, h, 32);
    
    cout<<"Created is validate: "<<x->isValid()<<endl;
    
    for(int i=0; i<h; i++) for(int j=0; j<w; j++) x->setPixelColor(j, i, (RGBQUAD*)&c[i][j]);
    
    cout<<"Image output validate: "<<x->save("./res/output.bmp")<<endl;
    
    FreeImage_DeInitialise();   
}

// ==================================================================================
// ==================================================================================
// ==================================================================================

int main()
{
    // TypeSectionBitmap();
    // ImageSectionBitmap();
    
    TypeSectionOutline();
    ImageSectionOutline();
    
    return 0;
}

