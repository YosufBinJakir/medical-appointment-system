import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Medicine } from './medicine';


@Injectable({
  providedIn: 'root'
})
export class MedicineService {
  private apiUrl = 'http://localhost:5055/api/medicines'; // Adjust port as needed

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private http: HttpClient) { }

  // GET all medicines
  getMedicines(): Observable<Medicine[]> {
    return this.http.get<Medicine[]>(this.apiUrl);
  }

  // GET medicine by id
  getMedicine(id: number): Observable<Medicine> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.get<Medicine>(url);
  }

  // POST new medicine
  addMedicine(medicine: Medicine): Observable<Medicine> {
    return this.http.post<Medicine>(this.apiUrl, medicine, this.httpOptions);
  }

  // PUT update medicine
  updateMedicine(medicine: Medicine): Observable<any> {
    const url = `${this.apiUrl}/${medicine.medicineId}`;
    return this.http.put(url, medicine, this.httpOptions);
  }

  // DELETE medicine
  deleteMedicine(id: number): Observable<Medicine> {
    const url = `${this.apiUrl}/${id}`;
    return this.http.delete<Medicine>(url, this.httpOptions);
  }
}