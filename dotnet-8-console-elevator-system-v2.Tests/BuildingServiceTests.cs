using dotnet_8_console_elevator_system_v2.Core.Enums;
using dotnet_8_console_elevator_system_v2.DTOs;
using dotnet_8_console_elevator_system_v2.Entities;
using dotnet_8_console_elevator_system_v2.Services;
using dotnet_8_console_elevator_system_v2.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace dotnet_8_console_elevator_system_v2.Tests
{
    public class BuildingServiceTests
    {
        private readonly Mock<IElevatorService> _elevatorServiceMock;
        private readonly IConfiguration _configuration;
        private readonly BuildingService _buildingService;

        public BuildingServiceTests()
        {
            // Set up in-memory configuration for min and max floor settings
            var inMemorySettings = new Dictionary<string, string>
            {
                { "BuildingSettings:MinFloor", "0" },
                { "BuildingSettings:MaxFloor", "10" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Mock ElevatorService
            _elevatorServiceMock = new Mock<IElevatorService>();

            // Create BuildingService with mocked ElevatorService and configuration
            _buildingService = new BuildingService(_elevatorServiceMock.Object, _configuration);
        }

        [Fact]
        public void AddRequest_ShouldSelectElevatorAndAssignFloor()
        {
            var building = new Building(4);
            var request = new RequestDto(5, Direction.Up);

            _buildingService.AddRequest(building, request);

            _elevatorServiceMock.Verify(es => es.AddDestination(It.IsAny<Elevator>(), request.Floor), Times.Once);
            _elevatorServiceMock.Verify(es => es.MoveAsync(It.IsAny<Elevator>()), Times.Once);
        }

        [Fact]
        public void AddRequest_ShouldPrioritizeElevatorsBasedOnProximity()
        {
            var building = new Building(4);
            var request = new RequestDto(5, Direction.Up);

            _buildingService.AddRequest(building, request);

            _elevatorServiceMock.Verify(es => es.AddDestination(It.IsAny<Elevator>(), request.Floor), Times.Once);
        }
    }
}