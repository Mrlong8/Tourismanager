
using System.Collections.Generic;
using System.Threading.Tasks;
using TourisManager.Core.Entities;

namespace TourisManager.Core.Interfaces

{
    public interface IChatService
    {
        Task<List<RoomChat>> GetRoomsAsync(string accountId, string role);

        Task<RoomChat?> GetRoomAsync(string roomChatId);

        Task<RoomChat> GetOrCreateRoomAsync(
            string locationId,
            string accountId);

        Task SendMessageAsync(
            string roomChatId,
            string accountId,
            string messageText,
            bool shouldAutoReply);
    }
}
