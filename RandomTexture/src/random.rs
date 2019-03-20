
extern crate rand; use rand::*;
extern crate ndarray; use ndarray::*;
use crate::util::*;
use crate::geo::*;

pub fn rdFl() -> f32 { thread_rng().gen::<f32>() }
pub fn rdU() -> u8 { thread_rng().gen::<u8>() }
pub fn rdPt() -> Pt { Pt::x().rot(rdFl() * 2.0 * pi) }

pub fn rd2DF(w : usize, h : usize) -> Array2<f32> {
    let mut res = Array::zeros((h, w));
    for (x, y) in iter2D(h, w) { res[[x, y]] = rdFl(); }
    res
}

pub fn rd2DU(w : usize, h : usize) -> Array2<u8> {
    let mut res = Array::zeros((h, w));
    for (x, y) in iter2D(h, w) { res[[x, y]] = rdU(); }
    res
}

pub fn rd2DPt(w : usize, h : usize) -> Array2<Pt> {
    let mut res = Array::zeros((h, w));
    for (x, y) in iter2D(h, w) { res[[x, y]] = rdPt().norm(); } 
    res
}

pub fn rdImg(w : usize, h : usize) -> [Array2<u8>; 4] {
    [rd2DU(w, h), rd2DU(w, h), rd2DU(w, h), rd2DU(w, h)]
}

// Random points in [ptRange.0 .. ptRange.1] [ptRange.0 .. ptRange.1]
pub fn randPtGroup(ptRange : (f32, f32), ptCnt : usize) -> Vec<Pt> {
    let mut pts = vec![Pt::zero(); ptCnt];
    for mut i in &mut pts {
        *i = Pt::new(
            rdFl() * (ptRange.1 - ptRange.0) + ptRange.0,
            rdFl() * (ptRange.1 - ptRange.0) + ptRange.0,
        ); 
    }
    pts
}
