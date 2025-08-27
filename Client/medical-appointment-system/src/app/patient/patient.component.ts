// patient.component.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PatientService } from '../patient.service';
import { Patient } from '../patient';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-patient',
  templateUrl: './patient.component.html',
  imports :[
    FormsModule, CommonModule, ReactiveFormsModule   
  ],
  styleUrls: ['./patient.component.css']
})
export class PatientComponent implements OnInit {
  patients: Patient[] = [];
  patientForm: FormGroup;
  isEditing = false;
  editingPatientId: number | null = null;
  loading = false;
  error = '';
  success = '';

  constructor(
    private fb: FormBuilder,
    private patientService: PatientService
  ) {
    this.patientForm = this.fb.group({
      patientName: ['', [Validators.required, Validators.minLength(2)]],
      
      email: ['', [Validators.required, Validators.email]],
     
    });
  }

  ngOnInit(): void {
    this.loadPatients();
  }

  // Load all patients
  loadPatients(): void {
    this.loading = true;
    this.patientService.getPatients().subscribe({
      next: (data) => {
        this.patients = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Failed to load patients');
        this.loading = false;
      }
    });
  }

  // Submit form (create or update)
  onSubmit(): void {
    if (this.patientForm.valid) {
      const patient: Patient = this.patientForm.value;
      
      if (this.isEditing && this.editingPatientId) {
        this.updatePatient(this.editingPatientId, patient);
      } else {
        this.createPatient(patient);
      }
    }
  }

  // Create new patient
  createPatient(patient: Patient): void {
    this.loading = true;
    this.patientService.createPatient(patient).subscribe({
      next: (response) => {
        this.showSuccess('Patient created successfully');
        this.resetForm();
        this.loadPatients();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Failed to create patient');
        this.loading = false;
      }
    });
  }

  // Update existing patient
  updatePatient(id: number, patient: Patient): void {
    this.loading = true;
    this.patientService.updatePatient(id, patient).subscribe({
      next: (response) => {
        this.showSuccess('Patient updated successfully');
        this.resetForm();
        this.loadPatients();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Failed to update patient');
        this.loading = false;
      }
    });
  }

  // Edit patient
  editPatient(patient: Patient): void {
    this.isEditing = true;
    this.editingPatientId = patient.patientId || null; 
    this.patientForm.patchValue({
      ...patient,
    });
  }

  // Delete patient
  deletePatient(id: number): void {
    if (confirm('Are you sure you want to delete this patient?')) {
      this.loading = true;
      this.patientService.deletePatient(id).subscribe({
        next: () => {
          this.showSuccess('Patient deleted successfully');
          this.loadPatients();
          this.loading = false;
        },
        error: (error) => {
          this.showError('Failed to delete patient');
          this.loading = false;
        }
      });
    }
  }

  // Reset form
  resetForm(): void {
    this.patientForm.reset();
    this.isEditing = false;
    this.editingPatientId = null;
    this.clearMessages();
  }

  // Show success message
  showSuccess(message: string): void {
    this.success = message;
    this.error = '';
    setTimeout(() => this.success = '', 5000);
  }

  // Show error message
  showError(message: string): void {
    this.error = message;
    this.success = '';
    setTimeout(() => this.error = '', 5000);
  }

  // Clear messages
  clearMessages(): void {
    this.error = '';
    this.success = '';
  }

  // Get form control for validation
  get f() {
    return this.patientForm.controls;
  }
}