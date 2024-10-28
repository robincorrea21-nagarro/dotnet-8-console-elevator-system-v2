using dotnet_8_console_elevator_system_v2.Core.Enums;

namespace dotnet_8_console_elevator_system_v2.Entities
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; }
        public Direction CurrentDirection { get; set; }
        public ElevatorState State { get; set; }
        public DateTime IdleSince { get; set; }
        public List<int> DestinationFloors { get; set; }

        public Elevator(int id)
        {
            Id = id;
            CurrentFloor = 0;
            CurrentDirection = Direction.Idle;
            State = ElevatorState.Idle;
            IdleSince = DateTime.Now;
            DestinationFloors = new List<int>();
        }
    }
}
