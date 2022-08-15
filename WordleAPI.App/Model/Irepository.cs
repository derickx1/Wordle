namespace WordleAPI.App
{
    public interface IRepository
    {
        public User GetUser(string name);
        public void InsertUser(User user);
    }
}