import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PrescriptionDto } from '../appoint-form-dto';

@Injectable({
  providedIn: 'root'
})
export class PrescriptionsService {
  private apiUrl = 'http://localhost:5055/api';

  constructor(private http: HttpClient) {}

  getPrescriptions(searchInput: string, pageNumber: number, pageSize: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/prescriptionDetails?searchInput=${searchInput}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
  getDoctors(){
    return this.http.get<any[]>(`${this.apiUrl}/doctors`);
  }

  getPatients(){
    return this.http.get<any[]>(`${this.apiUrl}/patients`);
  }

  getMedicines(){
    return this.http.get<any[]>(`${this.apiUrl}/medicines`);
  }
  updatePrescription(prescriptionDetailId: number, dto: PrescriptionDto): Observable<any> {
    return this.http.patch(`${this.apiUrl}/prescriptionDetails/${prescriptionDetailId}`, dto);
  }
  deletePrescription(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/prescriptionDetails/${id}`);
  }
}
