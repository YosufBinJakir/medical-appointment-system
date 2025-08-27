// doctor.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Doctor } from './doctor';


@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private apiUrl = 'http://localhost:5055/api/doctors'; // Adjust URL as needed

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) { }

  // GET: Get all doctors
  getDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET: Get doctor by ID
  getDoctor(id: number): Observable<Doctor> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<Doctor>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  // POST: Create new doctor
  createDoctor(doctor: Doctor): Observable<Doctor> {
    return this.http.post<Doctor>(this.apiUrl, doctor, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  // PUT: Update existing doctor
  updateDoctor(id: number, doctor: Doctor): Observable<any> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.put(url, doctor, this.httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  // DELETE: Delete doctor
  deleteDoctor(id: number): Observable<any> {
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