
# https://docs.datadoghq.com/tracing/setup_overview/setup/dotnet-core/?tab=linux#configuration-settings



FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 4477
ENV ASPNETCORE_URLS=http://*:4477

#copy csproj and restore as distinct layers
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["tootest-dotnet.csproj", "./"]
RUN dotnet restore -r linux-x64

COPY . .
WORKDIR "/src/."
RUN dotnet build "tootest-dotnet.csproj" -c Release -o /app/build -r linux-x64

FROM build AS publish
RUN dotnet publish "tootest-dotnet.csproj" -c Release -o /app/publish -r linux-x64





##*************************************************
## Installer image
#FROM amd64/buildpack-deps:focal-curl as installer
##Download Tracer
#RUN  TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
#    && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb
#
#
#
#
###############################
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

## Copy datadog
##COPY --from=installer ["/opt/datadog", "/opt/datadog"]
##COPY --from=installer ["/var/log/datadog", "/var/log/datadog"]
#
#RUN mkdir -p /opt/datadog \
#    && mkdir -p /var/log/datadog
#
#COPY --from=installer ["/datadog-dotnet-apm_1.28.2_amd64.deb", "/opt/datadog"]
#
#RUN TRACER_VERSION="1.28.2" \
#    && cd ../ && cd /opt/datadog \
#    && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
#    && rm ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb
#
## Enable the tracer
#ENV CORECLR_ENABLE_PROFILING=1
#ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
#ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
#ENV DD_DOTNET_TRACER_HOME=/opt/datadog
#ENV DD_INTEGRATIONS=/opt/datadog/integrations.json

# Tracer configurations
ENV DD_ENV=occ-tootest
ENV DD_SERVICE=tootest-dotnet
ENV DD_VERSION=0.1
#ENV DD_AGENT_HOST=
ENV DD_LOGS_INJECTION=true
ENV DD_TRACE_DEBUG=false
#ENV DD_TRACE_LOG_PATH=
ENV DD_RUNTIME_METRICS_ENABLED=true
ENV DD_TRACE_ENABLED=true


ENTRYPOINT ["dotnet", "Xom.Bcs.tootest-dotnet.Api.dll"]
