using AutoMapper;
using Capstone.Common.DTOs.Notification;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }
        public async Task<List<NotificationViewModel>> GetLatestNotifications(Guid userId, int page = 10)
        {
            var results = await _notificationRepository.GetQuery().Where(x => x.RecerverId == userId).OrderByDescending(y => y.CreateAt).Take(page).ToListAsync();
            return _mapper.Map<List<NotificationViewModel>>(results);
        }
    }
}
