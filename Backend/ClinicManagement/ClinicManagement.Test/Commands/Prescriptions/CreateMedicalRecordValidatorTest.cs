using ClinicManagement.Application.Commands.Prescriptions.CreateMedicalRecord;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Test.Commands.Prescriptions
{
    public class CreateMedicalRecordValidatorTest
    {
        private readonly CreateMedicalRecordValidator _validator;

        public CreateMedicalRecordValidatorTest()
        {
            _validator = new CreateMedicalRecordValidator();
        }

        [Fact]
        public void Validate_ValidCommand_ReturnsSuccess()
        {
            var command = new CreateMedicalRecordCommand
            {
                PrescriptionId = 1,
                Diagnosis = "Common cold",
                Notes = "Patient should rest and stay hydrated."
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_MissingPrescriptionId_ReturnsError()
        {
            var command = new CreateMedicalRecordCommand
            {
                Diagnosis = "Flu",
                Notes = "Take prescribed medication."
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.PrescriptionId);
        }

        [Fact]
        public void Validate_EmptyDiagnosis_ReturnsError()
        {
            var command = new CreateMedicalRecordCommand
            {
                PrescriptionId = 1,
                Diagnosis = "",
                Notes = "Sample notes"
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Diagnosis);
        }

        [Fact]
        public void Validate_TooLongDiagnosis_ReturnsError()
        {
            var command = new CreateMedicalRecordCommand
            {
                PrescriptionId = 1,
                Diagnosis = new string('D', 201),
                Notes = "Sample notes"
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Diagnosis);
        }

        [Fact]
        public void Validate_TooLongNotes_ReturnsError()
        {
            var command = new CreateMedicalRecordCommand
            {
                PrescriptionId = 1,
                Diagnosis = "Flu",
                Notes = new string('N', 501)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Notes);
        }
    }
}