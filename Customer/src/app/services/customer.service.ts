import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Customer,
  CustomerCreateUpdate,
  CustomerListDto,
  PagedResponse,
} from '../models/customer.model';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/customers`;

  constructor(private http: HttpClient) {}

  getCustomers(
    search?: string,
    industryId?: number,
    sortBy?: string,
    isDescending?: boolean,
    pageNumber?: number,
    pageSize?: number,
  ): Observable<PagedResponse<CustomerListDto>> {
    let params = new HttpParams();
    if (search) {
      params = params.set('search', search);
    }
    if (industryId) {
      params = params.set('industryId', industryId.toString());
    }
    if (sortBy) {
      params = params.set('sortBy', sortBy);
    }
    if (isDescending !== undefined) {
      params = params.set('isDescending', isDescending.toString());
    }
    if (pageNumber) {
      params = params.set('pageNumber', pageNumber.toString());
    }
    if (pageSize) {
      params = params.set('pageSize', pageSize.toString());
    }

    return this.http.get<PagedResponse<CustomerListDto>>(this.apiUrl, { params });
  }

  getCustomer(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`);
  }

  createCustomer(dto: CustomerCreateUpdate): Observable<Customer> {
    return this.http.post<Customer>(this.apiUrl, dto);
  }

  updateCustomer(id: number, dto: CustomerCreateUpdate): Observable<Customer> {
    return this.http.put<Customer>(`${this.apiUrl}/${id}`, dto);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
