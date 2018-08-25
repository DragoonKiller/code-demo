#pragma once

#include <cstdio>
#include <string>

namespace logConfiguration
{
    static std::string logFileName = "./log.txt";
}

static void Log(std::string const& s) { Log(s.c_str()); }

static FILE* GetLogFile()
{
    static FILE* f = fopen(logConfiguration::logFileName.c_str(), "w");
    return f;
}

template<typename ... Args>
static void Log(char const* s, Args ... args)
{
    fprintf(GetLogFile(), s, std::forward<Args>(args)...);
    fflush(GetLogFile());
}
