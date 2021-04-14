using SimpleSite.Model.Entities;

namespace SimpleSite.Data.Abstract
{
    public interface IUserRepository : IEntityBaseRepository<User>
    {
        bool IsUsernameUniq(string username);
        bool IsEmailUniq(string email);
    }
}