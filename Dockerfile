
FROM alpine:3.14


FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 4477
ENV ASPNETCORE_URLS=http://*:4477

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["tootest-dotnet.csproj", "./"]
RUN dotnet restore "tootest-dotnet.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "tootest-dotnet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "tootest-dotnet.csproj" -c Release -o /app/publish



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# Please select the corresponding download for your Linux containers https://github.com/DataDog/dd-trace-dotnet/releases/latest

## Download and install the Tracer
#RUN mkdir -p /opt/datadog
#RUN curl -L https://github.com/DataDog/dd-trace-dotnet/releases/download/v1.28.0/datadog-dotnet-apm-1.28.0.tar.gz \
#|  ar xzf - -C /opt/datadog
#
## Enable the tracer
#ENV CORECLR_ENABLE_PROFILING=1
#ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
#ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
#ENV DD_DOTNET_TRACER_HOME=/opt/datadog
#ENV DD_INTEGRATIONS=/opt/datadog/integrations.json

ENTRYPOINT ["dotnet", "tootest-dotnet.dll"]
