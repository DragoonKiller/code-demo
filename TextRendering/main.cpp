#include <bits/stdc++.h>

#include <ft2build.h>
#include FT_FREETYPE_H

#include <FreeImagePlus.h>

using namespace std;

typedef unsigned char byte;

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

void TypeSection()
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

void ImageSection()
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

int main()
{
    TypeSection();
    ImageSection();
    
    return 0;
}

