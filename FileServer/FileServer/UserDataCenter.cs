using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class UserDataCenter
    {

        public static List<User> userList = new List<User>();

        static UserDataCenter()
        {
            userList.Add(new User()
            {
                id = 1,
                name = "abc",
                pwd = "abc"
            });
        }
    }
}
