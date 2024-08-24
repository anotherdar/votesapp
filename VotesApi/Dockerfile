# Use the official .NET Core SDK as a parent image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore any dependencies (use the correct .csproj)
COPY ["VotesApi.csproj", "./"]
RUN dotnet restore "VotesApi.csproj"

# Copy the rest of the application code
COPY . .

# Publish the application
RUN dotnet publish "VotesApi.csproj" -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port your application will run on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "VotesApi.dll"]
