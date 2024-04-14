# ./CMakeLists.txt，工程总配置文件

# 最低版本要求，必须
cmake_minimum_required(VERSION 3.8 )

# set cmake search file
set (CMAKE_MODULE_PATH "${CMAKE_MODULE_PATH};${CMAKE_CURRENT_SOURCE_DIR}/cmakes")

set(CMAKE_SUPPRESS_REGENERATION true)

#set_property(GLOBAL PROPERTY USE_FOLDERS ON)

enable_language(CSharp)

# 项目名
project(ProjectComplleCheck LANGUAGES CSharp)

include(Common)

# plugin runtime
add_subdirectory(projects/Firstpass)

# plugin editor
add_subdirectory(projects/FirstpassEditor)

# mobascripts runtime
add_subdirectory(projects/MobaScripts)

# toolscripts runtime
add_subdirectory(projects/NormalScripts)

# toolscripts runtime
add_subdirectory(projects/Editor)