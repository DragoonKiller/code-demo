use ndarray::*;

use crate::util::*;
use crate::geo::*;
use crate::random::*;

pub fn worleyNoiseSeeded(shape : (usize, usize), pts : &Vec<Pt>, k : usize) -> Array2<f32> {
    let (h, w) = shape;
    let mut pts = pts.clone();
    let mxv = (w.pow(2) + h.pow(2)) as f32;
    let (mut mid, mut mxd) = (mxv, 0.0);
    let mut dst = Array::zeros(shape);
    for (i, j) in iter2D(h, w) {
        let cur = Pt::new(i as f32, j as f32);
        pts.sort_by(|&x, &y| { (x >> cur).len().partial_cmp(&(y >> cur).len()).unwrap() });
        dst[[i, j]] = (pts[k] >> cur).len();
        mid = f32::min(mid, dst[[i, j]]);
        mxd = f32::max(mxd, dst[[i, j]]);
    }
    for (x, y) in iter2D(h, w) { dst[[x, y]] = (dst[[x, y]] - mid) / (mxd - mid); }
    dst
}

pub fn worleyNoiseDiffSeeded(shape : (usize, usize), pts : &Vec<Pt>) -> Array2<f32>  {
    let (h, w) = shape;
    let mut pts = pts.clone();
    let mxv = (w.pow(2) + h.pow(2)) as f32;
    let (mut mid, mut mxd) = (mxv, 0.0);
    let mut dst = Array::zeros(shape);
    for (i, j) in iter2D(h, w) {
        let cur = Pt::new(i as f32, j as f32);
        pts.sort_by(|&x, &y| { (x >> cur).len().partial_cmp(&(y >> cur).len()).unwrap() });
        let (la, lb) = ((pts[0] >> cur).len(), (pts[1] >> cur).len()); 
        dst[[i, j]] = (la - lb).abs();
        mid = f32::min(mid, dst[[i, j]]);
        mxd = f32::max(mxd, dst[[i, j]]);
    }
    for (x, y) in iter2D(h, w) { dst[[x, y]] = (dst[[x, y]] - mid) / (mxd - mid); }
    dst
}

pub fn worleyNoiseEdgesSeeded(shape : (usize, usize), pts : &Vec<Pt>, lbound : f32, rbound : f32) -> Array2<f32> {
    let (h, w) = shape;
    let mut pts = pts.clone();
    let mxv = (w.pow(2) + h.pow(2)) as f32;
    let mut dst = Array::zeros(shape);
    for (i, j) in iter2D(h, w) {
        let cur = Pt::new(i as f32, j as f32);
        pts.sort_by(|&x, &y| { (x >> cur).len().partial_cmp(&(y >> cur).len()).unwrap() });
        let (la, lb) = ((pts[0] >> cur).len(), (pts[1] >> cur).len()); 
        let dx = (la - lb).abs();
        let dx = (dx - lbound) / (rbound - lbound);
        let dx = dx.max(0.0).min(1.0);
        dst[[i, j]] = 1.0 - dx;
    }
    dst
}

pub fn worleyNoiseEdges(shape : (usize, usize), ptRange : (f32, f32), ptCnt : usize, lbound : f32, rbound : f32) -> Array2<f32> {
    worleyNoiseEdgesSeeded(shape, &randPtGroup(ptRange, ptCnt), lbound, rbound)
}

pub fn worleyNoiseDiff(shape : (usize, usize), ptRange : (f32, f32), ptCnt : usize) -> Array2<f32> {
    worleyNoiseDiffSeeded(shape, &randPtGroup(ptRange, ptCnt))
}

pub fn worleyNoise(shape : (usize, usize), ptRange : (f32, f32), ptCnt : usize, k : usize) -> Array2<f32> {
    worleyNoiseSeeded(shape, &randPtGroup(ptRange, ptCnt), k)
}
