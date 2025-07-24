FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app 
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplication1.dll"]


#Dockerfile’ım 3 katmandan oluşuyor: önce SDK ile build, sonra publish, en sonunda sadece çalıştırma ortamı. Bu yapı hem daha küçük imaj oluşturur hem production-ready olur. Uygulamam 8080 portundan başlıyor ve Docker üzerinden izole şekilde çalıştırılabiliyor.