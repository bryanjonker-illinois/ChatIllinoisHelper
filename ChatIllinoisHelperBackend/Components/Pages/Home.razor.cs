using ChatIllinoisHelper.Data.DataHelper;
using ChatIllinoisHelper.Data.DataModels;
using ChatIllinoisHelper.Data.Logic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ChatIllinoisHelperBackend.Components.Pages {
    public partial class Home {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected ChatItemHelper ChatItemHelper { get; set; } = default!;

        public Dictionary<int, string> ChatItems { get; set; } = default!;

        public ChatItem CurrentChatItem { get; set; } = default!;

        public List<string> Models { get; set; } = ModelList.Models.ToList();
        public string Results { get; set; } = "";

        public async Task Save() {
            var email = (await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User?.Identity?.Name ?? "";
            CurrentChatItem.CreatedByEmail = email;
            var results = await ChatItemHelper.Save(CurrentChatItem);
            Results = results.Item1;
            CurrentChatItem.Id = results.Item2;
            if (CurrentChatItem.Id != 0 && !ChatItems.ContainsKey(CurrentChatItem.Id)) {
                ChatItems.Add(CurrentChatItem.Id, CurrentChatItem.Name + " / " + CurrentChatItem.Code);
            }
        }

        public async Task Delete() {
            ChatItems.Remove(CurrentChatItem.Id);
            Results = await ChatItemHelper.Delete(CurrentChatItem.Id);
            CurrentChatItem = new ChatItem();
        }

        public async Task OnChatItemChanged(int? id) {
            if (id.HasValue) {
                CurrentChatItem = await ChatItemHelper.Get(id.Value);
            }
        }

        protected override async Task OnInitializedAsync() {
            var email = (await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User?.Identity?.Name ?? "";
            ChatItems = await ChatItemHelper.GetByEmail(email);
            CurrentChatItem = new ChatItem();
            await base.OnInitializedAsync();
        }

    }
}
