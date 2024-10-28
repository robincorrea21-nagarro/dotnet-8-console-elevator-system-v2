using dotnet_8_console_elevator_system_v2.Core.Enums;

namespace dotnet_8_console_elevator_system_v2.DTOs
{
    public class RequestDto
    {
        public int Floor { get; set; }
        public Direction Direction { get; set; }

        public RequestDto(int floor, Direction direction)
        {
            Floor = floor;
            Direction = direction;
        }
    }
}
