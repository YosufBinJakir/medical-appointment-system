import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppointmentFormDto } from '../appoint-form-dto';
import { CommonModule } from '@angular/common';
import { PrescriptionsService } from '../services/prescriptions.service';
import { AppointmentsService } from '../services/appointments.service';


@Component({
  selector: 'app-appointment-form',
  standalone: true,
  imports:[
    CommonModule, 
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl:'./appointment-form.component.html'
})
export class AppointmentModalComponent implements OnInit {
  doctors: any[] = [];
  patients: any[] = [];
  medicines: any[] = [];
  appointmentForm!: FormGroup;

  constructor(private fb: FormBuilder,private appointmentService: AppointmentsService, private prescriptionService: PrescriptionsService) {}
  getDoctorList() {
    this.prescriptionService.getDoctors().subscribe({
      next: (res: any) => {
        this.doctors = res;
        console.log(this.doctors);
      },
      error: (err) => console.error(err)
    });
  }

  getPatientList() {
    this.prescriptionService.getPatients().subscribe({
      next: (res: any) => {
        this.patients = res;
      },
      error: (err) => console.error(err)
    });
  }

  getMedicineList() {
    this.prescriptionService.getMedicines().subscribe({
      next: (res: any) => {
        this.medicines = res;
      },
      error: (err) => console.error(err)
    });
  }

  ngOnInit(): void {
    this.appointmentForm = this.fb.group({
      patientId: [null, Validators.required],
      doctorId: [null, Validators.required],
      appointmentDate: [null, Validators.required],
      visitType: ['new', Validators.required],
      notes: [''],
      diagnosis: [''],
      prescriptionDetailFormDtos: this.fb.array([]) 
    });
    this.getPatientList();
    this.getMedicineList();
    this.getDoctorList();
    
  }

  
  get prescriptions(): FormArray {
    return this.appointmentForm.get('prescriptionDetailFormDtos') as FormArray;
  }

  // Add row
  addPrescription() {
    const prescriptionGroup = this.fb.group({
      medicineId: [null, Validators.required],
      dosage: ['', Validators.required],
      startDate: [null],
      endDate: [null],
      notes: ['']
    });
    this.prescriptions.push(prescriptionGroup);
  }

  // Remove row
  removePrescription(index: number) {
    this.prescriptions.removeAt(index);
  }

  // Submit
  submit() {
    if (this.appointmentForm.valid) {
      const dto: AppointmentFormDto = this.appointmentForm.value;
      console.log('Submitting Apppointment', dto);

      // Call API here
       this.appointmentService.saveAppointment(dto).subscribe({
        next: (res) => {
          console.log('Appointment saved successfully:', res);
          alert('Appointment saved successfully!');
          this.appointmentForm.reset(); // reset after success
        },
        error: (err) => {
          console.error('Error saving appointment:', err);
          alert('Failed to save appointment.');
        }
      });
    }
  }
}
