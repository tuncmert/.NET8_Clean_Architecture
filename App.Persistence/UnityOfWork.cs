using App.Application.Contracts.Persistence;

namespace App.Persistence
{
    public class UnityOfWork(AppDbContext context) : IUnityOfWork
    {
        public Task<int> SaveChangeAsync()
        {
            return context.SaveChangesAsync();
        }
    }

}
