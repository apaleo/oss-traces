ARG PROJECT_NAME=Traces.Web
ARG ROOT_FOLDER=src/$PROJECT_NAME

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app
ARG ROOT_FOLDER

# copy, restore and build everything and run tests
COPY . .
RUN dotnet restore
RUN dotnet build Traces.sln -c Release --no-restore
RUN for D in test/*; do [ -d "${D}" ] && echo "$D/$(basename $D).csproj"; done

#publish app
WORKDIR /app/$ROOT_FOLDER
RUN dotnet publish -c Release -o out --no-restore

#create image
FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS runtime
WORKDIR /app
ARG PROJECT_NAME
ARG ROOT_FOLDER
ENV APP=${PROJECT_NAME}.dll

COPY --from=build /app/$ROOT_FOLDER/out ./

CMD ASPNETCORE_URLS=http://+:$PORT dotnet $APP
