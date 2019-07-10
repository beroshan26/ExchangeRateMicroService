FROM microsoft/dotnet:latest

WORKDIR /app

COPY *.csproj .

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o out

#WORKDIR /app / ExchangeRateWebApi / out

#ENTRYPOINT ["dotnet", "ExchangeRateWebApi.dll"]

EXPOSE 5000/tcp

ENTRYPOINT [ "dotnet", "watch", "run", "--no-restore", "--urls", "https://0.0.0.0:5000"]cd\