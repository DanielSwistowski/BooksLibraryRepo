using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Repository
{
    public interface IMembershipRepository
    {
        string GetAccountConfirmationToken(string userEmail);
    }
}
