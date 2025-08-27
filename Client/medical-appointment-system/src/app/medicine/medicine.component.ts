import { Component, OnInit } from '@angular/core';
import { MedicineService } from '../medicine.service';
import { Medicine } from '../medicine';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';



@Component({
  selector: 'app-medicine',
  imports:[
    FormsModule, CommonModule, ReactiveFormsModule   
  ],
  templateUrl: './medicine.component.html',
  styleUrls: ['./medicine.component.css']
})
export class MedicineComponent implements OnInit {
  medicines: Medicine[] = [];
  selectedMedicine: Medicine = {} as Medicine;
  isEditing = false;
  showForm = false;
  errorMessage = '';

  constructor(private medicineService: MedicineService) { }

  ngOnInit(): void {
    this.loadMedicines();
  }

  // Load all medicines
  loadMedicines(): void {
    this.medicineService.getMedicines().subscribe({
      next: (data) => {
        this.medicines = data;
        this.errorMessage = '';
      },
      error: (error) => {
        this.errorMessage = 'Error loading medicines: ' + error.message;
        console.error('Error loading medicines:', error);
      }
    });
  }

  // Select medicine for editing
  editMedicine(medicine: Medicine): void {
    this.selectedMedicine = { ...medicine };
    this.isEditing = true;
    this.showForm = true;
  }

  // Create new medicine
  createMedicine(): void {
    this.selectedMedicine = { medicineId: 0, medicineName: '' };
    this.isEditing = false;
    this.showForm = true;
  }

  // Save medicine (create or update)
  saveMedicine(): void {
    if (this.isEditing) {
      this.medicineService.updateMedicine(this.selectedMedicine).subscribe({
        next: () => {
          this.loadMedicines();
          this.cancelEdit();
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = 'Error updating medicine: ' + error.message;
          console.error('Error updating medicine:', error);
        }
      });
    } else {
      this.medicineService.addMedicine(this.selectedMedicine).subscribe({
        next: () => {
          this.loadMedicines();
          this.cancelEdit();
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = 'Error creating medicine: ' + error.message;
          console.error('Error creating medicine:', error);
        }
      });
    }
  }

  // Delete medicine
  deleteMedicine(id: number): void {
    if (confirm('Are you sure you want to delete this medicine?')) {
      this.medicineService.deleteMedicine(id).subscribe({
        next: () => {
          this.loadMedicines();
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = 'Error deleting medicine: ' + error.message;
          console.error('Error deleting medicine:', error);
        }
      });
    }
  }

  // Cancel editing
  cancelEdit(): void {
    this.showForm = false;
    this.isEditing = false;
    this.selectedMedicine = {} as Medicine;
  }
}