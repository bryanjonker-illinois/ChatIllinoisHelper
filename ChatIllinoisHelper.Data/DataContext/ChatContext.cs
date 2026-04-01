using ChatIllinoisHelper.Data.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ChatIllinoisHelper.Data.DataContext {
    public class ChatContext : DbContext {
        private readonly Guid _id;

        public ChatContext() : base() {
            _id = Guid.NewGuid();
            Debug.WriteLine($"{_id} context created.");
        }

        public ChatContext(DbContextOptions<ChatContext> options) : base(options) {
            _id = Guid.NewGuid();
            Debug.WriteLine($"{_id} context created.");
        }

        public DbSet<ChatItem> ChatItems { get; set; }

        public override void Dispose() {
            Debug.WriteLine($"{_id} context disposed.");
            base.Dispose();
        }

        public override ValueTask DisposeAsync() {
            Debug.WriteLine($"{_id} context disposed async.");
            return base.DisposeAsync();
        }
    }
}
