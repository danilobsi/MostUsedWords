FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY Api/bin/Release/net5.0 App/
WORKDIR /App
ENTRYPOINT ["dotnet", "MyMostUsedWords.dll"]