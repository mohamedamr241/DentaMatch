﻿using DentaMatch.Models;
using DentaMatch.Repository.Case_Appointment.IRepository;
using DentaMatch.Repository.Notifications;
using Microsoft.AspNetCore.Identity;

namespace DentaMatch.Repository.Dental_Case.IRepository
{
    public interface IDentalUnitOfWork
    {
        IDentalCaseRepository DentalCaseRepository { get; }
        ICaseAppointmentRepository CaseAppointmentRepository { get; }
        IDentalCaseCommentRepository CaseCommentRepository { get; }
        UserManager<ApplicationUser> UserManager { get; }
        IDentalCaseProgressRepository CaseProgressRepository { get; }

        NotificationRepository notifications { get; }

        void Save();
    }
}
