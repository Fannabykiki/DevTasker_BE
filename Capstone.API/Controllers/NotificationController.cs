﻿using Capstone.API.Extentions;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.Notification;
using Capstone.DataAccess.Entities;
using Capstone.Service.NotificationService;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.API.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController: ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ClaimsIdentity _identity;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("latest")]
        public async Task<ActionResult<List<NotificationViewModel>>> GetLatest()
        {
            var userId = this.GetCurrentLoginUserId();
            var result = await _notificationService.GetLatestNotifications(userId);
            if (result == null)
            {
                return BadRequest("Project not have any task!");
            }
            return Ok(result);
        }
        [HttpGet("allNotification")]
        public async Task<ActionResult<List<NotificationViewModel>>> GetAllNotificationLatest()
        {
            var userId = this.GetCurrentLoginUserId();
            var result = await _notificationService.GetAllNotificationsByUser(userId);
            if (result == null)
            {
                return BadRequest("Project not have any task!");
            }
            return Ok(result);
        }
        [HttpPost("read")]
        public async Task<ActionResult<bool>> ReadNotification(ReadNotificationRequest request)
        {
            var userId = this.GetCurrentLoginUserId();
            var result = await _notificationService.MarkReadNotification(userId,request);
            if (result == null)
            {
                return BadRequest("Project not have any task!");
            }
            return Ok(result);
        }

    }
}
