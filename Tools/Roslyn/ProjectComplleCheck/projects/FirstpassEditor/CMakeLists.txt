# 工程名称，这里和unity里定义的一致
project(Assembly-CSharp-Editor-firstpass LANGUAGES CSharp)

file(GLOB_RECURSE SRC_FILES 
    ${UNITY_PROJECT_PATH}/Assets/Plugins/*.cs
)

list(FILTER SRC_FILES INCLUDE REGEX ".*/(Editor)/.*")

add_library(${PROJECT_NAME} SHARED ${SRC_FILES})

set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES
    ${COMMON_DOTNET_REFERENCES}
    ${CUSTOM_LIB_DLLS}
    ${ASSET_ROOT_DLLS}
    ${PLUGIN_DLLS}
)

SET(CMAKE_CSharp_FLAGS "/langversion:default /unsafe")

set_charp_defines(${WINDOWS_EDITOR_DEFINE})


assign_source_group(${SRC_FILES})

# Define dependencies.
TARGET_LINK_LIBRARIES(${PROJECT_NAME} 
    PUBLIC Assembly-CSharp-firstpass
)

