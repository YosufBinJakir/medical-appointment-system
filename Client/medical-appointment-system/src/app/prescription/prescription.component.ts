/* import { Component, OnInit } from '@angular/core';
import { AppointmentsService } from '../services/appointments.service';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PrescriptionsService } from '../services/prescriptions.service';

@Component({
  selector: 'app-prescription',
  standalone: true,
  imports: [CommonModule, DatePipe,FormsModule],
  templateUrl: './prescription.component.html',
  styleUrls: ['./prescription.component.css']
})
export class PrescriptionComponent implements OnInit {
  isEditing:boolean = false;
  doctors: any = [];
  patients: any[] =  [];
  medicines: any[] =  [];
  prescriptions: any = { data: [] };
  pagination: any;
  searchInput: string = '';
 // appointments: any[] = [];

  constructor(private prescriptionService: PrescriptionsService) {}


      getPrescriptionList(pageNumber: number = 1, pageSize: number = 10, searchInput: string = '') {
        this.prescriptionService.getPrescriptions(searchInput,pageNumber, pageSize)
          .subscribe({
            next: (res: any) => {
              this.prescriptions = res;

        
              this.pagination = res.pagination;
              //console.log(this.pagination)
            },
            error: (err) => console.error(err)
          });
      }

      getDotorList(){
        this.prescriptionService.getDoctors().subscribe({
          next: (res:any)=>{
            this.doctors = res;
            console.log(this.doctors);
          },
          error: (err) => console.error(err)
        })
      }

      getPatientList(){
        this.prescriptionService.getPatients().subscribe({
          next: (res:any)=>{
            this.patients = res;
            //console.log(this.patients);
          },
          error: (err) => console.error(err)
        })
      }

      getMedicineList(){
        this.prescriptionService.getMedicines().subscribe({
          next: (res:any)=>{
            this.medicines = res;
            //console.log(this.medicines);
          },
          error: (err) => console.error(err)
        })
      }
  ngOnInit() {
    if(this.isEditing){
      this.getDotorList();
    }
    this.getPrescriptionList(); 
    this.getPatientList(); 
    this.getMedicineList(); 
    this.getDotorList(); 
  }
}
 */

import { Component, OnInit } from '@angular/core';
import { AppointmentsService } from '../services/appointments.service';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PrescriptionsService } from '../services/prescriptions.service';
import { PrescriptionDto } from '../appoint-form-dto';

@Component({
  selector: 'app-prescription',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './prescription.component.html',
  styleUrls: ['./prescription.component.css']
})
export class PrescriptionComponent implements OnInit {
  editingPrescriptionId: number | null = null; // Track which prescription is being edited
  originalDoctorId: number | null = null; // Store original doctor ID for cancel functionality
  
  doctors: any[] = [];
  patients: any[] = [];
  medicines: any[] = [];
  prescriptions: any = { data: [] };
  pagination: any;
  searchInput: string = '';

  constructor(private prescriptionService: PrescriptionsService) {}

  getPrescriptionList(pageNumber: number = 1, pageSize: number = 10, searchInput: string = '') {
    this.prescriptionService.getPrescriptions(searchInput, pageNumber, pageSize)
      .subscribe({
        next: (res: any) => {
          this.prescriptions = res;
          this.pagination = res.pagination;
        },
        error: (err) => console.error(err)
      });
  }

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



  editingField: { prescriptionId: number, field: string } | null = null;
originalValues: any = {}; // Optional: store original values for cancel

startEditing(prescription: any, field: string) {
  this.editingField = { prescriptionId: prescription.prescriptionDetailId, field };
  this.originalValues[prescription.prescriptionDetailId + '_' + field] = prescription[field];
}

/* cancelEditing(prescription: any, field: string) {
  prescription[field] = this.originalValues[prescription.prescriptionDetailId + '_' + field];
  this.editingField = null;
} */

  cancelEditing(prescription: any) {
    const keys = Object.keys(prescription);
    keys.forEach(field => {
      const saved = this.originalValues[prescription.prescriptionDetailId + '_' + field];
      if (saved !== undefined) {
        prescription[field] = saved;
      }
    });
    this.editingField = null;
  }
  

isEditing(prescriptionId: number, field: string): boolean {
  return this.editingField?.prescriptionId === prescriptionId &&
         this.editingField?.field === field;
}

  

savePrescription(id :number, prescription: any) {
  debugger
  const dto: PrescriptionDto = {
    medicineId: prescription.medicineId,
    dosage: prescription.dosage,
    notes: prescription.notes,
    startDate: prescription.startDate,
    endDate: prescription.endDate
  };

  this.prescriptionService.updatePrescription(id, dto)
    .subscribe({
      next: () => {
        this.editingPrescriptionId = null; 
      },
      error: err => console.error(err)
    });
}
deletePrescription(id: number) {
  if (confirm('Are you sure you want to delete this prescription?')) {
    this.prescriptionService.deletePrescription(id).subscribe({
      next: () => {
        // Remove from local list after delete
        this.prescriptions = this.prescriptions.filter((p:any) => p.prescriptionDetailId !== id);
      },
      error: err => console.error('Delete failed:', err)
    });
  }
}
/* editingPrescriptionIds: number | null = null;
backupPrescription: any = {};  // will hold original values temporarily

startEditings(p: any) {
  this.editingPrescriptionId = p.id;
  this.backupPrescription[p.id] = { ...p }; // make a copy
}

cancelEditings(p: any, field: string) {
  if (this.backupPrescription[p.id]) {
    p[field] = this.backupPrescription[p.id][field]; // restore previous value
  }
  this.editingPrescriptionId = null;
}
 */

  /* editingField: { prescriptionId: number, field: string } | null = null;

startEditing(prescription: any, field: string) {
  this.editingField = { prescriptionId: prescription.prescriptionDetailId, field };
}

cancelEditing() {
  this.editingField = null;
}

isEditing(prescriptionId: number, field: string): boolean {
  return this.editingField?.prescriptionId === prescriptionId &&
         this.editingField?.field === field;
}
 */

  // Start editing a specific prescription
/*   startEditing(prescription: any) {
    this.editingPrescriptionId = prescription.prescriptionDetailId; 
    this.originalDoctorId = prescription.doctorId; // Store original value
  }
 */
  // Save the updated doctor selection
  /* saveDoctor(prescription: any) {
    // Call your API to update the prescription with new doctorId
    this.prescriptionService.updatePrescriptionDoctor(prescription.id, prescription.doctorId)
      .subscribe({
        next: (res: any) => {
          console.log('Doctor updated successfully');
          this.editingPrescriptionId = null;
          this.originalDoctorId = null;
          // Optionally refresh the prescription list
          // this.getPrescriptionList();
        },
        error: (err) => {
          console.error('Error updating doctor:', err);
          // Revert to original doctor ID on error
          prescription.doctorId = this.originalDoctorId;
        }
      });
  } */

  // Cancel editing and revert changes
  /* cancelEditing(prescription: any) {
    prescription.doctorId = this.originalDoctorId; // Revert to original value
    this.editingPrescriptionId = null;
    this.originalDoctorId = null;
  }

  // Check if a specific prescription is being edited
  isEditing(prescriptionId: number): boolean {
    return this.editingPrescriptionId === prescriptionId;
  } */

  ngOnInit() {
    this.getPrescriptionList();
    this.getPatientList();
    this.getMedicineList();
    this.getDoctorList();
  }
}