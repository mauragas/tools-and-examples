# Run below commands to deploy/run program in docker:
# docker build -t phonestatistics .
# docker run -it --rm -p 5000:80 --name phonestatistics_demo phonestatistics

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o out 

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "PhoneStatistics.dll"]