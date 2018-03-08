using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public interface IManagementService : IDisposable
    {
        bool ReleaseBook(int reservationId, int rentTime);
        void ReturnBook(int rentId);
        bool ShouldEnableUserAccount(int userId);
    }
}
