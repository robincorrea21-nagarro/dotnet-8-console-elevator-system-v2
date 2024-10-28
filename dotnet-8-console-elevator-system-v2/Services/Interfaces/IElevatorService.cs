using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnet_8_console_elevator_system_v2.Core.Enums;
using dotnet_8_console_elevator_system_v2.Entities;

namespace dotnet_8_console_elevator_system_v2.Services.Interfaces
{
    public interface IElevatorService
    {
        Task MoveAsync(Elevator elevator);
        void AddDestination(Elevator elevator, int floor);
    }
}
