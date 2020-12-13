using System.Collections.Generic;
using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Commander.Controllers
{
    //controller name should always be plural
    [Microsoft.AspNetCore.Mvc.Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        #region Fields

        private readonly ICommanderRepo _Repository;
        private readonly ILogger<CommandsController> _Log;
        private readonly IMapper _Mapper;

        #endregion

        #region Constructor

        public CommandsController(ICommanderRepo repo, ILogger<CommandsController> logger, IMapper mapper)
        {
            _Repository = repo;
            _Log = logger;
            _Mapper = mapper;
        }

        #endregion

        #region Controller Methods

        //Get api/commands
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            _Log.LogInformation("This was called.");
            IEnumerable<Command> commandItems = _Repository.GetAllCommands();

            if (commandItems != null) 
                return Ok(_Mapper.Map<IEnumerable<CommandReadDto>>(commandItems));

            return NotFound("No able to find command items. Sorry :(");
        }

        //Get api/commands/5
        [HttpGet("{id}", Name="GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var command = _Repository.GetCommandById(id);

            if (command != null) 
                return Ok(_Mapper.Map<CommandReadDto>(command));

            return NotFound($"No command found with id {id}");
        }

        //Post api/commands
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _Mapper.Map<Command>(commandCreateDto);
            _Repository.CreateCommand(commandModel);
            _Repository.SaveChanges();

            var commandReadDto = _Mapper.Map<CommandReadDto>(commandModel);
            
            //This is really really cool
            return CreatedAtRoute(nameof(GetCommandById), new {id = commandReadDto.Id}, commandReadDto);
        }

        //Put api/commands/{id]
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _Repository.GetCommandById(id);

            if (commandModelFromRepo is null)
            {
                return NotFound($"No command found with id {id}");
            }

            _Mapper.Map(commandUpdateDto, commandModelFromRepo);

            _Repository.UpdateCommand(commandModelFromRepo);

            _Repository.SaveChanges();

            return NoContent();
        }

        //THIS IS HELLA COOL. Need to work with this more. 
        //Patch api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDocument)
        {
            var commandModelFromRepo = _Repository.GetCommandById(id);

            if (commandModelFromRepo is null)
            {
                return NotFound($"No command found with id {id}");
            }

            var commandToPatch = _Mapper.Map<CommandUpdateDto>(commandModelFromRepo);

            patchDocument.ApplyTo(commandToPatch, ModelState);

            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _Mapper.Map(commandToPatch, commandModelFromRepo);

            _Repository.UpdateCommand(commandModelFromRepo);

            _Repository.SaveChanges();

            return NoContent();
        }

        //Delete api/commands/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandModelFromRepo = _Repository.GetCommandById(id);

            if (commandModelFromRepo is null)
            {
                return NotFound($"No command found with id {id}");
            }

            _Repository.DeleteCommand(commandModelFromRepo);
            _Repository.SaveChanges();

            return NoContent();
        }

        #endregion
    }
}
