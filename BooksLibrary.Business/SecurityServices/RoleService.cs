using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace BooksLibrary.Business.SecurityServices
{
    public class RoleService : IRoleService
    {
        public string[] GetAllRoles()
        {
            return Roles.GetAllRoles();
        }

        public void AddRoleToUser(string userName, string roleName)
        {
            Roles.AddUserToRole(userName, roleName);
        }

        public void RemoveRoleFromUser(string userName, string roleName)
        {
            Roles.RemoveUserFromRole(userName, roleName);
        }

        public void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }

        public void DeleteRole(string roleName)
        {
            Roles.DeleteRole(roleName, true);
        }

        public bool CheckIfRoleExists(string roleName)
        {
            return Roles.RoleExists(roleName);
        }

        public string[] GetRolesForUser(string userName)
        {
            return Roles.GetRolesForUser(userName);
        }

        public bool UserAccountContainSelectedRole(string roleName, string userName)
        {
            if (Roles.GetRolesForUser(userName).Contains(roleName))
                return true;
            else
                return false;
        }
    }
}
