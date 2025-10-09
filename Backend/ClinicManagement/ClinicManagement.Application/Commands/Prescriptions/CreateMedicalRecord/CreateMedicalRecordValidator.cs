using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Prescriptions.CreateMedicalRecord
{
    public class CreateMedicalRecordValidator:AbstractValidator<CreateMedicalRecordCommand>
    {
        public CreateMedicalRecordValidator() {
            RuleFor(x => x.PatientId).NotEmpty().WithMessage("Patient ID is required.");
            RuleFor(x => x.Date).NotEmpty().WithMessage("Date is required.");
            RuleFor(x => x.Diagnosis).NotEmpty().WithMessage("Diagnosis is required.")
                                     .MaximumLength(200).WithMessage("Diagnosis cannot exceed 200 characters.");
            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
