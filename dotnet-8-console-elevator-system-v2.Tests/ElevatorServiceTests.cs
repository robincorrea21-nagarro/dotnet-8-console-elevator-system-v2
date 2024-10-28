
using dotnet_8_console_elevator_system_v2.Core.Enums;
using dotnet_8_console_elevator_system_v2.Entities;
using dotnet_8_console_elevator_system_v2.Services;
using Microsoft.Extensions.Configuration;

namespace dotnet_8_console_elevator_system_v2.Tests
{
    public class ElevatorServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly ElevatorService _elevatorService;

        public ElevatorServiceTests()
        {
            // Set up configuration with in-memory settings
            var inMemorySettings = new Dictionary<string, string>
            {
                { "ElevatorSettings:FloorMovementDelayMilliseconds", "100" },
                { "ElevatorSettings:FloorStopDelayMilliseconds", "100" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Initialize ElevatorService with the real configuration
            _elevatorService = new ElevatorService(_configuration);
        }

        [Fact]
        public async Task MoveAsync_ShouldReachDestination_AndResetState()
        {
            var elevator = new Elevator(1);
            elevator.DestinationFloors.Add(5);

            await _elevatorService.MoveAsync(elevator);

            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(ElevatorState.Idle, elevator.State);
            Assert.Empty(elevator.DestinationFloors);
        }

        [Fact]
        public void AddDestination_ShouldAddFloorRespectingOrder_AndAvoidDuplicates()
        {
            var elevator = new Elevator(1);

            _elevatorService.AddDestination(elevator, 3);
            _elevatorService.AddDestination(elevator, 6); // Intermediate destination
            _elevatorService.AddDestination(elevator, 10); // Final destination
            _elevatorService.AddDestination(elevator, 3); // Duplicate

            Assert.Equal(new List<int> { 3, 6, 10 }, elevator.DestinationFloors);
        }

        [Fact]
        public void AddDestination_ShouldPrioritizeBasedOnCurrentDirection()
        {
            var elevator = new Elevator(1) { CurrentFloor = 2, CurrentDirection = Direction.Up };

            _elevatorService.AddDestination(elevator, 4);
            _elevatorService.AddDestination(elevator, 6);
            _elevatorService.AddDestination(elevator, 3);

            Assert.Equal(new List<int> { 4, 6, 3 }, elevator.DestinationFloors);
        }

        [Fact]
        public async Task MoveAsync_ShouldBecomeIdleAfterFinalDestination()
        {
            var elevator = new Elevator(1);
            elevator.DestinationFloors.Add(3);

            await _elevatorService.MoveAsync(elevator);

            Assert.Equal(ElevatorState.Idle, elevator.State);
            Assert.Empty(elevator.DestinationFloors);
        }
    }
}
