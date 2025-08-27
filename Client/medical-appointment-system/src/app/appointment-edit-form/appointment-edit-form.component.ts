import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, formatDate } from '@angular/common';
import { AppointmentsService } from '../services/appointments.service';

@Component({
  selector: 'app-appointment-edit-form',
  standalone: true,
  imports:[CommonModule, ReactiveFormsModule],
  templateUrl: './appointment-edit-form.component.html'
})
export class AppointmentEditFormComponent implements OnChanges {
  @Input() appointment!: any; 

  editForm!: FormGroup;
  doctors: any[] = [];
  patients: any[] = [];
  medicines: any[] = [];

  constructor(
    private fb: FormBuilder,
    private service: AppointmentsService
  ) {}

  ngOnChanges(changes: SimpleChanges) {
    // If appointment input changes and has value
    if (changes['appointment'] && this.appointment) {
      this.loadDropdowns().then(() => this.buildForm());
    }
  }

  // Load dropdown data in parallel
  async loadDropdowns() {
    const [doctors, patients, medicines] = await Promise.all([
      this.service.getDoctors().toPromise(),
      this.service.getPatients().toPromise(),
      this.service.getMedicines().toPromise()
    ]);
  
    this.doctors = doctors ?? [];   // fallback to empty array
    this.patients = patients ?? [];
    this.medicines = medicines ?? [];
  }
  

  // Build form after appointment data is available
  buildForm() {
    this.editForm = this.fb.group({
      appointmentId: [this.appointment.appointmentId],
      doctorId: [this.appointment.doctorId, Validators.required],
      patientId: [this.appointment.patientId, Validators.required],
      visitType: [this.appointment.visitType],
      notes: [this.appointment.notes],
      diagnosis: [this.appointment.diagnosis],
      appointDate: [this.appointment.appointDate ? this.appointment.appointDate : new Date(), Validators.required],
      prescriptionDetails: this.fb.array([])
    });

    // Patch prescription details
    if (this.appointment.prescriptionDetails) {
      this.appointment.prescriptionDetails.forEach((pd: any) => {
        this.prescriptionDetails.push(this.fb.group({
          prescriptionDetailId: [pd.prescriptionDetailId],
          medicineId: [pd.medicineId, Validators.required],
          dosage: [pd.dosage],
          notes: [pd.notes],
          startDate: [pd.startDate ? formatDate(pd.startDate, 'yyyy-MM-dd', 'en') : null],
  endDate: [pd.endDate ? formatDate(pd.endDate, 'yyyy-MM-dd', 'en') : null]
        }));
      });
    }
  }

  get prescriptionDetails(): FormArray {
    return this.editForm.get('prescriptionDetails') as FormArray;
  }

  addPrescription(): void {
    this.prescriptionDetails.push(this.fb.group({
      prescriptionDetailId: [0],
      medicineId: [null],
      dosage: [''],
      notes: [''],
      startDate: [''],
      endDate: ['']
    }));
  }

  removePrescription(index: number): void {
    this.prescriptionDetails.removeAt(index);
  }

  onSubmit(): void {
    if (this.editForm.valid) {
      this.service.updateAppointment(this.editForm.value).subscribe({
        next: () => alert('Appointment updated!'),
        error: err => console.error(err)
      });
    }
  }
}
