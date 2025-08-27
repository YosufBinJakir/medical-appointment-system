import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AppointmentComponent } from './appointment/appointment.component';
import { PrescriptionComponent } from './prescription/prescription.component';
import { MedicineComponent } from './medicine/medicine.component';
import { DoctorComponent } from './doctor/doctor.component';
import { PatientComponent } from './patient/patient.component';

export const routes: Routes = [
    {path:'', component:HomeComponent},
    {path: 'home', component: HomeComponent},
    {path: 'appointments', component: AppointmentComponent},
    {path: 'prescriptionDetail', component: PrescriptionComponent},
    {path: 'medicines', component: MedicineComponent},
    {path: 'doctors', component: DoctorComponent},
    {path: 'patients', component: PatientComponent},
    {path: '**', redirectTo: 'home'}
];
