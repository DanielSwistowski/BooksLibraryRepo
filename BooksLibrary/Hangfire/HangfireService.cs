using BooksLibrary.Business.Models;
using BooksLibrary.Business.SecurityServices;
using BooksLibrary.Business.Service;
using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksLibrary.Hangfire
{
    public class HangfireService : IHangfireService
    {
        private readonly IReservationService reservationService;
        private readonly IRentService rentService;
        private readonly IUserService userService;
        private readonly IReminderService reminderService;
        private readonly IEmailService emailService;
        public HangfireService(IReservationService reservationService, IRentService rentService, IUserService userService, IReminderService reminderService, IEmailService emailService)
        {
            this.reservationService = reservationService;
            this.rentService = rentService;
            this.userService = userService;
            this.reminderService = reminderService;
            this.emailService = emailService;
        }

        public void DeleteReservationAfterExpiredTime()
        {
            var reservationsToDelete = reservationService.GetAllReservations(r => r.DateOfReceipt < DateTime.Now).ToList();

            if (reservationsToDelete != null)
            {
                foreach (var reservation in reservationsToDelete)
                {
                    reservationService.DeleteReservation(reservation.ReservationId);
                }
            }
        }

        public void LockUserAccountIfTimeToReturnBookExpired()
        {
            var rentsList = rentService.GetAllRents(r => r.ReturnDate < DateTime.Now).ToList();

            if (rentsList != null)
            {
                string lockReason = "Upłynął termin zwrotu książki!";
                foreach (var rent in rentsList)
                {
                    UserModel user = userService.GetUserById(rent.UserId);
                    if (user.UserIsEnabled == true)
                    {
                        userService.DisableUser(user.UserName, lockReason, true);

                        Book book = rent.Book;
                        dynamic email = new Email("AccountWasDisabled");
                        email.To = user.Email;
                        email.Subject = "Twoje konto zostało zablokowane";
                        email.Book = rent.Book;
                        email.ReturnDate = rent.ReturnDate;
                        emailService.Send(email);
                    }
                }
            }
        }


        public void SendReminderAboutEndingTimeToReturnBook()
        {
            int day = DateTime.Now.Day - 1;
            int day2 = DateTime.Now.Day + 2;
            var rentsList = rentService.GetAllRents(r => r.ReturnDate.Day > day && r.ReturnDate.Day < day2).ToList();

            if (rentsList != null)
            {
                foreach (var rent in rentsList)
                {
                    if (reminderService.ReminderExists(rent.RentId) == false)
                    {
                        dynamic email = new Email("ReminderAboutEndingTimeToReturnBook");
                        email.To = rent.User.Email;
                        email.Subject = "Przypomnienie o upływającym terminie zwrotu książki";
                        email.Book = rent.Book;
                        email.ReturnDate = rent.ReturnDate;
                        emailService.Send(email);

                        Reminder reminder = new Reminder();
                        reminder.RentId = rent.RentId;
                        reminder.ReminderWasSent = true;
                        reminderService.AddReminder(reminder);
                    }
                }
            }
        }
    }
}