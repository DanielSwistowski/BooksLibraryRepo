using BooksLibrary.Business.Models;
using BooksLibrary.Business.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Business.Service
{
    public class ReminderService : IReminderService
    {
        private readonly IUnitOfWork unitOfWork;
        public ReminderService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Reminder GetReminderByRentId(int rentId)
        {
            return unitOfWork.ReminderRepository.Get().Where(r => r.RentId == rentId).Single();
        }

        public void AddReminder(Reminder reminder)
        {
            unitOfWork.ReminderRepository.Add(reminder);
            unitOfWork.Commit();
        }

        public void DeleteReminder(int rentId)
        {

            Reminder reminder = unitOfWork.ReminderRepository.GetById(rentId);
            unitOfWork.ReminderRepository.Delete(reminder);
            unitOfWork.Commit();
        }

        public bool ReminderExists(int rentId)
        {
            return unitOfWork.ReminderRepository.Get().Where(r => r.RentId == rentId).Any();
        }
    }
}
