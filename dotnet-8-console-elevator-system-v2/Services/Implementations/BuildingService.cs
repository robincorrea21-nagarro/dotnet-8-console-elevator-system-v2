using dotnet_8_console_elevator_system_v2.Core.Enums;
using dotnet_8_console_elevator_system_v2.DTOs;
using dotnet_8_console_elevator_system_v2.Entities;
using dotnet_8_console_elevator_system_v2.Services.Interfaces;

using Microsoft.Extensions.Configuration;

namespace dotnet_8_console_elevator_system_v2.Services
{
    public class BuildingService : IBuildingService
    {
        private readonly IElevatorService _elevatorService;
        private readonly int _minFloor;
        private readonly int _maxFloor;
        private readonly int _requestGenerationIntervalMilliseconds;

        public BuildingService(IElevatorService elevatorService, IConfiguration configuration)
        {
            _elevatorService = elevatorService;
            _minFloor = configuration.GetValue<int>("BuildingSettings:MinFloor");
            _maxFloor = configuration.GetValue<int>("BuildingSettings:MaxFloor");
            _requestGenerationIntervalMilliseconds = configuration.GetValue<int>("BuildingSettings:RequestGenerationIntervalMilliseconds");
        }

        public async Task StartElevatorSystemAsync(Building building)
        {
            while (true)
            {
                GenerateNewRequest(building);
                await Task.Delay(_requestGenerationIntervalMilliseconds); // Simulate system checking delay
            }
        }

        private void GenerateNewRequest(Building building)
        {
            var random = new Random();
            int floor = random.Next(_minFloor, _maxFloor + 1);

            // Determine the direction based on the selected floor:
            // - If on the ground floor, direction is Up
            // - If on the top floor, direction is Down
            // - Otherwise, randomly select Up or Down

            Direction direction;
            if (floor == _minFloor)
            {
                direction = Direction.Up;
            }
            else if (floor == _maxFloor)
            {
                direction = Direction.Down;
            }
            else
            {
                direction = random.Next(0, 2) == 0 ? Direction.Up : Direction.Down;
            }

            // Create a new request object with the selected floor and direction
            RequestDto request = new RequestDto(floor, direction);

            // Add the request to the building to process
            AddRequest(building, request);
        }

        public void AddRequest(Building building, RequestDto request)
        {
            Console.WriteLine($"Someone pressed the \"{request.Direction.ToString().ToLower()}\" button on floor {request.Floor}.");

            Elevator selectedElevator = GetNearestAvailableElevator(building.Elevators, request);
            if (selectedElevator != null)
            {
                _elevatorService.AddDestination(selectedElevator, request.Floor);

                // Only start MoveAsync if the elevator is idle or has completed all destinations
                if (selectedElevator.State == ElevatorState.Idle || selectedElevator.DestinationFloors.Count == 0)
                {
                    Task.Run(() => _elevatorService.MoveAsync(selectedElevator)); // Run MoveAsync in parallel
                }
            }
            else
            {
                Console.WriteLine("No available elevator at the moment.");
            }
        }

        private Elevator GetNearestAvailableElevator(List<Elevator> elevators, RequestDto request)
        {
            Elevator bestElevator = null;
            double bestScore = double.MinValue; // The higher the score, the better the elevator

            foreach (var elevator in elevators)
            {
                // Calculate idle time in seconds
                TimeSpan idleTime = elevator.State == ElevatorState.Idle ? DateTime.Now - elevator.IdleSince : TimeSpan.Zero;

                // Calculate distance to the requested floor
                int distance = Math.Abs(elevator.CurrentFloor - request.Floor);

                // Normalize idle time
                double idleTimeScore = idleTime.TotalSeconds / 10.0;

                // Normalize distance
                double maxDistance = 10.0;
                double distanceScore = maxDistance - distance;

                // Combine idle time and distance into a final score, giving each a weight
                double idleWeight = 0.7;
                double distanceWeight = 0.3;

                double finalScore = (idleTimeScore * idleWeight) + (distanceScore * distanceWeight);

                // Choose the elevator with the best score
                if (finalScore > bestScore)
                {
                    bestScore = finalScore;
                    bestElevator = elevator;
                }
            }

            return bestElevator;
        }
    }
}
