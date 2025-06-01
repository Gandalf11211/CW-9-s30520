using CW_9_s30520.Data;
using CW_9_s30520.Exceptions;
using CW_9_s30520.Models;
using CW_9_s30520.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CW_9_s30520.Service;

public interface IDbService
{
    Task<PrescriptionResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto prescriptionData);
    Task<PrescriptionResponseDto> GetPrescriptionByIdAsync(int id);
    Task<PatientDetailsGetDto> GetPatientDetailsByIdAsync(int id);
}

public class DbService(AppDbContext data) : IDbService
{
    public async Task<PrescriptionResponseDto> CreatePrescriptionAsync(PrescriptionCreateDto prescriptionData)
    {
        var medicaments = prescriptionData.Medicaments.Select(m => m.IdMedicament).Distinct();

        var avaibleMedicaments =  prescriptionData.Medicaments
            .Where(m => medicaments.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToList();
        
        var notAvaibleMedicaments = medicaments.Except(avaibleMedicaments).ToList();

        if (notAvaibleMedicaments.Any())
        {
            throw new NotFoundException("These medicaments are not available");
        }

        if (prescriptionData.Medicaments == null || prescriptionData.Medicaments.Count == 0)
        {
            throw new EmptyPrescriptionException("There are no medicaments on the prescription!");
        }

        if (prescriptionData.Medicaments.Count > 10)
        {
            throw new OutOfBoundException("There are too many medicaments on the prescription. Prescription can have max 10 medicaments!");
        }

        if (prescriptionData.Date > prescriptionData.DueDate)
        {
            throw new OutOfBoundException("Due date must be greater than or equal to Date !");
        }
        
        var doctor = await data.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == prescriptionData.IdDoctor);

        if (doctor == null)
        {
            throw new NotFoundException("Doctor does not exist!");
        }

        using var transaction = await data.Database.BeginTransactionAsync();

        try
        {
            Patient? patient = null;

            if (prescriptionData.Patient.IdPatient != null)
            {
                patient = await data.Patients.FirstOrDefaultAsync(
                    p => p.IdPatient == prescriptionData.Patient.IdPatient);
            }

            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = prescriptionData.Patient.FirstName,
                    LastName = prescriptionData.Patient.LastName,
                    BirthDate = prescriptionData.Patient.BirthDate,
                };
                await data.Patients.AddAsync(patient);
                await data.SaveChangesAsync();
            }

            var prescription = new Prescription
            {
                IdPatient = patient.IdPatient,
                IdDoctor = doctor.IdDoctor,
                Date = prescriptionData.Date,
                DueDate = prescriptionData.DueDate,
            };
            await data.Prescriptions.AddAsync(prescription);
            await data.SaveChangesAsync();

            var prescriptionMedicaments = prescriptionData.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdPrescription = prescription.IdPrescription,
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Details = m.Description
            }).ToList();

            await data.PrescriptionMedicaments.AddRangeAsync(prescriptionMedicaments);
            await data.SaveChangesAsync();

            await transaction.CommitAsync();

            var result = await data.Prescriptions
                .Where(p => p.IdPrescription == prescription.IdPrescription)
                .Select(p => new PrescriptionResponseDto
                {
                    IdPrescription = p.IdPrescription,
                    Date = p.Date,
                    DueDate = p.DueDate,
                    Doctor = new DoctorGetDto()
                    {
                        IdDoctor = p.IdDoctor,
                        FirstName = p.Doctor.FirstName,
                        LastName = p.Doctor.LastName,
                        Email = p.Doctor.Email,
                    },
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentGetDto
                    {
                        MedicamentId = pm.IdMedicament,
                        Name = pm.Medicament.Name,
                        Description = pm.Medicament.Description,
                        Dose = pm.Dose,
                        Details = pm.Details
                    }).ToList()
                }).FirstAsync();

            return result;
        }
        catch(Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }




    public async Task<PrescriptionResponseDto> GetPrescriptionByIdAsync(int id)
    {
        var prescription = await data.Prescriptions.Where(p => p.IdPrescription == id)
            .Select(p => new PrescriptionResponseDto
            {
                IdPrescription = p.IdPrescription,
                Date = p.Date,
                DueDate = p.DueDate,
                Doctor = new DoctorGetDto
                {
                    IdDoctor = p.Doctor.IdDoctor,
                    FirstName = p.Doctor.FirstName,
                    LastName = p.Doctor.LastName,
                    Email = p.Doctor.Email,
                },
                Medicaments = p.PrescriptionMedicaments.Select(pm => new MedicamentGetDto
                {
                    MedicamentId = pm.IdMedicament,
                    Name = pm.Medicament.Name,
                    Description = pm.Medicament.Description,
                    Dose = pm.Dose,
                    Details = pm.Details
                }).ToList()
            }).FirstOrDefaultAsync();

        if (prescription == null)
        {
            throw new NotFoundException("Prescription not found");
        }

        return prescription;
    }
    
    
    public async Task<PatientDetailsGetDto> GetPatientDetailsByIdAsync(int id)
    {
        var patient = await data.Patients
            .Where(p => p.IdPatient == id)
            .Select(p => new PatientDetailsGetDto
            {
                IdPatient = p.IdPatient,
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthDate = p.BirthDate,
                Prescriptions = p.Prescriptions
                    .OrderBy(prs => prs.DueDate)
                    .Select(prs => new PrescriptionResponseDto
                    {
                        IdPrescription = prs.IdPrescription,
                        Date = prs.Date,
                        DueDate = prs.DueDate,
                        Doctor = new DoctorGetDto
                        {
                            IdDoctor = prs.Doctor.IdDoctor,
                            FirstName = prs.Doctor.FirstName,
                            LastName = prs.Doctor.LastName,
                            Email = prs.Doctor.Email,
                        },
                        Medicaments = prs.PrescriptionMedicaments.Select(pm => new MedicamentGetDto
                        {
                            MedicamentId = pm.IdMedicament,
                            Name = pm.Medicament.Name,
                            Description = pm.Medicament.Description,
                            Dose = pm.Dose,
                            Details = pm.Details
                        }).ToList()
                    }).ToList()
            }).FirstOrDefaultAsync();

        if (patient == null)
        {
            throw new NotFoundException("Patient not found");
        }
        
        return patient;
    }
}