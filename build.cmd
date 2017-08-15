rem SET Build_Number=42
nuget restore
msbuild build.proj /t:Build;Publish;Test /p:Configuration=Debug
rem msbuild build.proj /t:Build;Publish;Test /p:Configuration=Release