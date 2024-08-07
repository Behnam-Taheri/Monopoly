﻿using IdGen;
using Microsoft.AspNetCore.Mvc;
using Tactical.Framework.Application.CQRS.CommandHandling;
using Tactical.Monopoly.Application.Boards.CommandHandlers;
using Tactical.Monopoly.Application.Contract.Boards.Commands;
using Tactical.Monopoly.Domain.Boards.Contracts;
using Tactical.Monopoly.Domain.Boards.Events;
using Tactical.Monopoly.Queries.Contracts;

namespace Tactical.Monopoly.EndPoint.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly IBoardReadRepository _boardRepository;
        public BoardsController(ICommandBus commandBus, IBoardReadRepository boardRepository)
        {
            _commandBus = commandBus;
            _boardRepository = boardRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var board = await _boardRepository.GetAsync(id, cancellationToken);
            return Ok(board);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateBoardCommand command, CancellationToken cancellationToken)
        {
            Guid id = default;
            await _commandBus.Execute(command)
                .On<BoardCreatedEvent>(e => id = e.Id)
                .DispatchAsync(cancellationToken);
            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _commandBus.DispatchAsync(new DeleteBoardCommand(id), cancellationToken);
            return Ok();
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(MovePlayerCommand command, CancellationToken cancellationToken)
        {
            PlayerMovedToJailEvent @event = default;
            await _commandBus.Execute(command)
                .On<PlayerMovedToJailEvent>(e => @event = e)
                .DispatchAsync(cancellationToken);

            return Ok(@event);
        }

        [HttpPatch("stay-in-jail")]
        public async Task<IActionResult> StayInJail(StayInJailCommand command, CancellationToken cancellationToken)
        {
            await _commandBus.DispatchAsync(command, cancellationToken);
            return Ok();
        }
    }
}
