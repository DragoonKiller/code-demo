build :
	nvcc ./src/World.cu \
		-std=c++11 \
		--compiler-options '-fPIC' \
		-shared \
		-O3 \
		-o ./bin/environment.so
	mv ./bin/environment.so ../environment.so
