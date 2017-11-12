project(FreetypeTest)
cmake_minimum_required(VERSION 2.8)

find_package(Freetype REQUIRED)
include_directories(${FREETYPE_INCLUDE_DIRS})

aux_source_directory(. SRC_LIST)

SET(CMAKE_CXX_FLAGS "-O2 -std=c++11 -g")

SET(CMAKE_EXE_LINKER_FLAGS "-v -lfreetype")

add_executable(${PROJECT_NAME} ${SRC_LIST})

target_link_libraries(${PROJECT_NAME} freetype freeimage freeimageplus)
