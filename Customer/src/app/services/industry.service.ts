import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Industry } from '../models/customer.model';

@Injectable({
  providedIn: 'root',
})
export class IndustryService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/industries`;

  constructor(private http: HttpClient) {}

  getIndustries(): Observable<Industry[]> {
    return this.http.get<Industry[]>(this.apiUrl);
  }
}
