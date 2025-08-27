import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppointmentFormDto } from '../appoint-form-dto';

@Injectable({
  providedIn: 'root'
})
export class AppointmentsService {
  private apiUrl = 'http://localhost:5055/api/appointments';

  constructor(private http: HttpClient) {}

  getAppointments(searchInput: string, pageNumber: number, pageSize: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?searchInput=${searchInput}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
  saveAppointment(data: AppointmentFormDto): Observable<any> {
    return this.http.post<any>(this.apiUrl, data);
  }

  getAppointmentById(id: number) {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
  
  


  getDoctors() {
    return this.http.get<{doctorId: number, doctorName: string}[]>('http://localhost:5055/api/doctors');
  }

  getPatients() {
    return this.http.get<{patientId: number, patientName: string}[]>('http://localhost:5055/api/patients');
  }

  getMedicines() {
    return this.http.get<{medicineId: number, medicineName: string}[]>('http://localhost:5055/api/medicines');
  }
  updateAppointment(appointment: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${appointment.appointmentId}`, appointment);
  }
  deleteAppointment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  downloadPdf(appointmentId: number) {
    return this.http.get(`${this.apiUrl}/${appointmentId}/pdf`, {
      responseType: 'blob' 
    });
  }
  


  sendPrescriptionEmail(appointmentId: number, patientEmail: string): Observable<any> {
    const request = {
      patientEmail: patientEmail,
      subject: 'Your Prescription from Healthcare Clinic',
      body: `
        <h3>Dear Patient,</h3>
        <p>Please find your prescription attached to this email.</p>
        <p>If you have any questions, please contact our clinic.</p>
        <br>
        <p>Best regards,<br>Healthcare Clinic Team</p>
      `
    };

    return this.http.post(`${this.apiUrl}/${appointmentId}/send-email`, request);
  }
  
  /* getAppointment(id: number) {
    return this.http.get<AppointmentDto>(`${this.apiUrl}/appointments/${id}`);
  } */
}
