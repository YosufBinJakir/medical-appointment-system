import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AppointmentComponent } from './appointment/appointment.component';
import { PrescriptionComponent } from './prescription/prescription.component';

export const routes: Routes = [
    {path:'', component:HomeComponent},
    {path: 'home', component: HomeComponent},
    {path: 'appointments', component: AppointmentComponent},
    {path: 'prescriptionDetail', component: PrescriptionComponent},
    {path: '**', redirectTo: 'home'}
];
