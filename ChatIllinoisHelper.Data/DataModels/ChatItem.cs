using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatIllinoisHelper.Data.DataModels {
    public class ChatItem : BaseDataItem {
        public string ApiSecret { get; set; } = "";
        public DateTime? ApiSecretLastChanged { get; set; }
        public string Code { get; set; } = "";

        public string CreatedByEmail { get; set; } = "";

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        public bool RemoveThinkText { get; set; }

        public string Model { get; set; } = "";
        public string Name { get; set; } = "";
        public string SystemPrompt { get; set; } = "";

        public string SystemPromptMassaged => SystemPrompt.Replace('\r', ' ').Replace('\n', ' ');

        public void Prepare() {
            LastUpdated = DateTime.UtcNow;
            ApiSecret = ApiSecret.Trim();
            Code = Code.ToLowerInvariant().Replace(" ", "");
            Name = Name.Trim();
        }
    }
}
