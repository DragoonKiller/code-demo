x : build
	if EXIST output rmdir /s /q output
	mkdir output
	cargo test

run : src/*
	cargo run

build : src/*
	cargo build
