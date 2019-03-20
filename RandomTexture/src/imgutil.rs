
extern crate image; use image::*;
extern crate imageproc; use imageproc::drawing::*;
extern crate ndarray; use ndarray::*;

use crate::util::*;

pub fn read(name : &str) -> DynamicImage {
    let mut src = open(name).unwrap();
    let (h, w) = (src.height() as usize, src.width() as usize);
    println!("h {} w {} ct {:?}", h, w, src.color());
    src
}

pub fn toImg(a : [Array2<u8>; 4]) -> RgbaImage {
    let (h, w) = (a[0].shape()[0], a[0].shape()[1]);
    let mut res = RgbaImage::new(h as u32, w as u32);
    for (x, y) in iter2D(h, w) {
        res.put_pixel(x as u32, y as u32, Rgba([
            a[0][[x, y]],
            a[1][[x, y]],
            a[2][[x, y]],
            a[3][[x, y]]
        ]));
    }
    res
}

pub fn toGrayImg(a : &Array2<u8>) -> GrayImage {
    let (h, w) = (a.shape()[0], a.shape()[1]);
    let mut res = GrayImage::new(h as u32, w as u32);
    for (x, y) in iter2D(h, w) {
        res.put_pixel(x as u32, y as u32, Luma([ a[[x, y]] ]));
    }
    res
}

pub fn toarr(a : DynamicImage) -> [Array2<u8>; 4] {
    let (h, w) = (a.height() as usize, a.width() as usize);
    let mut res = vec4(Array::zeros((h, w)));
    for (x, y) in iter2D(h, w) {
        res[0][[x, y]] = a.get_pixel(x as u32, y as u32)[0];
        res[1][[x, y]] = a.get_pixel(x as u32, y as u32)[0];
        res[2][[x, y]] = a.get_pixel(x as u32, y as u32)[0];
        res[3][[x, y]] = a.get_pixel(x as u32, y as u32)[0];
    }
    res
}

pub trait ToU8 { fn toU8(&self) -> Array2<u8>; }
impl ToU8 for Array2<f32> {
    fn toU8(&self) -> Array2<u8> {
        let mut x = Array::zeros((self.shape()[0], self.shape()[1]));
        for (i, j) in iter2D(self.shape()[0], self.shape()[1]) { x[[i, j]] = (self[[i, j]] * 255.0) as u8; }
        x
    }
}
pub trait ToF32 { fn toF32(&self) -> Array2<f32>; }
impl ToF32 for Array2<u8> {
    fn toF32(&self) -> Array2<f32> {
        let mut x = Array::zeros((self.shape()[0], self.shape()[1]));
        for (i, j) in iter2D(self.shape()[0], self.shape()[1]) { x[[i, j]] = self[[i, j]] as f32 / 255.0; }
        x
    }
}
