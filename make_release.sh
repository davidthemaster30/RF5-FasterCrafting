rm -rf obj
rm -rf bin

dotnet build FasterCrafting.csproj -f net6.0 -c Release

zip -j 'RF5FasterCrafting_v2.1.0.zip' './bin/Release/net6.0/FasterCrafting.dll' './bin/Release/net6.0/FasterCrafting.cfg'
