FROM public.ecr.aws/docker/library/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM public.ecr.aws/docker/library/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ProyectoAnalisis.dll"]