rem SET Build_Number=42
nuget restore
rem msbuild build.proj /t:Build;Publish /p:Configuration=Debug
msbuild build.proj /t:Build;Publish;Test /p:Configuration=Release