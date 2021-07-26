SET CORECLR_ENABLE_PROFILING=1
SET CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}



SET DD_ENV=tootest
SET DD_SERVICE=tootest-dotnet
SET DD_VERSION=0.1
SET DD_LOGS_INJECTION=true
SET DD_TAGS=xom_org:FLCIT,xom_env:tootest,xom_app_support_group:tocomplete,xom:app_name:china_oneconnect,xom_app_id:APP-12070,xom_hosting_env:alibaba,xom_hosting_type:paas

rem Launch application
bin\Release\net5.0\tootest-dotnet.exe