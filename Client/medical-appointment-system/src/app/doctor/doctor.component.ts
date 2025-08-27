// doctor.component.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { DoctorService } from '../doctor.service';
import { Doctor } from '../doctor';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-doctor',
  templateUrl: './doctor.component.html',
  imports :[
    FormsModule, CommonModule, ReactiveFormsModule   
  ],
  styleUrls: ['./doctor.component.css']
})
export class DoctorComponent implements OnInit {
  doctors: Doctor[] = [];
  doctorForm: FormGroup;
  isEditing = false;
  editingDoctorId: number | null = null;
  loading = false;
  error = '';
  success = '';
  searchTerm = '';

  constructor(
    private fb: FormBuilder,
    private doctorService: DoctorService
  ) {
    this.doctorForm = this.fb.group({
      doctorName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    this.loadDoctors();
  }

 
  loadDoctors(): void {
    this.loading = true;
    this.doctorService.getDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
        this.loading = false;
      },
      error: (error) => {
        this.showError('Failed to load doctors');
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.doctorForm.valid) {
      const doctor: Doctor = this.doctorForm.value;
      
      if (this.isEditing && this.editingDoctorId) {
        this.updateDoctor(this.editingDoctorId, doctor);
      } else {
        this.createDoctor(doctor);
      }
    } else {
      this.markFormGroupTouched();
    }
  }


  createDoctor(doctor: Doctor): void {
    this.loading = true;
    this.doctorService.createDoctor(doctor).subscribe({
      next: (response) => {
        this.showSuccess('Doctor created successfully');
        this.resetForm();
        this.loadDoctors();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Failed to create doctor');
        this.loading = false;
      }
    });
  }


  updateDoctor(id: number, doctor: Doctor): void {
    this.loading = true;
    this.doctorService.updateDoctor(id, doctor).subscribe({
      next: (response) => {
        this.showSuccess('Doctor updated successfully');
        this.resetForm();
        this.loadDoctors();
        this.loading = false;
      },
      error: (error) => {
        this.showError('Failed to update doctor');
        this.loading = false;
      }
    });
  }


  editDoctor(doctor: Doctor): void {
    this.isEditing = true;
    this.editingDoctorId = doctor.doctorId || null;
    
    this.doctorForm.patchValue({
      doctorName: doctor.doctorName
    });


    const formElement = document.getElementById('doctorForm');
    if (formElement) {
      formElement.scrollIntoView({ behavior: 'smooth' });
    }
  }


  deleteDoctor(id: number, doctorName: string): void {
    if (confirm(`Are you sure you want to delete Dr. ${doctorName}?`)) {
      this.loading = true;
      this.doctorService.deleteDoctor(id).subscribe({
        next: () => {
          this.showSuccess('Doctor deleted successfully');
          this.loadDoctors();
          this.loading = false;
        },
        error: (error) => {
          this.showError('Failed to delete doctor');
          this.loading = false;
        }
      });
    }
  }


  resetForm(): void {
    this.doctorForm.reset();
    this.isEditing = false;
    this.editingDoctorId = null;
    this.clearMessages();
  }


  showSuccess(message: string): void {
    this.success = message;
    this.error = '';
    setTimeout(() => this.success = '', 5000);
  }


  showError(message: string): void {
    this.error = message;
    this.success = '';
    setTimeout(() => this.error = '', 5000);
  }


  clearMessages(): void {
    this.error = '';
    this.success = '';
  }

  markFormGroupTouched(): void {
    Object.keys(this.doctorForm.controls).forEach(key => {
      this.doctorForm.get(key)?.markAsTouched();
    });
  }


  get f() {
    return this.doctorForm.controls;
  }


  get filteredDoctors(): Doctor[] {
    if (!this.searchTerm) {
      return this.doctors;
    }
    return this.doctors.filter(doctor => 
      doctor.doctorName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }


  clearSearch(): void {
    this.searchTerm = '';
  }
}