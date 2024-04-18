# c# common
set(CSHARP_FLAGS_COMMON "/langversion:default /unsafe")
# nowarn
set(PROJECT_NOWARN "CS0414;NETSDK1049;CS0219;CS0169;CS0649;CS0436;CS0618;CS0162;CS0168;CS0078")

# common
set(UNITY_COMMON_DEFINES "UNITY_2019_4_33;UNITY_2019_4;UNITY_2019;UNITY_5_3_OR_NEWER;UNITY_5_4_OR_NEWER;UNITY_5_5_OR_NEWER;UNITY_5_6_OR_NEWER;UNITY_2017_1_OR_NEWER;UNITY_2017_2_OR_NEWER;UNITY_2017_3_OR_NEWER;UNITY_2017_4_OR_NEWER;UNITY_2018_1_OR_NEWER;UNITY_2018_2_OR_NEWER;UNITY_2018_3_OR_NEWER;UNITY_2018_4_OR_NEWER;UNITY_2019_1_OR_NEWER;UNITY_2019_2_OR_NEWER;UNITY_2019_3_OR_NEWER;UNITY_2019_4_OR_NEWER;UNITY_EDITOR_SINGLE_ABB_OPT;NET_4_6;UNITY_EDITOR_LOD;CREATE_GPU_PROGRAM_CALLBACK;UNITY_EDITOR_BUILDIL2CPPONLY;UNITY_EDITOR_BUNDLETHREADARCHIVE;UNITY_EDITOR_DEPENDENCYBATCH;UNITY_2017;ASTAR_NO_JSON;ASTAR_NO_ZIP;BEHAVIAC_RELEASE;BEHAVIAC_NOT_USE_UNITY")

# system editor
set(UNITY_WIN_EDITOR_DEFINES "UNITY_EDITOR;UNITY_EDITOR_64;UNITY_EDITOR_WIN")
set(UNITY_MAC_EDITOR_DEFINES "UNITY_EDITOR;UNITY_EDITOR_64;UNITY_EDITOR_OSX")

# platform
set(UNITY_WIN_STANDALONE_DEFINES "PLATFORM_STANDALONE;PLATFORM_STANDALONE_WIN;UNITY_STANDALONE_WIN;UNITY_STANDALONE")
set(UNITY_ANDROID_DEFINES "PLATFORM_ANDROID;UNITY_ANDROID;UNITY_ANDROID_API;UNITY_ANDROID_SUPPORTS_SHADOWFILES")
set(UNITY_IOS_DEFINES "PLATFORM_IOS;UNITY_IOS;PLATFORM_IPHONE;UNITY_IPHONE;UNITY_IPHONE_API")

# platform special
set(PROJECT_WIN_STANDALONE_EDITOR_DEFINES "D_USEPREFAB;UNITY_DEBUG;LOGIC_UNITY_EDITOR")



function(set_project_defines _platform _type _system)
    message("[ProjectDefines]set_project_defines sys:${_system} type:${_type} platform:${_platform}")
    # get config name
    string(TOUPPER CONFIGURATION_${_system}_${_platform}_${_type} _config_name)
    set(_config_name ${${_config_name}})
    message("[ProjectDefines]config name:${_config_name}")
    
    # system
    string(TOUPPER UNITY_${_system}_${_type}_DEFINES _editor_system_define_name)
    message("[ProjectDefines]_editor_system_define_name:${_editor_system_define_name}")
    set_csharp_config_defines(${_config_name} "${${_editor_system_define_name}}")

    # if(${_type} STREQUAL ${CHECK_TYPE_EDITOR})
        # string(TOUPPER UNITY_EDITOR_${_system}_DEFINES _editor_system_define_name)
        # message("[ProjectDefines] ${_config_name}->${${_editor_system_define_name}}")
        # set_csharp_config_defines(${_config_name} "${${_editor_system_define_name}}")
    # endif()
    
    # platform
    if("${_platform}" STREQUAL "${PLATFORM_NAME_STANDALONE}")
        string(TOUPPER UNITY_${_system}_${_platform}_DEFINES _platform_define_name)
    else()
        string(TOUPPER UNITY_${_platform}_DEFINES _platform_define_name)
    endif()
    message("[ProjectDefines] _platform_define_name:${_platform_define_name}")
    set_csharp_config_defines(${_config_name} "${${_platform_define_name}}")
    
    # platform special
    if("${_platform}" STREQUAL "${PLATFORM_NAME_STANDALONE}")
        string(TOUPPER PROJECT_${_system}_${_platform}_${_type}_DEFINES _platform_define_name)
    else()
        string(TOUPPER PROJECT_${_platform}_${_type}_DEFINES _platform_define_name)
    endif()
    message("[ProjectDefines]_platform_define_name:${_platform_define_name}")
    set_csharp_config_defines(${_config_name} "${${_platform_define_name}}")
endfunction()

function(set_project_defines_all)
    set_csharp_flags(${CSHARP_FLAGS_COMMON})
    set_csharp_nowarns(${PROJECT_NOWARN})
    
    # global
    set_csharp_defines(${UNITY_COMMON_DEFINES})

    # win editor
    set_project_defines(${PLATFORM_NAME_STANDALONE} ${CHECK_TYPE_EDITOR} ${SYSTEM_NAME_WIN})

    #win runtime
    set_project_defines(${PLATFORM_NAME_STANDALONE} ${CHECK_TYPE_RUNTIME} ${SYSTEM_NAME_WIN})

    #android editor
    set_project_defines(${PLATFORM_NAME_ANDROID} ${CHECK_TYPE_EDITOR} ${SYSTEM_NAME_WIN})

    #android runtime
    set_project_defines(${PLATFORM_NAME_ANDROID} ${CHECK_TYPE_RUNTIME} ${SYSTEM_NAME_WIN})

    #ios editor
    set_project_defines(${PLATFORM_NAME_IOS} ${CHECK_TYPE_EDITOR} ${SYSTEM_NAME_MAC})

    #ios runtime
    set_project_defines(${PLATFORM_NAME_IOS} ${CHECK_TYPE_RUNTIME} ${SYSTEM_NAME_MAC})
endfunction()

function(set_project_defines_all2)
    set_csharp_flags(${CSHARP_FLAGS_COMMON})
    set_csharp_nowarns(${PROJECT_NOWARN})

    # global
    set_csharp_defines(${UNITY_COMMON_DEFINES})

    # win editor
    set_csharp_config_defines(${CONFIGURATION_WIN_STANDALONE_EDITOR} ${UNITY_WIN_EDITOR_DEFINES})
    set_csharp_config_defines(${CONFIGURATION_WIN_STANDALONE_EDITOR} ${UNITY_WIN_STANDALONE_DEFINES})

    #win runtime
    set_csharp_config_defines(${CONFIGURATION_WIN_STANDALONE_RUNTIME} ${UNITY_WIN_STANDALONE_DEFINES})

    #android editor
    set_csharp_config_defines(${CONFIGURATION_WIN_ANDROID_EDITOR} ${UNITY_WIN_EDITOR_DEFINES})
    set_csharp_config_defines(${CONFIGURATION_WIN_ANDROID_EDITOR} ${UNITY_ANDROID_DEFINES})

    #android runtime
    set_csharp_config_defines(${CONFIGURATION_WIN_ANDROID_RUNTIME} ${UNITY_ANDROID_DEFINES})

    #ios editor
    set_csharp_config_defines(${CONFIGURATION_MAC_IOS_EDITOR} ${UNITY_MAC_EDITOR_DEFINES})
    set_csharp_config_defines(${CONFIGURATION_MAC_IOS_EDITOR} ${UNITY_IOS_DEFINES})

    #ios runtime
    set_csharp_config_defines(${CONFIGURATION_MAC_IOS_RUNTIME} ${UNITY_IOS_DEFINES})
endfunction()