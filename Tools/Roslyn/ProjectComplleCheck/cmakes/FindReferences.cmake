set(LIBRARY_OUTPUT_PATH ${PROJECT_SOURCE_DIR}/bin)
set(UNITY_PROJECT_PATH ${PROJECT_SOURCE_DIR}/../..)
set(CUSTOM_LIBS_PATH ${PROJECT_SOURCE_DIR}/libs)

message("UNITY_PROJECT:${UNITY_PROJECT_PATH}")

file(GLOB_RECURSE CUSTOM_LIB_DLLS
    ${CUSTOM_LIBS_PATH}/*.dll
)

set(CUSTOM_LIB_DLLS_RUNTIME ${CUSTOM_LIB_DLLS})
list(FILTER CUSTOM_LIB_DLLS_RUNTIME EXCLUDE REGEX ".*Editor.*")
# message(STATUS ${CUSTOM_LIB_DLLS})
# message(STATUS ${CUSTOM_LIB_DLLS_RUNTIME})

file(GLOB_RECURSE CUSTOM_LIB_DLLS_EDITOR
    ${CUSTOM_LIBS_PATH}/*.Editor.dll
)
# message(STATUS ${CUSTOM_LIB_DLLS_EDITOR})

file(GLOB ASSET_ROOT_DLLS
    ${UNITY_PROJECT_PATH}/Assets/*.dll
)
#message(STATUS ${ASSET_ROOT_DLLS})

file(GLOB_RECURSE PLUGIN_DLLS
    ${UNITY_PROJECT_PATH}/Assets/Plugins/*.dll
)
#list(FILTER PLUGIN_DLLS EXCLUDE REGEX ".*/Editor/.*")
#message(STATUS ${PLUGIN_DLLS})

file(GLOB_RECURSE EDITOR_DLLS
    ${UNITY_PROJECT_PATH}/Assets/Editor/*.dll
    ${UNITY_PROJECT_PATH}/Assets/Plugins/*.dll
)
list(FILTER EDITOR_DLLS INCLUDE REGEX ".*/Editor/.*")

#message(STATUS ${PLUGIN_DLLS})

set(COMMON_DOTNET_REFERENCES "Microsoft.CSharp"
    "PresentationCore"
    "PresentationFramework"
    "System"
    "System.Core"
    "System.Data"
    "System.Data.DataSetExtensions"
    "System.Net"
    "System.Net.Http"
    "System.Xaml"
    "System.Xml"
    "System.Xml.Linq"
    "System.Runtime.Serialization"
)
