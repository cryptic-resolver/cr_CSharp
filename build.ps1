# https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet-publish
#
# Publish option:
#     -c        --configuration <CONFIGURATION> (default Debug)
#     -r        --runtime <RUNTIME_IDENTIFIER>
#               --os
#     -a        --arch <ARCHITECTURE>
#     -f        --framework <FRAMEWORK> (already specified in cr.csproj)
#     -o        --output <OUTPUT_DIRECTORY>
#               --self-contained [true|false]
#
# Output dir: [project_file_folder]/bin/[configuration]/[framework]/[runtim]/publish/
#


# self-contained will compile everything including System.xx.dll
# and when use taht, you should add runtimid
# https://docs.microsoft.com/zh-cn/dotnet/core/rid-catalog
#
# That is because the Runtime is included. Add --self-contained false to publish without the .NET Core Runtime. 
# Of course, that means the Runtime needs to be installed on your target device
#
# dotnet publish .\cr.csproj -c Release --self-contained True --os linux
#

# https://www.mihajakovac.com/build-dotnet-single-exe/

dotnet publish -c Release --self-contained True --runtime win-x64 -p:PublishTrimmed=True -p:PublishSingleFile=True -p:IncludeNativeLibrariesForSelfExtract=True -p:DebugType=None -p:DebugSymbols=False
# 12MB
# I must say the binary with the huge size(already trimmed) is very slow compared with that in D or Go.
#

dotnet publish -c Release --self-contained True --runtime linux-x64 /property:PublishTrimmed=True /property:PublishSingleFile=True /property:IncludeNativeLibrariesForSelfExtract=True /property:DebugType=None /property:DebugSymbols=False
# 14MB

dotnet publish -c Release --self-contained True --runtime osx-x64 /property:PublishTrimmed=True /property:PublishSingleFile=True /property:IncludeNativeLibrariesForSelfExtract=True /property:DebugType=None /property:DebugSymbols=False
# 14MB


