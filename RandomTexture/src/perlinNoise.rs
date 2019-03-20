
use ndarray::*;

use crate::util::*;
use crate::random::*;
use crate::geo::*;

pub fn perlinNoise(
    shape : (usize, usize), freq : usize,
    smooth : &Fn(f32)->f32,
) -> Array2<f32> {
    let (gh, gw) = (freq, freq);
    let (dh, dw) = (256, 256);
    let rdGrad = rd2DPt(gh, gw);
    let mut dst = Array::zeros(shape); // RGBA
    for (i, j) in iter2D(dh, dw) {
        let cur = Pt::new(
            i as f32 / (dh - 1) as f32 * (gh - 3) as f32 + 1.0,
            j as f32 / (dw - 1) as f32 * (gw - 3) as f32 + 1.0,
        );
        let anchorBase = Pt::new(cur.x.floor(), cur.y.floor());
        let (bi, bj) = ( cur.x as usize, cur.y as usize );
        let getWeight = |anchor : Pt, grad : Pt| grad & (anchor >> cur); 
        let weight = [
            getWeight(anchorBase, rdGrad[[bi, bj]]),
            getWeight(anchorBase + Pt::y(), rdGrad[[bi, bj + 1]]),
            getWeight(anchorBase + Pt::x(), rdGrad[[bi + 1, bj]]),
            getWeight(anchorBase + Pt::x() + Pt::y(), rdGrad[[bi + 1, bj + 1]])
        ];
        let xlerp = |l : f32, r : f32, v : f32| l + (r - l) * smooth(v);
        let d = cur - anchorBase;
        let v = xlerp( xlerp(weight[0], weight[1], d.y), xlerp(weight[2], weight[3], d.y), d.x);
        let v = v * 0.5 + 0.5; // normalize
        let lem = (v * 255.0) as u8;
        dst[[i, j]] = v;
    }
    dst
}
