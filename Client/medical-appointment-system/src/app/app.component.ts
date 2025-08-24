import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppointmentComponent } from "./appointment/appointment.component";
import { PrescriptionComponent } from "./prescription/prescription.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AppointmentComponent, PrescriptionComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'medical-appointment-system';
}
