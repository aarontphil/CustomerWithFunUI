import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CustomerDocument } from '../models/customer.model';

@Injectable({
  providedIn: 'root',
})
export class DocumentService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/customers`;

  constructor(private http: HttpClient) {}

  uploadDocument(
    customerId: number,
    documentType: string,
    file: File,
  ): Observable<CustomerDocument> {
    const formData = new FormData();
    formData.append('documentType', documentType);
    formData.append('file', file);

    return this.http.post<CustomerDocument>(`${this.baseUrl}/${customerId}/documents`, formData);
  }

  deleteDocument(customerId: number, documentId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${customerId}/documents/${documentId}`);
  }
}
