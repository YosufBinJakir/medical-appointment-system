// patient.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Patient } from './patient';


@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private apiUrl = 'http://localhost:5055/api/patients'; // Adjust URL as needed

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) { }

  // GET: Get all patients
  getPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET: Get patient by ID
  getPatient(id: number): Observable<Patient> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<Patient>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  // POST: Create new patient
  createPatient(patient: Patient): Observable<Patient> {
    return this.http.post<Patient>(this.apiUrl, patient, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  // PUT: Update existing patient
  updatePatient(id: number, patient: Patient): Observable<any> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.put(url, patient, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  // DELETE: Delete patient
  deletePatient(id: number): Observable<any> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.delete(url, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: any): Observable<never> {
    console.error('An error occurred:', error);
    return throwError(() => new Error(error.message || 'Server error'));
  }
}