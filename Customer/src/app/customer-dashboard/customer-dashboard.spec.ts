import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CustomerDashboard } from './customer-dashboard';
import { CustomerService } from '../services/customer.service';
import { IndustryService } from '../services/industry.service';
import { DocumentService } from '../services/document.service';
import { NotificationService } from '../services/notification.service';
import { of } from 'rxjs';

describe('CustomerDashboard', () => {
  let component: CustomerDashboard;
  let fixture: ComponentFixture<CustomerDashboard>;

  const mockCustomerService = {
    getCustomers: () =>
      of({
        items: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 5,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      }),
    getCustomer: () => of({}),
    createCustomer: () => of({}),
    updateCustomer: () => of({}),
    deleteCustomer: () => of({}),
  };

  const mockIndustryService = {
    getIndustries: () => of([]),
  };

  const mockDocumentService = {
    uploadDocument: () => of({}),
    deleteDocument: () => of({}),
  };

  const mockNotificationService = {
    success: () => {},
    error: () => {},
    warning: () => {},
    info: () => {},
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerDashboard],
      providers: [
        { provide: CustomerService, useValue: mockCustomerService },
        { provide: IndustryService, useValue: mockIndustryService },
        { provide: DocumentService, useValue: mockDocumentService },
        { provide: NotificationService, useValue: mockNotificationService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
