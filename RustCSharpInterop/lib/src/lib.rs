use std::mem::*;

#[allow(non_upper_case_globals)]
pub static mut _data : Option<Vec<i32>> = None;
pub unsafe fn get_data() -> &'static mut Vec<i32> {
    match _data {
        Some(ref mut d) => { d },
        _ => { panic!("static data not inited!"); }
    }
}

#[no_mangle]
pub unsafe extern fn init(sz : usize) {
    _data = Some(vec![0; sz]);
}

#[no_mangle]
pub unsafe extern fn add_up(sz : usize) {
    let data = get_data();
    for i in 1..sz { data[i] = data[i] + data[i-1]; }
}

#[no_mangle]
pub unsafe extern fn set(i : usize, value : i32) {
    let data = get_data();
    data[i] = value;
}

#[no_mangle]
pub unsafe extern fn index(i : usize) -> i32 {
    get_data()[i]
}

#[no_mangle]
pub unsafe extern fn usize_length() -> usize {
    size_of::<usize>()
}

#[no_mangle]
pub unsafe extern fn get_pointer() -> *mut i32 {
    get_data().as_mut_ptr()
}

#[no_mangle]
pub extern fn add(x : i32, y : i32) -> i32 {
    x + y
}

#[no_mangle]
pub extern fn mult(x : i32, y : i32) -> i64 {
    x as i64 * y as i64
}
