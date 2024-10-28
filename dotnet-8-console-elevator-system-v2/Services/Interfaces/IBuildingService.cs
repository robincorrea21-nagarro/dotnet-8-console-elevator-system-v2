using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnet_8_console_elevator_system_v2.DTOs;
using dotnet_8_console_elevator_system_v2.Entities;

namespace dotnet_8_console_elevator_system_v2.Services.Interfaces
{
    public interface IBuildingService
    {
        Task StartElevatorSystemAsync(Building building);
        void AddRequest(Building building, RequestDto request);
    }
}
