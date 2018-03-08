using BooksLibrary.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public interface IReminderService
    {
        Reminder GetReminderByRentId(int rentId);
        void AddReminder(Reminder reminder);
        void DeleteReminder(int rentId);
        bool ReminderExists(int rentId);
    }
}
