all:
	msbuild /p:Configuration=Release /t:Restore ../Goui.Wasm.Build.Tasks/Goui.Wasm.Build.Tasks.csproj
	msbuild /p:Configuration=Release ../Goui.Wasm.Build.Tasks/Goui.Wasm.Build.Tasks.csproj
	nuget pack
