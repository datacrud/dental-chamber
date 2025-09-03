using System;
using System.Linq;
using System.Web.Http;
using DM.Models;
using DM.RequestModels;
using DM.Service.Contacts;

namespace DM.AuthServer.Controllers
{
    [Authorize]
    [RoutePrefix("api/Prescriptions")]
    public class PrescriptionController : ApiController
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPatientCreateService _patientCreateService;

        public PrescriptionController(IPrescriptionService prescriptionService, IPatientCreateService patientCreateService)
        {
            _prescriptionService = prescriptionService;
            _patientCreateService = patientCreateService;
        }

        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult Get()
        {
            return Ok(_prescriptionService.GetAll());
        }

        [HttpGet]
        [Route("GetById")]
        public IHttpActionResult Get(string request)
        {
            return Ok(_prescriptionService.GetById(Guid.Parse(request)));
        }

        [HttpGet]
        [Route("GetPatientCurrentPrescription")]
        public IHttpActionResult GetPatientCurrentPrescription(string request)
        {
            Prescription prescription = _prescriptionService.GetPatientCurrentPrescription(Guid.Parse(request)).LastOrDefault(x=> x.StatusId == 5);
            return Ok(prescription);
        }        

        [HttpPost]
        [Route("Create")]
        public IHttpActionResult Post(Prescription prescription)
        {
            Patient patient = _patientCreateService.GetById(prescription.PatientId);

            prescription.Code = HelperRequestModel.GenerateBillCode(patient.Code, (_prescriptionService.GetAll().Count(x => x.PatientId == prescription.PatientId) + 1).ToString());

            return Ok(_prescriptionService.Add(prescription));
        }

        [HttpPut]
        [Route("Update")]
        public IHttpActionResult Put(Prescription prescription)
        {
            return Ok(_prescriptionService.Edit(prescription));
        }


        [HttpDelete]
        [Route("Delete")]
        public IHttpActionResult Delete(string request)
        {
            return Ok(Guid.Parse(request));
        }


        [HttpGet]
        [Route("GetPatientHistory")]
        public IHttpActionResult GetPatientHistory(Guid patientId)
        {
            var patientHistories = _prescriptionService.GetPatientHistory(patientId);

            var histories = patientHistories
                .OrderByDescending(x=> x.Created)
                .Select(x => new
            {
                PrescriptionId = x.Id,
                x.PatientId,
                PatientName = x.Patient.Name,
                BillNo = x.Code,
                Created = x.Created.ToString("yyyy-MM-dd"),

                x.TotalCharge,
                x.DiscountPercent,
                x.DiscountAmount,
                x.FixedDiscount,
                x.TotalDiscountAmount,
                x.TotalPaid,
                x.TotalDue,

                StatusName = x.Status.Name,

                PatientMedicalServices = x.PatientMedicalServices.Select(y => new
                {
                    MedicalServiceName = y.MedicalService.Name,
                    y.Quantity
                }).ToList(),

                Payments = x.Payments.Select(y => new { Created = y.Created.ToString("yyyy-MM-dd"), y.Amount }).ToList(),

            }).ToList();

            return Ok(histories);
        }
    }
}
