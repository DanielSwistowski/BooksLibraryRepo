using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Repository
{
    public class MembershipRepository : IMembershipRepository
    {
        private BookLibraryDataContext context;
        public MembershipRepository(BookLibraryDataContext context)
        {
            this.context = context;
        }

        public string GetAccountConfirmationToken(string userEmail)
        {
            string sqlCmd = "select ConfirmationToken from webpages_Membership join [UserModel] on [UserModel].UserId=webpages_Membership.UserId where Email=" + "'" + userEmail + "'";
            string token = context.Database.SqlQuery<string>(sqlCmd).Single();
            return token;
        }
    }
}
