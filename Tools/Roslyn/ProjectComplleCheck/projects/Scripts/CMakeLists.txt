# 工程名称，这里单独分开
project(Assembly-CSharp LANGUAGES CSharp)

file(GLOB_RECURSE SRC_FILES 
    ${UNITY_PROJECT_PATH}/Assets/YHLib/Scripts/*.cs
)

list(FILTER SRC_FILES EXCLUDE REGEX ".*/(Editor)/.*")

add_library(${PROJECT_NAME} SHARED ${SRC_FILES})

set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES
    ${COMMON_DOTNET_REFERENCES}
    ${CUSTOM_LIB_DLLS}
    ${ASSET_ROOT_DLLS}
    ${PLUGIN_DLLS}
)



assign_source_group(${SRC_FILES})

# Define dependencies.
# TARGET_LINK_LIBRARIES(${PROJECT_NAME} 
    # PUBLIC Assembly-CSharp-firstpass
# )
