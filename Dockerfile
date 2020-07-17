FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dotnet-build
WORKDIR /app
COPY . .
RUN dotnet restore
WORKDIR /app/Zwyssigly.ImageServer.Standalone
RUN dotnet publish -c Release -o out --no-restore

FROM node:latest as node-build
WORKDIR /app
COPY ./imageserver-ui .
RUN npm i -g @quasar/cli
RUN npm install
RUN quasar build

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

COPY --from=node-build /app/dist/spa ./ui
COPY --from=dotnet-build /app/Zwyssigly.ImageServer.Standalone/out ./

ENTRYPOINT ["dotnet", "Zwyssigly.ImageServer.Standalone.dll"]

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

VOLUME /usr/share/imageserver/data

ENV Mongo__ConnectionString=
ENV Disk__Directory=/usr/share/imageserver/data
ENV Cors__AllowedOrigins__0=