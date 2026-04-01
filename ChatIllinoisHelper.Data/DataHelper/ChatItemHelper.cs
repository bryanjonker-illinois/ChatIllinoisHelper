using ChatIllinoisHelper.Data.DataContext;
using ChatIllinoisHelper.Data.DataModels;

namespace ChatIllinoisHelper.Data.DataHelper {
    public class ChatItemHelper(ChatRepository chatRepository) {
        private readonly ChatRepository _chatRepository = chatRepository;

        public async Task<Dictionary<int, string>> GetByEmail(string email) {
            return await _chatRepository.ReadAsync(r => r.ChatItems.Where(c => c.CreatedByEmail == email).ToDictionary(c => c.Id, c => c.Name + " / " + c.Code));
        }

        public async Task<ChatItem> Get(int id) {
            return await _chatRepository.ReadAsync(r => r.ChatItems.FirstOrDefault(c => c.Id == id)) ?? new ChatItem();
        }

        public async Task<ChatItem> GetByCode(string code) {
            return await _chatRepository.ReadAsync(r => r.ChatItems.FirstOrDefault(c => c.Code == code)) ?? new ChatItem();
        }

        public async Task<string> Delete(int id) {
            var chatItem = await _chatRepository.ReadAsync(r => r.ChatItems.FirstOrDefault(c => c.Id == id));
            if (chatItem == null) {
                return "Chat item not found.";
            }
            await _chatRepository.DeleteAsync(chatItem);
            return $"Chat item {chatItem.Name} deleted successfully.";
        }

        public async Task<(string, int)> Save(ChatItem chatItem) {
            if (chatItem == null) {
                return ("Invalid chat item.", 0);
            }
            chatItem.Prepare();
            if (string.IsNullOrWhiteSpace(chatItem.Name) || string.IsNullOrWhiteSpace(chatItem.Code)) {
                return ("Chat item name and code cannot be empty.", 0);
            }
            if (string.IsNullOrWhiteSpace(chatItem.Model)) {
                return ("You must choose an AI model before continuing.", 0);
            }
            if (chatItem.Id == 0) {
                var existingItem = await _chatRepository.ReadAsync(r => r.ChatItems.FirstOrDefault(c => c.Name == chatItem.Name || c.Code == chatItem.Code));
                if (existingItem != null) {
                    return ($"Chat item {chatItem.Name} already exists.", 0);
                }
                await _chatRepository.CreateAsync(chatItem);
                return ($"Chat item {chatItem.Name} created successfully.", chatItem.Id);
            } else {
                await _chatRepository.UpdateAsync(chatItem);
                return ($"Chat item {chatItem.Name} updated successfully.", chatItem.Id);
            }
        }
    }
}
