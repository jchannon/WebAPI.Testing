msbuild .\src\WebAPI.Testing\WebAPI.Testing.csproj /t:Build /p:Configuration="Release 4.5"
msbuild .\src\WebAPI.Testing\WebAPI.Testing.csproj /t:Build;Package /p:Configuration="Release 4.0"