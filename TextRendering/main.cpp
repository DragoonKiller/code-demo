#include <bits/stdc++.h>

#include <ft2build.h>
#include FT_FREETYPE_H

#include <FreeImagePlus.h>

using namespace std;

typedef unsigned char byte;

const int w = 200;
const int h = 200;

struct color
{
    // not RGBA but BGRA though.
    byte b;
    byte g;
    byte r;
    byte a;
};

color c[201][201];

void TypeSection()
{
    FT_Library lib;
    FT_Init_FreeType(&lib);
    
    FT_Face face;
    int erx = FT_New_Face(lib, "./res/LiberationMono-Regular.ttf", 0, &face);
    if(erx) { cout<<"Load face failed."; } else { cout<<"Load face succeed."<<endl; }
    
    FT_Set_Char_Size(face, 12<<8, 12<<8, 300, 300);
    
    auto indexA = FT_Get_Char_Index(face, 'A');
    
    cout<<"The glyph index of A is: "<<indexA<<endl;
    
    FT_Load_Glyph(face, indexA, FT_LOAD_DEFAULT);
    
    char* fmt = (char*)&(face->glyph->format);
    cout<<"Get a glyph of type: "<<fmt[3]<<fmt[2]<<fmt[1]<<fmt[0]<<endl;
    
    // The rendered bitmap glyph size.
    // rendered object will be stored into face->glyph->buffer.
    FT_Set_Pixel_Sizes(face, 128, 128);
    
    FT_Render_Glyph(face->glyph, FT_RENDER_MODE_NORMAL);
    
    FT_Bitmap& pic = face->glyph->bitmap;
    
    cout<<"Get a rendered picture with width: "<<face->glyph->bitmap.width<<" height: "<<face->glyph->bitmap.rows<<endl;
    
    cout<<"Bitmap store to an "<<w<<" * "<<h<<" array."<<endl;
    for(int i=0; i<h; i++) for(int j=0; j<w; j++)
    {
        int mi = (int)round((double) i / h * pic.rows);
        int mj = (int)round((double) j / w * pic.width);
        c[i][j].r = face->glyph->bitmap.buffer[mi * pic.width + mj];
        c[i][j].g = face->glyph->bitmap.buffer[mi * pic.width + mj];
        c[i][j].a = 255;
    }
    
    cout<<"Glyph count: "<<face->num_glyphs<<endl;
}

void ImageSection()
{
    FreeImage_Initialise();
    
    shared_ptr<fipImage> x = make_shared<fipImage>(FIT_BITMAP, w, h, 24);
    
    cout<<"Created is validate: "<<x->isValid()<<endl;
    
    for(int i=0; i<h; i++) for(int j=0; j<w; j++)
    {
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

