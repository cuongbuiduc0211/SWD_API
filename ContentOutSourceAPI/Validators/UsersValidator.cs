using ContentOutSourceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentOutSourceAPI.Validators
{
    public class UsersValidator
    {
        public Boolean ValidateUserFields(TblUsers tblUsers)
        {
            if (tblUsers.Username.Length < 0)
            {
                throw new ArgumentException("Username can't be blank");
            }
            if (tblUsers.Password.Length < 0)
            {
                throw new ArgumentException("Password can't be blank");
            }
            return true;
        }
    }
}
