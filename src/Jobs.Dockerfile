FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source
COPY ./Domain ./Domain
COPY ./Application ./Application
COPY ./Infrastructure ./Infrastructure
COPY ./Jobs ./Jobs

WORKDIR /source/Jobs
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Jobs.dll"]