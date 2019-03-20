extern crate num_traits;

pub struct Iter2D<T> where T : num_traits::NumOps {
    i : T,
    j : T,
    t : T,
    r : T,
}

impl<T> Iterator for Iter2D<T> where
    T : num_traits::PrimInt
{
    type Item = (T, T);
    fn next(&mut self) -> Option<(T, T)> {
        let one = T::from(1).unwrap();
        let zero = T::from(0).unwrap();
        self.j = self.j + one;
        if self.j == self.r { self.i = self.i + one; self.j = zero; }
        if self.i == self.t { None }
        else { Some(( self.i, self.j )) }
    }
}

// iteration [0 .. t][0 .. r]
pub fn iter2D<T>(t : T, r : T) -> Iter2D<T> where T : num_traits::PrimInt {
    Iter2D::<T> { i : T::from(0).unwrap(), j : T::from(0).unwrap(), t : t, r : r, }
}

// iteration [b .. t][l .. r]
pub fn iter2R<T>(b : T, t : T, l : T, r : T) -> Iter2D<T> where T : num_traits::PrimInt {
    Iter2D::<T> { i : b, j : l, t : t - T::from(1).unwrap(), r : r - T::from(1).unwrap(), }
}

// iteration [b ..= t][l ..= r]
pub fn iter2I<T>(b : T, t : T, l : T, r : T) -> Iter2D<T> where T : num_traits::PrimInt {
    Iter2D::<T> { i : b, j : l, t : t, r : r, }
}

pub fn vec4<T>(v : T) -> [T; 4] where T : Clone {
    [v.clone(), v.clone(), v.clone(), v.clone()]
}

macro_rules! iter2D
{
    (t:expr, r:expr) => {{ iter2D(0, t, 0, r) }};
    (b:expr, t:expr, l:expr, r:expr) => {{ iter2D(b, t, l, r) }}
}

macro_rules! assignAll
{
    ($v:expr => $($s:expr),+) =>
    {{
        $(
            $s = $v;
        )+
    }}
}
