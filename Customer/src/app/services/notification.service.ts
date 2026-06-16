import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { ApiErrorResponse } from '../models/customer.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private toastr: ToastrService) {}

  success(message: string, title?: string): void {
    this.toastr.success(message, title);
  }

  error(message: string, title?: string): void {
    this.toastr.error(message, title);
  }

  warning(message: string, title?: string): void {
    this.toastr.warning(message, title);
  }

  info(message: string, title?: string): void {
    this.toastr.info(message, title);
  }

  customerCreated(): void {
    this.success('Customer created successfully.', 'Success');
  }

  customerUpdated(): void {
    this.success('Customer updated successfully.', 'Success');
  }

  customerDeleted(): void {
    this.success('Customer deleted successfully.', 'Success');
  }

  uploadCompleted(): void {
    this.success('Upload completed successfully.', 'Upload Completed');
  }

  invalidForm(message = 'Please fill in all required fields.'): void {
    this.warning(message, 'Invalid Form');
  }

  missingRequiredFields(message = 'Missing required fields.'): void {
    this.warning(message, 'Missing Required Fields');
  }

  noRecordsFound(message = 'No records found.'): void {
    this.warning(message, 'No Records Found');
  }

  showApiError(error: unknown, fallbackMessage: string, title = 'API Failure'): void {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 0) {
        this.error('A network failure occurred while contacting the server.', 'Network Failure');
        return;
      }

      const apiError = error.error as ApiErrorResponse | string | null;
      if (typeof apiError === 'string' && apiError.trim()) {
        this.error(apiError, title);
        return;
      }

      if (apiError && typeof apiError === 'object') {
        if (apiError.message) {
          this.error(apiError.message, title);
          return;
        }

        const validationMessages = apiError.errors
          ? Object.values(apiError.errors).flat().filter(Boolean)
          : [];

        if (validationMessages.length) {
          this.error(validationMessages.join(' '), title);
          return;
        }
      }

      if (error.status >= 500) {
        this.error('An unexpected server error occurred.', 'Unexpected Server Error');
        return;
      }
    }

    this.error(fallbackMessage, title);
  }
}
