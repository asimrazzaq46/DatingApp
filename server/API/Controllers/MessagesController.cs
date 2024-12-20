using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController(IUnitOfWork _unitOfWork, IMapper _mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUserName();
        if (username == createMessageDto.RecipentUserName.ToLower()) return BadRequest("You cannot message yourself");
        var sender = await _unitOfWork.UserRepositery.GetUserByUserNameAsync(username);
        var recipient = await _unitOfWork.UserRepositery.GetUserByUserNameAsync(createMessageDto.RecipentUserName);

        if (recipient is null || sender is null || sender.UserName is null || recipient.UserName is null)
            return BadRequest("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipent = recipient,
            SenderUsername = sender.UserName,
            RecipentUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        _unitOfWork.MessageRepositery.AddMessage(message);

        if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));
        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>>
    GetMessagesForUSer([FromQuery] MessagesParams messagesParams)
    {
        messagesParams.Username = User.GetUserName();
        var messages = await _unitOfWork.MessageRepositery.GetMessagesForUser(messagesParams);
        Response.AddPaginationHeader(messages);
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUserName();
        return Ok(await _unitOfWork.MessageRepositery.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUserName();
        var message = await _unitOfWork.MessageRepositery.GetMessage(id);

        if (message == null) return BadRequest("Cannot delete this message");

        if (message.SenderUsername != username && message.RecipentUsername != username) return Forbid();

        if (message.SenderUsername == username) message.SenderDeleted = true;

        if (message.RecipentUsername == username) message.RecipentDeleted = true;

        if (message is { SenderDeleted: true, RecipentDeleted: true })
        {
            _unitOfWork.MessageRepositery.DeleteMessage(message);
        }

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the message");




    }


}
