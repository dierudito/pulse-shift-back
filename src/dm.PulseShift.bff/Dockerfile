# --- Estágio 1: Build ---
# Usamos a imagem do SDK do .NET para compilar e publicar
# Ajuste a versão do .NET (ex: 7.0, 6.0) se necessário
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia os arquivos de projeto (.csproj) e solução (.sln) primeiro
# Isso aproveita o cache de layers do Docker. Se os projetos não mudarem, o 'restore' não roda novamente.
# Ajuste os caminhos conforme a estrutura do seu projeto.
COPY *.sln .
COPY src/PulseShift.Api/*.csproj ./src/PulseShift.Api/
# Adicione linhas COPY para OUTROS projetos referenciados pela API (Domain, Application, Infrastructure, etc.)
COPY src/PulseShift.Domain/*.csproj ./src/PulseShift.Domain/
COPY src/PulseShift.Application/*.csproj ./src/PulseShift.Application/
COPY src/PulseShift.Infrastructure/*.csproj ./src/PulseShift.Infrastructure/
# Restaura as dependências
RUN dotnet restore "PulseShift.Api.csproj" --use-current-runtime # Especifique o csproj da API ou o .sln

# Copia todo o resto do código fonte
COPY . .

# Publica a aplicação em modo Release para a pasta /app/publish
# Certifique-se de que o caminho para o csproj da API está correto
RUN dotnet publish "src/PulseShift.Api/PulseShift.Api.csproj" -c Release -o /app/publish --no-restore

# --- Estágio 2: Runtime ---
# Usamos a imagem do ASP.NET Core Runtime, que é menor que a do SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia os arquivos publicados do estágio de build para a imagem final
COPY --from=build /app/publish .

# --- Segurança: Executar como usuário não-root (Recomendado) ---
# As imagens recentes da Microsoft geralmente criam um usuário 'app' ou definem $APP_UID.
# Se a linha abaixo não funcionar ou você precisar de mais controle,
# pode ser necessário criar o usuário manualmente (veja comentários no final).
# Este usuário é criado nas imagens base mais recentes do .NET 8.
USER app
# Ou use um UID específico se definido na imagem base: USER $APP_UID

# --- Configuração de Rede e Entrypoint ---
# Expõe as portas que a aplicação ouvirá dentro do container
# Use as portas que sua aplicação Kestrel está configurada para ouvir (veja ASPNETCORE_URLS)
EXPOSE 8080
EXPOSE 8081 # Remova se não usar HTTPS

# Define as URLs que o Kestrel deve ouvir dentro do container.
# O '+' significa ouvir em qualquer endereço IP dentro do container nas portas especificadas.
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081

# Define o ponto de entrada para executar a DLL da sua API
# Substitua 'PulseShift.Api.dll' pelo nome real da DLL gerada pelo seu projeto Web API
ENTRYPOINT ["dotnet", "PulseShift.Api.dll"]