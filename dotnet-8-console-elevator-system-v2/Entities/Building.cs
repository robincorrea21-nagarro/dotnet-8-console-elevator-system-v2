namespace dotnet_8_console_elevator_system_v2.Entities
{
    public class Building
    {
        public List<Elevator> Elevators { get; private set; }

        public Building(int numberOfElevators)
        {
            Elevators = new List<Elevator>();
            for (int i = 0; i < numberOfElevators; i++)
            {
                Elevators.Add(new Elevator(i + 1));
            }
        }
    }
}
