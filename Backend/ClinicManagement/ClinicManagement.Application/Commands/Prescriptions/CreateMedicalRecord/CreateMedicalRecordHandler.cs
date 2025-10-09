using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Prescriptions.CreateMedicalRecord
{
    public class CreateMedicalRecordHandler:IRequestHandler<CreateMedicalRecordCommand, int>
    {
        private readonly IMedicalRecordService medicalRecordService;
        public CreateMedicalRecordHandler(IMedicalRecordService _medicalRecordService)
        {
            medicalRecordService = _medicalRecordService;
        }
        public async Task<int> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return await medicalRecordService.CreateMedicalRecord(request.PatientId, request.Date, request.Notes, request.Diagnosis);
        }
    }
}
