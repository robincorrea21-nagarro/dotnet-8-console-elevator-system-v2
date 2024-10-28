using dotnet_8_console_elevator_system_v2.Core.Enums;
using dotnet_8_console_elevator_system_v2.Entities;
using dotnet_8_console_elevator_system_v2.Services.Interfaces;

using Microsoft.Extensions.Configuration;

namespace dotnet_8_console_elevator_system_v2.Services
{
    public class ElevatorService: IElevatorService
    {
        private readonly int _floorMovementDelayMilliseconds;
        private readonly int _floorStopDelayMilliseconds;

        public ElevatorService(IConfiguration configuration)
        {
            // Read delay values from configuration
            _floorMovementDelayMilliseconds = configuration.GetValue<int>("ElevatorSettings:FloorMovementDelayMilliseconds", 1000);
            _floorStopDelayMilliseconds = configuration.GetValue<int>("ElevatorSettings:FloorStopDelayMilliseconds", 1000);
        }

        public async Task MoveAsync(Elevator elevator)
        {
            while (elevator.DestinationFloors.Count > 0)
            {
                elevator.State = ElevatorState.Moving;
                int nextFloor = elevator.DestinationFloors.First();
                elevator.DestinationFloors.RemoveAt(0);

                // Simulate elevator movement between floors
                while (elevator.CurrentFloor != nextFloor)
                {
                    if (elevator.CurrentFloor < nextFloor)
                    {
                        elevator.CurrentFloor++;
                        elevator.CurrentDirection = Direction.Up;
                    }
                    else if (elevator.CurrentFloor > nextFloor)
                    {
                        elevator.CurrentFloor--;
                        elevator.CurrentDirection = Direction.Down;
                    }

                    SetConsoleColor(elevator.Id);
                    Console.WriteLine($"Car {elevator.Id} is at floor {elevator.CurrentFloor}");
                    ResetConsoleColor();

                    // Add this condition in order to proceed with stop message.
                    if (elevator.CurrentFloor == nextFloor) {
                        continue;
                    }

                    await Task.Delay(_floorMovementDelayMilliseconds); // Simulate delay between floors
                }

                SetConsoleColor(elevator.Id);
                Console.WriteLine($"Car {elevator.Id} has arrived at floor {elevator.CurrentFloor} and is stopping.");
                ResetConsoleColor();
                elevator.State = ElevatorState.Stopped;
                await Task.Delay(_floorStopDelayMilliseconds); // Simulate passenger interaction

                // Elevator goes idle after reaching the destination
                elevator.CurrentDirection = Direction.Idle;
            }

            elevator.State = ElevatorState.Idle;
            elevator.IdleSince = DateTime.Now;

            SetConsoleColor(elevator.Id);
            Console.WriteLine($"Car {elevator.Id} is idle at floor {elevator.CurrentFloor}, waiting for the next request.");
            ResetConsoleColor();
        }

        public void AddDestination(Elevator elevator, int floor)
        {
            // Check if floor is already in the list or if it's the elevator's current floor
            if (!elevator.DestinationFloors.Contains(floor) && (elevator.CurrentFloor != floor || elevator.State == ElevatorState.Idle))
            {
                elevator.DestinationFloors.Add(floor);

                SetConsoleColor(elevator.Id);
                Console.WriteLine($"Assigned floor {floor} to Elevator {elevator.Id}.");
                ResetConsoleColor();
            }
        }

        private void SetConsoleColor(int elevatorId)
        {
            // Define an array of available console colors
            ConsoleColor[] colors = new ConsoleColor[]
            {
                ConsoleColor.Cyan,
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.Magenta,
                ConsoleColor.Blue,     // Added a new color for 5 or more elevators
                ConsoleColor.DarkRed,  // Additional colors can be added here
                ConsoleColor.DarkCyan
            };

            // Cycle through the available colors based on the elevatorId
            Console.ForegroundColor = colors[(elevatorId - 1) % colors.Length];
        }

        private void ResetConsoleColor()
        {
            Console.ResetColor();
        }
    }
}
