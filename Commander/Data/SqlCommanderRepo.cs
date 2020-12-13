using System;
using System.Collections.Generic;
using System.Linq;
using Commander.Models;

namespace Commander.Data
{
    public class SqlCommanderRepo : ICommanderRepo
    {
        #region Fields

        private readonly CommanderContext _CommanderContext;

        #endregion

        #region Constructor

        public SqlCommanderRepo(CommanderContext context)
        {
            _CommanderContext = context;
        }

        #endregion

        #region ICommanderRepo

        public void CreateCommand(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _CommanderContext.Commands.Add(command);
            SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_CommanderContext.SaveChanges() >= 0);
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return _CommanderContext.Commands.ToList();
        }

        public Command GetCommandById(int id)
        {
            return _CommanderContext.Commands.FirstOrDefault(x => x.Id == id);
        }

        public void UpdateCommand(Command command)
        {
          //Do nothing
        }

        public void DeleteCommand(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _CommanderContext.Commands.Remove(command);
        }

        #endregion
    }
}
