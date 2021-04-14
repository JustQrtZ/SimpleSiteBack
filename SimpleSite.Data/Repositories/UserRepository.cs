using SimpleSite.Data.Abstract;
using SimpleSite.Model.Entities;

namespace SimpleSite.Data.Repositories
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository {
        public UserRepository (SimpleSiteContext context) : base (context) { }

        public bool IsEmailUniq (string email) {
            var user = this.GetSingle(u => u.Email == email);
            return user == null;
        }

        public bool IsUsernameUniq (string username) {
            var user = this.GetSingle(u => u.Username == username);
            return user == null;
        }
    }
}