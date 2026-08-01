# inport the compiler (.Net sdk from url as build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build 

# Create a working directory for the container

WORKDIR /src
# Copy the files from computer first dot to the container working directory(src) second dot
COPY . .

# Run to compile the code and store the Release mode version i.e no debugging tools in the app publish path of the container

RUN dotnet publish src/DevBoard.Api/DevBoard.Api.csproj -c Release -o /app/publish

# For the second stage of multi stage build
# get the asp.net runtime which is just a runtime environment that is lightweight, safe and fast to run the finished comipiled binaries

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
# set the working environment to app path
WORKDIR /app
# copy the finished binaries from the initial build file path and drops them into the working dir path /app (.)
COPY --from=build /app/publish .
# this exposes the port 8080 so docker knows that tcp traffic is going to come from here for the app in the container
EXPOSE 8080
# 
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "DevBoard.Api.dll"]