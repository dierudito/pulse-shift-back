# Usar apenas a imagem de runtime, pois não haverá build dentro do contêiner
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copiar os arquivos já publicados da sua máquina local
COPY ./publish .

# Definir o usuário não-root
USER app

# Ponto de entrada
ENTRYPOINT ["dotnet", "dm.PulseShift.bff.dll"]