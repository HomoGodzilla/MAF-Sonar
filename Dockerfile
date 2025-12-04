FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os .csproj files, mantendo a estrutura de pastas
COPY MAF/Core/Domain/MAF.Core.Domain.csproj Core/Domain/
COPY MAF/Core/Application/MAF.Core.Application.csproj Core/Application/
COPY MAF/Ports/Input/MAF.Ports.Input.csproj Ports/Input/
COPY MAF/Ports/Output/MAF.Ports.Output.csproj Ports/Output/
COPY MAF/Adapters/Input/WebServer/MAF.Adapters.Input.WebServer.csproj Adapters/Input/WebServer/
COPY MAF/Adapters/Output/Firebase/MAF.Adapters.Output.Firebase.csproj Adapters/Output/Firebase/


RUN dotnet restore "Core/Domain/MAF.Core.Domain.csproj"
RUN dotnet restore "Core/Application/MAF.Core.Application.csproj"
RUN dotnet restore "Ports/Input/MAF.Ports.Input.csproj"
RUN dotnet restore "Ports/Output/MAF.Ports.Output.csproj"
RUN dotnet restore "Adapters/Output/Firebase/MAF.Adapters.Output.Firebase.csproj"
RUN dotnet restore "Adapters/Input/WebServer/MAF.Adapters.Input.WebServer.csproj"

COPY MAF/ .

# Define o workdir para o publish
WORKDIR "/src/Adapters/Input/WebServer"
# Publica o projeto principal (WebServer), que tem referência aos outros
RUN dotnet publish "MAF.Adapters.Input.WebServer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MAF.Adapters.Input.WebServer.dll"]