using BackEnd.Entities;

namespace BlogWebApp.ViewModels
{
    public class UserPostsViewModel
    {
        public string Username { get; set; }
        public List<UserPost> Posts { get; set; }

    }
}
