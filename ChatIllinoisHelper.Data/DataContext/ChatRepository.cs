using ChatIllinoisHelper.Data.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ChatIllinoisHelper.Data.DataContext {
    public class ChatRepository(IDbContextFactory<ChatContext> factory) {
        private readonly IDbContextFactory<ChatContext> _factory = factory;

        public int Create<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            _ = context.Add(item);
            return context.SaveChanges();
        }

        public async Task<int> CreateAsync<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            _ = context.Add(item);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync<T>(T item) {
            if (item == null) {
                return 0;
            }
            using var context = _factory.CreateDbContext();
            _ = context.Remove(item);
            return await context.SaveChangesAsync();
        }

        public T Read<T>(Func<ChatContext, T> work) {
            var context = _factory.CreateDbContext();
            return work(context);
        }

        public async Task<T> ReadAsync<T>(Func<ChatContext, T> work) {
            var context = _factory.CreateDbContext();
            return await Task.Run(() => work(context));
        }

        public async Task<int> UpdateAsync<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            _ = context.Update(item);
            return await context.SaveChangesAsync();
        }
    }
}
