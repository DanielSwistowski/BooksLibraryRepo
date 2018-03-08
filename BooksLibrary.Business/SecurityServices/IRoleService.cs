using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.SecurityServices
{
    public interface IRoleService
    {
        string[] GetAllRoles();
        void AddRoleToUser(string userName, string roleName);
        void RemoveRoleFromUser(string userName, string roleName);
        void CreateRole(string roleName);
        void DeleteRole(string roleName);
        bool CheckIfRoleExists(string roleName);
        string[] GetRolesForUser(string userName);
        bool UserAccountContainSelectedRole(string roleName, string userName);
    }
}
