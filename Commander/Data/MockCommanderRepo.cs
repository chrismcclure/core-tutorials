using Commander.Models;
using System.Collections.Generic;

namespace Commander.Data
{
    public class MockCommanderRepo : ICommanderRepo
    {
        #region ICommanderRepo

        public void CreateCommand(Command command)
        {
            throw new System.NotImplementedException();
        }

        public bool SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return new List<Command>()
            {
                new Command
                {
                    Id = 0,
                    HowTo = "Boil an egg",
                    Line = "Boil Water",
                    Platform = "Kettle and Pan"
                },
                new Command
                {
                    Id = 0,
                    HowTo = "Btest",
                    Line = "Bor",
                    Platform = "Ketand Pan"
                },
                new Command
                {
                    Id = 0,
                    HowTo = "testegg",
                    Line = "Boil  dssdfWater",
                    Platform = "Kettle  sdfdsand Pan"
                }
            };
        }

        public Command GetCommandById(int id)
        {
            return new Command
            {
                Id = 0,
                HowTo = "Boil an egg",
                Line = "Boil Water",
                Platform = "Kettle and Pan"
            };
        }

        public void UpdateCommand(Command command)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCommand(Command command)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
