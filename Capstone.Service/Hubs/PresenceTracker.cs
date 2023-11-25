using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.Hubs
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers =
            new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string userid, string connectionId)
        {
            bool isOnline = false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(userid))
                {
                    OnlineUsers[userid].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(userid, new List<string> { connectionId });
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string userid, string connectionId)
        {
            bool isOffline = false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(userid)) return Task.FromResult(isOffline);

                OnlineUsers[userid].Remove(connectionId);
                if (OnlineUsers[userid].Count == 0)
                {
                    OnlineUsers.Remove(userid);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
        public Task<bool> IsOnlineUser(string userId)
        {
            lock (OnlineUsers)
            {
                return Task.FromResult(OnlineUsers.ContainsKey(userId));
            }
        } 
        public Task<List<string>> GetConnectionsForUser(string userId)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(userId   );
            }

            return Task.FromResult(connectionIds);
        }
    }
}
