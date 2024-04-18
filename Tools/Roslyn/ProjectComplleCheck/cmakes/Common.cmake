set(CSHARP_FRAMEWORK "v4.7.1")

# system name
set(SYSTEM_NAME_WIN "WIN")
set(SYSTEM_NAME_MAC "MAC")

# platform name
set(PLATFORM_NAME_STANDALONE "Standalone")
set(PLATFORM_NAME_ANDROID "Android")
set(PLATFORM_NAME_IOS "iOS")

# check type
set(CHECK_TYPE_RUNTIME "Runtime")
set(CHECK_TYPE_EDITOR "Editor")

# flag
function(set_csharp_flags)
    foreach(_define IN ITEMS ${ARGN})
        set(_CSharp_FLAGS "${_CSharp_FLAGS} ${_define}")
    endforeach()
    # message("_CSharp_FLAGS:${_CSharp_FLAGS}")
    set(CMAKE_CSharp_FLAGS "${CMAKE_CSharp_FLAGS} ${_CSharp_FLAGS}" CACHE STRING "" FORCE)
    message("CMAKE_CSharp_FLAGS:${CMAKE_CSharp_FLAGS}")
endfunction()

function(set_csharp_config_flags _config)
    foreach(_define IN ITEMS ${ARGN})
        set(_CSharp_FLAGS "${_CSharp_FLAGS} ${_define}")
    endforeach()
    # message("[Common]"_CSharp_FLAGS:${_CSharp_FLAGS})
    string(TOUPPER ${_config} _upper_config)
    # message("[Common]CMAKE_CSharp_FLAGS_${_upper_config}:${CMAKE_CSharp_FLAGS_${_upper_config}}")
    set(CMAKE_CSharp_FLAGS_${_upper_config} "${CMAKE_CSharp_FLAGS_${_upper_config}} ${_CSharp_FLAGS}" CACHE STRING "" FORCE)
    message("[Common]CMAKE_CSharp_FLAGS_${_upper_config}:${CMAKE_CSharp_FLAGS_${_upper_config}}")
endfunction()

# defines
function(set_csharp_defines)
    foreach(_define IN ITEMS ${ARGN})
        set(_CSharp_FLAGS "${_CSharp_FLAGS} /define:${_define}")
    endforeach()
    # message("_CSharp_FLAGS:${_CSharp_FLAGS}")
    set(CMAKE_CSharp_FLAGS "${CMAKE_CSharp_FLAGS} ${_CSharp_FLAGS}" CACHE STRING "" FORCE)
    message("CMAKE_CSharp_FLAGS:${CMAKE_CSharp_FLAGS}")
endfunction()

function(set_csharp_config_defines _config)
    foreach(_define IN ITEMS ${ARGN})
        set(_CSharp_FLAGS "${_CSharp_FLAGS} /define:${_define}")
    endforeach()
    # message("[Common]"_CSharp_FLAGS:${_CSharp_FLAGS})
    string(TOUPPER ${_config} _upper_config)
    # message("[Common]CMAKE_CSharp_FLAGS_${_upper_config}:${CMAKE_CSharp_FLAGS_${_upper_config}}")
    set(CMAKE_CSharp_FLAGS_${_upper_config} "${CMAKE_CSharp_FLAGS_${_upper_config}} ${_CSharp_FLAGS}" CACHE STRING "" FORCE)
    message("[Common]CMAKE_CSharp_FLAGS_${_upper_config}:${CMAKE_CSharp_FLAGS_${_upper_config}}")
endfunction()

# nowarn
function(set_csharp_nowarns)
    set(CMAKE_CSharp_FLAGS "${CMAKE_CSharp_FLAGS} /nowarn:${ARGN}" CACHE STRING "" FORCE)
    message("CMAKE_CSharp_FLAGS:${CMAKE_CSharp_FLAGS}")
endfunction()

function(set_csharp_config_nowarns _config)
    string(TOUPPER ${_config} _upper_config)
    # message("[Common]CMAKE_CSharp_FLAGS_${_upper_config}:${CMAKE_CSharp_FLAGS_${_upper_config}}")
    set(CMAKE_CSharp_FLAGS_${_upper_config} "${CMAKE_CSharp_FLAGS_${_upper_config}} /nowarn:${ARGN}" CACHE STRING "" FORCE)
    message("[Common]CMAKE_CSharp_FLAGS_${_upper_config}:${CMAKE_CSharp_FLAGS_${_upper_config}}")
endfunction()

function(assign_source_group)
    foreach(_source IN ITEMS ${ARGN})
        if (IS_ABSOLUTE "${_source}")
            file(RELATIVE_PATH _source_rel "${UNITY_PROJECT_PATH}" "${_source}")
        else()
            set(_source_rel "${_source}")
        endif()
        get_filename_component(_source_path "${_source_rel}" PATH)
        string(REPLACE "/" "\\" _source_path_msvc "${_source_path}")
        source_group("${_source_path_msvc}" FILES "${_source}")
    endforeach()
endfunction()