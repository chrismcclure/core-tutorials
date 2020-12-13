using System.Collections.Generic;
using Commander.Models;

namespace Commander.Data
{
    public interface ICommanderRepo
    {
        /// <summary>
        /// Create a command in database
        /// </summary>
        /// <param name="command"></param>
        void CreateCommand(Command command);

        /// <summary>
        /// Saves the changes to the database
        /// </summary>
        /// <returns></returns>
        bool SaveChanges();

        /// <summary>
        /// Gets all the commands
        /// </summary>
        /// <returns></returns>
        IEnumerable<Command> GetAllCommands();

        /// <summary>
        /// Get the command by the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Command GetCommandById(int id);

        /// <summary>
        /// Update the command in the database
        /// </summary>
        /// <param name="command"></param>
        void UpdateCommand(Command command);

        void DeleteCommand(Command command);
    }
}