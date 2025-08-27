export interface PrescriptionDetailFormDto {
    medicineId?: number;
    dosage?: string;
    startDate?: string|Date;
    endDate?: string|Date;
    notes?: string;
  }
  
  export interface AppointmentFormDto {
    patientId?: number;
    doctorId?: number;
    appointmentDate?: string|Date;
    visitType?: string;
    notes?: string;
    diagnosis?: string;
    prescriptionDetailFormDtos: PrescriptionDetailFormDto[];
  }

  export interface PrescriptionDto {
    medicineId: number;
    dosage: string;
    notes: string;
    startDate: string;
    endDate: string;
  }