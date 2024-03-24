#!/bin/bash
msbuild='/c/Program Files (x86)/Microsoft Visual Studio/2022/BuildTools/MsBuild/Current/Bin/MSBuild.exe'
"$msbuild" -p:Configuration=Release \
	&& cp -v bin/Release/DecimalPlaces.dll ./mod/dll/ \
	&& rm -rf ../saves/mods/DecimalPlaces \
	&& cp -vr ./mod ../saves/mods/DecimalPlaces

