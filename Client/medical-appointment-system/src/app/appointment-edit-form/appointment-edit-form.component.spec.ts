import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentEditFormComponent } from './appointment-edit-form.component';

describe('AppointmentEditFormComponent', () => {
  let component: AppointmentEditFormComponent;
  let fixture: ComponentFixture<AppointmentEditFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppointmentEditFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AppointmentEditFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
