using TechXpress.Data.Entities;

namespace TechXpress.Web.Models
{
    public class UserListViewModel
    {
        public  List<User> Users { get; set; }
        public  Dictionary<string, IList<string>> UserRoles { get; set; }
    }
}
