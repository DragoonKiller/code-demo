#![allow(unused)]
#![allow(non_snake_case)]

extern crate image; use image::*;
extern crate imageproc; use imageproc::drawing::*;
extern crate ndarray; use ndarray::*;

#[macro_use] mod util; #[macro_use] use util::*;
mod geo; use geo::*;
mod random; use random::*;
mod imgutil; use imgutil::*;
mod perlinNoise; use perlinNoise::*;
mod worleyNoise; use worleyNoise::*;

use std::path::*;

// pub fn smooth(v : f32) -> f32 { v }
pub fn smoothPoly(v : f32) -> f32 { 6.0 * f32::powf(v, 5.0) - 15.0 * f32::powf(v, 4.0) + 10.0 * f32::powf(v, 3.0) }
pub fn smoothCos(v : f32) -> f32 { 0.5 * (1.0 - (v * pi).cos()) }
// pub fn smooth(v : f32) -> f32 { if v < 0.25 { 0.0 } else if v > 0.75 { 1.0 } else { (v - 0.25) * 2.0 } }

macro_rules! saveImg
{
    ($name:expr, $x:expr) => {{
        let pathStr = "./output/".to_owned() + $name + ".png";
        let path = Path::new(&pathStr);
        println!("{:?}", path);
        $x.save(path).unwrap();
    }}
}

#[test]
fn white() {
    let noise = rd2DU(256, 256);
    saveImg!("white", toGrayImg(&noise));
}

#[test]
fn perlinSqr() {
    let noise = perlinNoise((256, 256), 32, &smoothCos);
    fn dpoly(x : f32, mid : f32) -> f32 {
        if x <= mid { (1.0 / mid) * (x - mid) * x + x }
        else { -(1.0 / (1.0 - mid)) * (x - mid) * (x - 1.) + x }
    }
    saveImg!("perlin-sqr", toGrayImg(&noise.mapv(|x| dpoly(x, 0.3)).toU8()));
}

#[test]
fn perlinCos() {
    let noise = perlinNoise((256, 256), 16, &smoothCos);
    saveImg!("perlin-cos-linear", toGrayImg(&noise.toU8()));
    saveImg!("perlin-cos-sqrt", toGrayImg(&noise.mapv(|x| x.sqrt()).toU8()));
}

#[test]
fn perlinPoly() {
    let noise = perlinNoise((256, 256), 16, &smoothPoly);
    saveImg!("perlin-poly-linear", toGrayImg(&noise.toU8()));
    saveImg!("perlin-poly-sqrt", toGrayImg(&noise.mapv(|x| x.sqrt()).toU8()));
}

#[test]
fn perlinCompCos() {
    let noise = [
        perlinNoise((256, 256), 8, &smoothPoly),
        perlinNoise((256, 256), 16, &smoothPoly),
        perlinNoise((256, 256), 32, &smoothPoly),
        perlinNoise((256, 256), 64, &smoothPoly)
    ];
    let noisePerlin = (0.4 * &noise[0] + 0.3 * &noise[1] + 0.2 * &noise[2] + 0.1 * &noise[3]).toU8();
    saveImg!("perlin-composed-linear", toGrayImg(&noisePerlin));
}

#[test]
fn perlinCompPoly() {
    let noise = [
        perlinNoise((256, 256), 8, &smoothPoly),
        perlinNoise((256, 256), 16, &smoothPoly),
        perlinNoise((256, 256), 32, &smoothPoly),
        perlinNoise((256, 256), 64, &smoothPoly)
    ];
    let v = 0.5187900636758872; // (sum [i = 1 ..= 4] v^i) == 1
    let noisePerlin = (v * &noise[0] + v.powi(2) * &noise[1] + v.powi(3) * &noise[2] + v.powi(4) * &noise[3]).toU8();
    saveImg!("perlin-composed-polynomial", toGrayImg(&noisePerlin));
}

#[test]
fn worley() {
    let noise = worleyNoise((128, 128), (-10.0, 138.0), 50, 0).toU8();
    saveImg!("worley-1st", toGrayImg(&noise));
    let noise = worleyNoise((128, 128), (-10.0, 138.0), 50, 1).toU8();
    saveImg!("worley-2ed", toGrayImg(&noise));
    let noise = worleyNoise((128, 128), (-10.0, 138.0), 50, 2).toU8();
    saveImg!("worley-3rd", toGrayImg(&noise));
}

#[test]
fn worleySqr() {
    let noise = worleyNoise((128, 128), (-10.0, 138.0), 50, 0).mapv(|x| (x - 1.).powi(2)).toU8();
    saveImg!("worley-sqrt", toGrayImg(&noise));
}

#[test]
fn worleyDiff() {
    let noise = worleyNoiseDiff((128, 128), (-10.0, 138.0), 50).toU8();
    saveImg!("worley-diff", toGrayImg(&noise));
}

#[test]
fn worleyEdges() {
    let noise = worleyNoiseEdges((128, 128), (-10.0, 138.0), 50, 0.2, 1.0).toU8();
    saveImg!("worley-edges", toGrayImg(&noise));
}


fn main()
{
    
}
