using BooksLibrary.Business.Models;
using BooksLibrary.Business.Repository;
using BooksLibrary.Business.Service;
using BooksLibrary.Business.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Business.Tests.Services
{
    [TestClass]
    public class ReminderServiceTest
    {
        private Mock<IUnitOfWork> mockUoW = null;
        private Mock<IRepository<Reminder>> mockReminderRepo = null;
        private List<Reminder> remindersList = null;

        public ReminderServiceTest()
        {
            mockUoW = new Mock<IUnitOfWork>();
            mockReminderRepo = new Mock<IRepository<Reminder>>();
            remindersList = new List<Reminder>(){
                new Reminder{ RentId = 1, ReminderWasSent=true },
                new Reminder{RentId = 2, ReminderWasSent=false},
                new Reminder{RentId = 3, ReminderWasSent=true}
            };
        }


        #region GetReminderByRentId

        [TestMethod]
        public void GetReminderByRentId_return_correct_reminder_object()
        {
            Reminder reminder = new Reminder() { RentId = 4, ReminderWasSent = true };
            remindersList.Add(reminder);

            mockReminderRepo.Setup(m => m.Get()).Returns(remindersList.AsQueryable());
            mockUoW.Setup(m => m.ReminderRepository).Returns(mockReminderRepo.Object);

            ReminderService service = new ReminderService(mockUoW.Object);
            var result = service.GetReminderByRentId(4);

            Assert.AreEqual(reminder, result);
        }

        #endregion


        #region AddReminder

        [TestMethod]
        public void AddReminder_can_add_new_reminder()
        {
            mockReminderRepo.Setup(m => m.Add(It.IsAny<Reminder>())).Callback((Reminder reminder) => remindersList.Add(reminder));
            mockUoW.Setup(m => m.ReminderRepository).Returns(mockReminderRepo.Object);

            ReminderService service = new ReminderService(mockUoW.Object);
            Reminder reminderToAdd = new Reminder();
            service.AddReminder(reminderToAdd);

            Assert.AreEqual(4, remindersList.Count);
            Assert.IsTrue(remindersList.Contains(reminderToAdd));
            mockReminderRepo.Verify(m => m.Add(reminderToAdd), Times.Once);
        }

        #endregion


        #region DeleteReminder

        [TestMethod]
        public void DeleteReminder_can_delete_reminder()
        {
            mockReminderRepo.Setup(m => m.Delete(It.IsAny<Reminder>())).Callback((Reminder reminder) =>
                {
                    var rem = remindersList.Where(r => r.RentId == 1).Single();
                    remindersList.Remove(rem);
                });

            mockUoW.Setup(m => m.ReminderRepository).Returns(mockReminderRepo.Object);

            ReminderService service = new ReminderService(mockUoW.Object);
            service.DeleteReminder(1);

            Assert.AreEqual(2, remindersList.Count);
            Assert.AreEqual(null, remindersList.Find(r => r.RentId == 1));
            mockReminderRepo.Verify(m => m.Delete(It.IsAny<Reminder>()), Times.Once);
        }

        #endregion


        #region ReminderExists

        [TestMethod]
        public void ReminderExists_return_true_if_reminder_exists()
        {
            mockReminderRepo.Setup(m => m.Get()).Returns(remindersList.AsQueryable());
            mockUoW.Setup(m => m.ReminderRepository).Returns(mockReminderRepo.Object);

            ReminderService service = new ReminderService(mockUoW.Object);
            var result = service.ReminderExists(1);

            Assert.IsTrue(result);
        }

        #endregion
    }
}
