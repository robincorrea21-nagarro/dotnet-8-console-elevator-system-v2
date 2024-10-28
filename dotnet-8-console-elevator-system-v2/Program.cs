using dotnet_8_console_elevator_system_v2.Entities;
using dotnet_8_console_elevator_system_v2.Services;
using dotnet_8_console_elevator_system_v2.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())  // This requires the FileExtensions package
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();

// Register configuration as a singleton
serviceCollection.AddSingleton<IConfiguration>(config);

// Register services
serviceCollection.AddTransient<IElevatorService, ElevatorService>();
serviceCollection.AddScoped<IBuildingService, BuildingService>();

// Register Building as transient because it's a stateful object created with parameters
serviceCollection.AddTransient(provider =>
{
    // Fetch the number of elevators from appsettings.json
    int numberOfElevators = config.GetValue<int>("BuildingSettings:NumberOfElevators");
    return new Building(numberOfElevators);
});

// Build the service provider
var serviceProvider = serviceCollection.BuildServiceProvider();

// Resolve dependencies
var buildingService = serviceProvider.GetRequiredService<IBuildingService>();

// Create a building with 4 elevators
var building = serviceProvider.GetRequiredService<Building>();

// Start the elevator request system
await buildingService.StartElevatorSystemAsync(building);
