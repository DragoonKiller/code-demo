#![allow(non_upper_case_globals)]

pub const eps : f32 = 1e-6;
pub const pi : f32 = 3.141592653589793;

#[derive(Copy, Clone, PartialEq, Debug)]
pub struct Pt { pub x : f32, pub y : f32 }

impl Pt
{
    pub fn x() -> Pt { Pt { x : 1.0, y : 0.0 } }
    pub fn y() -> Pt { Pt { x : 0.0, y : 1.0 } }
    pub fn one() -> Pt { Pt { x : 1.0, y : 1.0 } }
    pub fn zero() -> Pt { Pt::new(0.0, 0.0) }
    fn is_zero(&self) -> bool { self.x.abs() < eps && self.y.abs() < eps }
    
    pub fn new (x : f32, y : f32) -> Pt {
        Pt { x : x, y : y }
    }
    
    pub fn len(&self) -> f32 {
        self.len2().sqrt()
    }
    
    pub fn len2(&self) -> f32 {
        self.x * self.x + self.y * self.y
    }
    
    pub fn a(&self) -> f32 {
        self.y.atan2(self.x)
    }
    
    pub fn norm(&self) -> Pt {
        if (self.len()).abs() < eps {
            Pt{ x : 0.0, y : 0.0 }
        } else {
            Pt { x : self.x / self.len(), y : self.y / self.len() }
        }
    }
    
    pub fn rot(&self, a : f32) -> Pt {
        Pt {
            x : self.x * a.cos() - self.y * a.sin(),
            y : self.x * a.sin() + self.y * a.cos() 
        }
    }
    
    pub fn pairwise(&self, f : &Fn(f32)->f32) -> Pt {
        Pt {
            x : f(self.x),
            y : f(self.y),
        }
    }
}

impl num_traits::Zero for Pt
{
    fn zero() -> Pt { Pt::new(0.0, 0.0) }
    fn is_zero(&self) -> bool { self.x.abs() < eps && self.y.abs() < eps }
}

impl std::ops::Shl for Pt
{
    type Output = Pt;
    fn shl(self, b : Pt) -> Pt {
        self - b
    }
}

impl std::ops::Shr for Pt
{
    type Output = Pt;
    fn shr(self, b : Pt) -> Pt {
        b - self
    }
}

impl std::ops::Add for Pt
{
    type Output = Pt;
    fn add(self, b : Pt) -> Pt {
        Pt::new(self.x + b.x, self.y + b.y)
    }
}

impl std::ops::Sub for Pt
{
    type Output = Pt;
    fn sub(self, b : Pt) -> Pt {
        Pt::new(self.x - b.x, self.y - b.y)
    }
}

impl std::ops::Mul for Pt {
    type Output = Pt;
    fn mul(self, b : Pt) -> Pt {
        Pt::new(self.x * b.x, self.y * b.y)
    }
}

impl std::ops::Div for Pt {
    type Output = Pt;
    fn div(self, b : Pt) -> Pt {
        Pt::new(self.x / b.x, self.y / b.y)
    }
}

impl std::ops::BitAnd for Pt
{
    type Output = f32;
    fn bitand(self, b : Pt) -> f32 {
        self.x * b.x + self.y * b.y
    }
}

impl std::ops::BitXor for Pt
{
    type Output = f32;
    fn bitxor(self, b : Pt) -> f32 {
        self.x * b.y - self.y * b.x
    }
}

impl std::ops::AddAssign for Pt {
    fn add_assign(&mut self, v : Pt) { self.x += v.x; self.y += v.y; }
}
impl std::ops::SubAssign for Pt {
    fn sub_assign(&mut self, v : Pt) { self.x -= v.x; self.y -= v.y; }
}
impl std::ops::MulAssign for Pt {
    fn mul_assign(&mut self, v : Pt) { self.x *= v.x; self.y *= v.y; }
}
impl std::ops::DivAssign for Pt {
    fn div_assign(&mut self, v : Pt) { self.x /= v.x; self.y /= v.y; }
}
