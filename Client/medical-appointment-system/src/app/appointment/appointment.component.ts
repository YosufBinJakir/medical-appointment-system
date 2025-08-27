import { Component, OnInit } from '@angular/core';
import { AppointmentsService } from '../services/appointments.service';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppointmentModalComponent } from "../appointment-form/appointment-form.component";
import { AppointmentEditFormComponent } from '../appointment-edit-form/appointment-edit-form.component';
import { Modal } from 'bootstrap';
import { PrescriptionsService } from '../services/prescriptions.service';

//declare var bootstrap: any;

export interface PrescriptionDetailFormDto {
  medicineId?: number;
  dosage?: string;
  startDate?: string;
  endDate?: string;
  notes?: string;
}

export interface AppointmentFormDto {
  patientId?: number;
  doctorId?: number;
  appointmentDate?: string;
  visitType?: string;
  notes?: string;
  diagnosis?: string;
  prescriptionDetailFormDtos: PrescriptionDetailFormDto[];
}


@Component({
  selector: 'app-appointment',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, AppointmentModalComponent, AppointmentEditFormComponent],
  templateUrl: './appointment.component.html',
  styleUrls: ['./appointment.component.css']
})
export class AppointmentComponent implements OnInit {
  
  sendingMap: { [key: number]: boolean } = {};
  selectedAppointment: any = null;
  appointments: any = { data: [] };
  prescriptions: any = { data: [] };
  pagination: any;
  searchInput: string = '';

  appointmentId: number = 5; // This should come from your route or state
  patientEmail: string = '';
  isSending: boolean = false;
  message: string = '';
 // appointments: any[] = [];

  constructor(private appointmentService: AppointmentsService, private presService: PrescriptionsService) {}

/*   getAppointmentList(searchInput: string = '',pageNumber: number = 1, pageSize: number = 10) {
    this.appointmentService.getAppointments(searchInput,pageNumber, pageSize)
      .subscribe({
        next: (data: any) => {
          this.appointments = data;
          console.log('Appointments:', data);
        },
        error: (err) => {
          console.error('Error fetching appointments:', err);
        }
      });
  } */
 
      getAppointmentList(pageNumber: number = 1, pageSize: number = 10, searchInput: string = '') {
        this.appointmentService.getAppointments(searchInput,pageNumber, pageSize)
          .subscribe({
            next: (res: any) => {
              this.appointments = res;
              this.pagination = res.pagination;
              console.log(this.pagination)
            },
            error: (err) => console.error(err)
          });
      }
      getPrescriptionList(pageNumber: number = 1, pageSize: number = 10, searchInput: string = '') {
        this.presService.getPrescriptions(searchInput, pageNumber, pageSize)
          .subscribe({
            next: (res: any) => {
              this.prescriptions = res;
              this.pagination = res.pagination;
            },
            error: (err) => console.error(err)
          });
      }
  ngOnInit() {
    this.getAppointmentList(); 
    this.getPrescriptionList();
  }

  editForm: AppointmentFormDto = {
    patientId: 0,
    doctorId: 0,
    appointmentDate: '',
    visitType: '',
    notes: '',
    diagnosis: '',
    prescriptionDetailFormDtos: []
  };

  onEditAppointment(id: number) {
    this.appointmentService.getAppointmentById(id).subscribe({
      next: (data) => {
        this.selectedAppointment = data; 
        // open bootstrap modal
        const modal = new Modal(document.getElementById('editAppointmentment')!);
        modal.show();
      }
    });
  }
  delete(id: number): void {
    if (confirm('Are you sure you want to delete this appointment?')) {
      this.appointmentService.deleteAppointment(id).subscribe({
        next: () => {
          alert('Appointment deleted successfully!');
          // Optionally refresh the list
          this.getAppointmentList(); 
        },
        error: err => console.error('Error deleting appointment:', err)
      });
    }
  }
  
 /*  downloadAppointmentPdf(id: number) {
    this.appointmentService.downloadPdf(id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `${patientName}Appointment_${id}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Error downloading PDF:', err)
    });
  } */
    downloadAppointmentPdf(appointment: any) {
      this.appointmentService.downloadPdf(appointment.appointmentId).subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          // Use patientName from the appointment
          const patientName = appointment.patientName.replace(/\s+/g, '_'); // replace spaces with underscore
          a.download = `${patientName}_Appointment_${appointment.appointmentId}.pdf`;
          a.click();
          window.URL.revokeObjectURL(url);
        },
        error: (err) => console.error('Error downloading PDF:', err)
      });
    }
    


    /* sendToPatient(id:number, email:string) {
      debugger
      if (!email) {
        this.message = 'Please enter patient email';
        return;
      }
  
      if (!this.isValidEmail(email)) {
        this.message = 'Please enter a valid email address';
        return;
      }
  
      this.isSending = true;
      this.message = '';
  
      this.appointmentService.sendPrescriptionEmail(id, email)
        .subscribe({
          next: (response: any) => {
            this.isSending = false;
            this.message = 'Prescription sent successfully!';
            this.patientEmail = '';
          },
          error: (err) => {
            this.isSending = false;
            console.error('Error sending email:', err);
            this.message = 'Failed to send prescription. Please try again.';
          }
        });
    } */
  
   



  

        sendToPatient(appointmentId: number, email: string) {
          if (!email) {
            this.message = 'Please enter patient email';
            return;
          }
      
          if (!this.isValidEmail(email)) {
            this.message = 'Please enter a valid email address';
            return;
          }
          
          this.sendingMap[appointmentId] = true; 
      
          this.appointmentService.sendPrescriptionEmail(appointmentId, email).subscribe({
            next: () => {
              alert('Email sent successfully!');
              this.sendingMap[appointmentId] = false;
            },
            error: (err) => {
              console.error(err);
              alert('Failed to send email.');
              this.sendingMap[appointmentId] = false;
            }
          });
        }
private isValidEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}
clearSearch(): void {
  this.searchInput = '';
  this.getAppointmentList(1, 10, '');
}
refreshPage() {
  window.location.reload(); // refreshes the page
}

}
