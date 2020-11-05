FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY *.sln .
COPY SnomedTemplateService.Core/*.csproj SnomedTemplateService.Core/
COPY SnomedTemplateService.Data/*.csproj SnomedTemplateService.Data/
COPY SnomedTemplateService.Parser/*.csproj SnomedTemplateService.Parser/
COPY SnomedTemplateService.Util/*.csproj SnomedTemplateService.Util/
COPY SnomedTemplateService.Web/*.csproj SnomedTemplateService.Web/

RUN dotnet restore

COPY SnomedTemplateService.Core/ SnomedTemplateService.Core/
COPY SnomedTemplateService.Data/ SnomedTemplateService.Data/
COPY SnomedTemplateService.Parser/ SnomedTemplateService.Parser/
COPY SnomedTemplateService.Util/ SnomedTemplateService.Util/
COPY SnomedTemplateService.Web/ SnomedTemplateService.Web/

WORKDIR /app/SnomedTemplateService.Web
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY --from=build-env /app/SnomedTemplateService.Web/out .
ENTRYPOINT ["dotnet", "SnomedTemplateService.Web.dll"]

