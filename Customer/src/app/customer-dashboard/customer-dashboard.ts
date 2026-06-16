import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { finalize, Subject, debounceTime, distinctUntilChanged, forkJoin } from 'rxjs';
import { CustomerService } from '../services/customer.service';
import { IndustryService } from '../services/industry.service';
import { DocumentService } from '../services/document.service';
import { NotificationService } from '../services/notification.service';
import {
  Industry,
  CustomerCreateUpdate,
  Customer as BackendCustomer,
} from '../models/customer.model';
import { environment } from '../../environments/environment';

type Section = 'home' | 'add' | 'view' | 'edit';

interface Customer {
  id?: string;
  name: string;
  firstName?: string;
  lastName?: string;
  profileImage?: string;
  email: string;
  phone: string;
  secondaryPhone?: string;
  address?: string;
  company?: string;
  jobTitle?: string;
  industry?: string;
}

interface CustomerForm {
  profileImage: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  secondaryPhone: string;
  address: string;
  company: string;
  jobTitle: string;
  industry: string;
}

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer-dashboard.html',
  styleUrl: './customer-dashboard.css',
})
export class CustomerDashboard implements OnInit, OnDestroy {
  isDarkMode = false;
  activeSection: Section = 'home';
  customers: Customer[] = [];
  customerListEmptyMessage = 'Get started by adding your first customer.';
  editCustomerId = '';
  editDraft = this.emptyForm();
  editOriginal = this.emptyForm();
  newCustomer = this.emptyForm();
  notice = '';
  searchTerm = '';
  private searchSubject = new Subject<string>();

  /* Pagination & Filter State */
  isLoading = false;
  isIndustryLoading = false;
  industries: Industry[] = [];
  filterIndustryId = '';
  sortBy = '';
  isDescending = false;
  pageNumber = 1;
  pageSize = 5;
  totalCount = 0;
  totalPages = 1;
  hasPreviousPage = false;
  hasNextPage = false;
  apiBaseUrl = environment.apiBaseUrl;

  private editOriginalContactId?: number;
  private editOriginalEmploymentId?: number;

  /* Multi-step form state */
  currentStep = 1;
  readonly totalSteps = 3;

  /* Camera state */
  showCamera = false;
  cameraTarget: 'new' | 'edit' = 'new';
  private cameraStream: MediaStream | null = null;

  @ViewChild('cameraVideo') cameraVideoRef!: ElementRef<HTMLVideoElement>;
  @ViewChild('cameraCanvas') cameraCanvasRef!: ElementRef<HTMLCanvasElement>;

  constructor(
    private customerService: CustomerService,
    private industryService: IndustryService,
    private documentService: DocumentService,
    private notificationService: NotificationService,
  ) {}

  ngOnInit(): void {
    this.initTheme();
    this.loadIndustries();
    this.loadCustomers();

    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.pageNumber = 1;
        this.loadCustomers();
      });
  }

  initTheme(): void {
    if (typeof window !== 'undefined' && window.localStorage) {
      const savedTheme = localStorage.getItem('theme');
      if (savedTheme === 'dark') {
        this.isDarkMode = true;
        document.body.classList.add('dark-theme');
      } else {
        this.isDarkMode = false;
        document.body.classList.remove('dark-theme');
      }
    }
  }

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    if (typeof window !== 'undefined' && window.localStorage) {
      if (this.isDarkMode) {
        document.body.classList.add('dark-theme');
        localStorage.setItem('theme', 'dark');
      } else {
        document.body.classList.remove('dark-theme');
        localStorage.setItem('theme', 'light');
      }
    }
  }

  get selectedCustomer(): Customer | undefined {
    return this.customers.find((customer) => customer.id === this.editCustomerId);
  }

  get hasEditChanges(): boolean {
    const d = this.editDraft;
    const o = this.editOriginal;
    return (
      d.profileImage !== o.profileImage ||
      d.firstName !== o.firstName ||
      d.lastName !== o.lastName ||
      d.email !== o.email ||
      d.phone !== o.phone ||
      d.secondaryPhone !== o.secondaryPhone ||
      d.address !== o.address ||
      d.company !== o.company ||
      d.jobTitle !== o.jobTitle ||
      d.industry !== o.industry
    );
  }

  /* ── Step validation ── */

  get isStep1Valid(): boolean {
    return !!this.newCustomer.profileImage;
  }

  get isStep2Valid(): boolean {
    const f = this.newCustomer;
    const namePattern = /^[A-Za-z]+( [A-Za-z]+)*$/;
    const phonePattern = /^(?!(\d)\1{9}$)[0-9]{10}$/;
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    return (
      namePattern.test(f.firstName.trim()) &&
      namePattern.test(f.lastName.trim()) &&
      emailPattern.test(f.email.trim()) &&
      phonePattern.test(f.phone.trim()) &&
      phonePattern.test(f.secondaryPhone.trim()) &&
      f.address.trim().length > 0
    );
  }

  get isStep3Valid(): boolean {
    const f = this.newCustomer;
    const namePattern = /^[A-Za-z0-9]+( [A-Za-z0-9]+)*$/;

    return (
      namePattern.test(f.company.trim()) && namePattern.test(f.jobTitle.trim()) && !!f.industry
    );
  }

  get isCurrentStepValid(): boolean {
    switch (this.currentStep) {
      case 1:
        return this.isStep1Valid;
      case 2:
        return this.isStep2Valid;
      case 3:
        return this.isStep3Valid;
      default:
        return false;
    }
  }

  nextStep(): void {
    if (this.currentStep < this.totalSteps && this.isCurrentStepValid) {
      this.currentStep++;
    }
  }

  prevStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  goToStep(step: number): void {
    /* Only allow going back, or forward if all prior steps are valid */
    if (step < this.currentStep) {
      this.currentStep = step;
    } else if (step === 2 && this.isStep1Valid) {
      this.currentStep = step;
    } else if (step === 3 && this.isStep1Valid && this.isStep2Valid) {
      this.currentStep = step;
    }
  }

  /* ── Section navigation ── */

  setSection(section: Section): void {
    this.activeSection = section;
    this.notice = '';

    if (section === 'add') {
      this.currentStep = 1;
    }

    if (section === 'edit' && !this.selectedCustomer && this.customers[0]?.id) {
      this.selectCustomerForEdit(this.customers[0].id);
    }
  }

  /* ── CRUD ── */

  loadCustomers(): void {
    this.isLoading = true;
    const industryId = this.filterIndustryId ? Number(this.filterIndustryId) : undefined;

    this.customerService
      .getCustomers(
        this.searchTerm.trim(),
        industryId,
        this.sortBy || undefined,
        this.isDescending,
        this.pageNumber,
        this.pageSize,
      )
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (res) => {
          this.totalCount = res.totalCount;
          this.totalPages = res.totalPages || 1;
          this.hasPreviousPage = res.hasPreviousPage;
          this.hasNextPage = res.hasNextPage;
          this.pageNumber = res.pageNumber || this.pageNumber;

          this.customers = (res.items || []).map((item) => ({
            id: item.customerId.toString(),
            name: item.fullName,
            firstName: item.firstName,
            lastName: item.lastName,
            email: item.email || '',
            phone: item.phone || '',
            secondaryPhone: item.secondaryPhone || '',
            address: item.address || '',
            company: item.companyName || '',
            industry: item.industryName || '',
            profileImage: this.toAssetUrl(item.profileImageUrl),
          }));

          if (this.customers.length === 0 && (this.searchTerm || this.filterIndustryId)) {
            this.customerListEmptyMessage = 'Try adjusting your search, sort, or industry filter.';
            this.notificationService.noRecordsFound('No records found matching your filters.');
          } else {
            this.customerListEmptyMessage = 'Get started by adding your first customer.';
          }
        },
        error: (err) => {
          this.customers = [];
          this.totalCount = 0;
          this.totalPages = 1;
          this.hasPreviousPage = false;
          this.hasNextPage = false;
          this.customerListEmptyMessage = 'We could not load customers right now.';
          this.notificationService.showApiError(err, 'Failed to load customers.');
        },
      });
  }

  loadIndustries(): void {
    this.isIndustryLoading = true;
    this.industryService
      .getIndustries()
      .pipe(
        finalize(() => {
          this.isIndustryLoading = false;
        }),
      )
      .subscribe({
        next: (res) => {
          this.industries = res.filter((industry) => industry.isActive);
        },
        error: (error) => {
          this.industries = [];
          this.notificationService.showApiError(error, 'Failed to load industries.');
        },
      });
  }

  onSearchChange(): void {
    this.searchSubject.next(this.searchTerm);
  }

  toggleSort(column: string): void {
    if (this.sortBy === column) {
      this.isDescending = !this.isDescending;
    } else {
      this.sortBy = column;
      this.isDescending = false;
    }
    this.pageNumber = 1;
    this.loadCustomers();
  }

  toggleSortDirection(): void {
    this.isDescending = !this.isDescending;
    this.pageNumber = 1;
    this.loadCustomers();
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.pageNumber = page;
      this.loadCustomers();
    }
  }

  nextPage(): void {
    if (this.hasNextPage) {
      this.pageNumber++;
      this.loadCustomers();
    }
  }

  prevPage(): void {
    if (this.hasPreviousPage) {
      this.pageNumber--;
      this.loadCustomers();
    }
  }

  addCustomer(form: NgForm): void {
    if (form.invalid) {
      this.notificationService.invalidForm();
      return;
    }

    if (!this.newCustomer.industry) {
      this.notificationService.missingRequiredFields(
        'Please select an industry before saving the customer.',
      );
      return;
    }

    const dto: CustomerCreateUpdate = {
      firstName: this.newCustomer.firstName.trim(),
      lastName: this.newCustomer.lastName.trim(),
      contact: {
        email: this.newCustomer.email.trim(),
        primaryPhone: this.newCustomer.phone.trim(),
        secondaryPhone: this.newCustomer.secondaryPhone.trim(),
        address: this.newCustomer.address.trim(),
        isActive: true,
      },
      employment: {
        industryId: Number(this.newCustomer.industry),
        companyName: this.newCustomer.company.trim(),
        jobTitle: this.newCustomer.jobTitle.trim(),
        isActive: true,
      },
    };

    this.isLoading = true;
    this.customerService
      .createCustomer(dto)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (created) => {
          this.handleCustomerSaved(
            created.customerId,
            this.newCustomer.profileImage,
            'created',
            () => {
              this.resetAddForm(form);
              this.activeSection = 'view';
            },
          );
        },
        error: (err) => {
          this.notificationService.showApiError(err, 'Failed to create customer.');
        },
      });
  }

  selectCustomerForEdit(id: string | undefined): void {
    if (!id) {
      return;
    }

    const customerId = Number(id);
    this.isLoading = true;
    this.customerService
      .getCustomer(customerId)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: (customer) => {
          this.editCustomerId = id;
          this.editDraft = this.formFromCustomer(customer);
          this.editOriginal = this.formFromCustomer(customer);
          this.activeSection = 'edit';
          this.notice = '';
        },
        error: (err) => {
          this.notificationService.showApiError(err, 'Failed to load customer details.');
        },
      });
  }

  updateCustomer(form: NgForm): void {
    if (form.invalid || !this.editCustomerId) {
      this.notificationService.invalidForm();
      return;
    }

    if (!this.editDraft.industry) {
      this.notificationService.missingRequiredFields(
        'Please select an industry before updating the customer.',
      );
      return;
    }

    const customerId = Number(this.editCustomerId);
    const dto: CustomerCreateUpdate = {
      firstName: this.editDraft.firstName.trim(),
      lastName: this.editDraft.lastName.trim(),
      contact: {
        contactId: this.editOriginalContactId,
        email: this.editDraft.email.trim(),
        primaryPhone: this.editDraft.phone.trim(),
        secondaryPhone: this.editDraft.secondaryPhone.trim(),
        address: this.editDraft.address.trim(),
        isActive: true,
      },
      employment: {
        employmentId: this.editOriginalEmploymentId,
        industryId: Number(this.editDraft.industry),
        companyName: this.editDraft.company.trim(),
        jobTitle: this.editDraft.jobTitle.trim(),
        isActive: true,
      },
    };

    const hasNewImage = this.editDraft.profileImage.startsWith('data:image/');

    if (hasNewImage) {
      let file: File;
      try {
        file = this.dataURLtoFile(this.editDraft.profileImage, `profile_${customerId}.png`);
      } catch {
        this.notificationService.error(
          'Customer details updated failed to prepare because the selected image was invalid.',
          'Upload Failure',
        );
        return;
      }

      this.isLoading = true;
      forkJoin({
        update: this.customerService.updateCustomer(customerId, dto),
        upload: this.documentService.uploadDocument(customerId, 'ProfileImage', file),
      })
        .pipe(
          finalize(() => {
            this.isLoading = false;
          }),
        )
        .subscribe({
          next: () => {
            this.notificationService.customerUpdated();
            this.notificationService.uploadCompleted();
            this.loadCustomers();
            this.activeSection = 'view';
          },
          error: (err) => {
            this.notificationService.showApiError(err, 'Failed to update customer or upload profile image.');
            this.loadCustomers();
            this.activeSection = 'view';
          },
        });
    } else {
      this.isLoading = true;
      this.customerService
        .updateCustomer(customerId, dto)
        .pipe(
          finalize(() => {
            this.isLoading = false;
          }),
        )
        .subscribe({
          next: () => {
            this.notificationService.customerUpdated();
            this.loadCustomers();
            this.activeSection = 'view';
          },
          error: (err) => {
            this.notificationService.showApiError(err, 'Failed to update customer.');
          },
        });
    }
  }

  deleteCustomer(id: string | undefined): void {
    if (!id) {
      return;
    }

    if (confirm('Are you sure you want to soft delete this customer?')) {
      this.isLoading = true;
      const customerId = Number(id);
      this.customerService
        .deleteCustomer(customerId)
        .pipe(
          finalize(() => {
            this.isLoading = false;
          }),
        )
        .subscribe({
          next: () => {
            this.notificationService.customerDeleted();

            if (this.editCustomerId === id) {
              this.editCustomerId = '';
              this.editDraft = this.emptyForm();
              this.editOriginal = this.emptyForm();
            }

            if (this.customers.length === 1 && this.pageNumber > 1) {
              this.pageNumber--;
            }

            this.loadCustomers();
            this.activeSection = 'view';
          },
          error: (err) => {
            this.notificationService.showApiError(err, 'Failed to delete customer.');
          },
        });
    }
  }

  /* ── Helpers ── */

  filteredCustomers(): Customer[] {
    return this.customers;
  }

  initials(customer: Customer): string {
    return customer.name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part[0])
      .join('')
      .toUpperCase();
  }

  digitsOnly(value: string): string {
    return value.replace(/\D/g, '').slice(0, 10);
  }

  onImageSelected(event: Event, target: 'new' | 'edit'): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      return;
    }

    if (!file.type.startsWith('image/')) {
      this.notificationService.warning('Please select a valid image file.', 'Upload Failure');
      input.value = '';
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const image = reader.result?.toString() || '';
      if (target === 'new') {
        this.newCustomer.profileImage = image;
      } else {
        this.editDraft.profileImage = image;
      }
    };
    reader.readAsDataURL(file);
  }

  removeImage(target: 'new' | 'edit'): void {
    if (target === 'new') {
      this.newCustomer.profileImage = '';
    } else {
      this.editDraft.profileImage = '';
    }
  }

  /* ── Camera ── */

  async openCamera(target: 'new' | 'edit'): Promise<void> {
    this.cameraTarget = target;
    this.showCamera = true;

    try {
      this.cameraStream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: 'user', width: { ideal: 640 }, height: { ideal: 480 } },
        audio: false,
      });

      /* Wait one tick for the ViewChild to render */
      setTimeout(() => {
        if (this.cameraVideoRef?.nativeElement) {
          this.cameraVideoRef.nativeElement.srcObject = this.cameraStream;
        }
      });
    } catch {
      this.showCamera = false;
      this.notificationService.warning('Camera access was denied or unavailable.', 'Warning');
    }
  }

  capturePhoto(): void {
    const video = this.cameraVideoRef?.nativeElement;
    const canvas = this.cameraCanvasRef?.nativeElement;
    if (!video || !canvas) return;

    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    ctx.drawImage(video, 0, 0);
    const image = canvas.toDataURL('image/png');

    if (this.cameraTarget === 'new') {
      this.newCustomer.profileImage = image;
    } else {
      this.editDraft.profileImage = image;
    }

    this.closeCamera();
  }

  closeCamera(): void {
    if (this.cameraStream) {
      this.cameraStream.getTracks().forEach((track) => track.stop());
      this.cameraStream = null;
    }
    this.showCamera = false;
  }

  ngOnDestroy(): void {
    this.closeCamera();
    this.searchSubject.complete();
  }

  /* ── Private ── */

  private resetAddForm(form: NgForm): void {
    this.newCustomer = this.emptyForm();
    this.currentStep = 1;
    form.resetForm(this.newCustomer);
  }

  private formFromCustomer(customer: BackendCustomer): CustomerForm {
    this.editOriginalContactId = customer.contact?.contactId;
    this.editOriginalEmploymentId = customer.employment?.employmentId;

    const profileDoc = [...(customer.documents || [])]
      .filter((d) => d.isActive && d.documentType === 'ProfileImage')
      .sort((a, b) => new Date(b.uploadedDate).getTime() - new Date(a.uploadedDate).getTime())[0];
    const profileImage = this.toAssetUrl(profileDoc?.filePath, profileDoc?.uploadedDate);

    return {
      profileImage: profileImage,
      firstName: customer.firstName || '',
      lastName: customer.lastName || '',
      email: customer.contact?.email || '',
      phone: customer.contact?.primaryPhone || '',
      secondaryPhone: customer.contact?.secondaryPhone || '',
      address: customer.contact?.address || '',
      company: customer.employment?.companyName || '',
      jobTitle: customer.employment?.jobTitle || '',
      industry: customer.employment?.industryId ? customer.employment.industryId.toString() : '',
    };
  }

  private emptyForm(): CustomerForm {
    return {
      profileImage: '',
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      secondaryPhone: '',
      address: '',
      company: '',
      jobTitle: '',
      industry: '',
    };
  }

  private dataURLtoFile(dataurl: string, filename: string): File {
    const arr = dataurl.split(',');
    const mime = arr[0].match(/:(.*?);/)?.[1] || 'image/png';
    const bstr = atob(arr[1]);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) {
      u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
  }

  private toAssetUrl(path?: string | null, version?: string): string {
    if (!path) {
      return '';
    }

    const separator = path.includes('?') ? '&' : '?';
    return version
      ? `${this.apiBaseUrl}${path}${separator}v=${encodeURIComponent(version)}`
      : `${this.apiBaseUrl}${path}`;
  }

  private handleCustomerSaved(
    customerId: number,
    profileImage: string,
    action: 'created' | 'updated',
    onComplete: () => void,
  ): void {
    const showSaveNotification = () => {
      if (action === 'created') {
        this.notificationService.customerCreated();
      } else {
        this.notificationService.customerUpdated();
      }
    };

    if (!profileImage.startsWith('data:image/')) {
      showSaveNotification();
      this.loadCustomers();
      onComplete();
      return;
    }

    let file: File;
    try {
      file = this.dataURLtoFile(profileImage, `profile_${customerId}.png`);
    } catch {
      showSaveNotification();
      this.notificationService.error(
        'Customer was saved, but the selected image could not be prepared for upload.',
        'Upload Failure',
      );
      this.loadCustomers();
      onComplete();
      return;
    }

    this.isLoading = true;
    this.documentService
      .uploadDocument(customerId, 'ProfileImage', file)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        }),
      )
      .subscribe({
        next: () => {
          showSaveNotification();
          this.notificationService.uploadCompleted();
          this.loadCustomers();
          onComplete();
        },
        error: (error) => {
          showSaveNotification();
          this.notificationService.showApiError(
            error,
            'Customer was saved, but profile image upload failed.',
            'Upload Failure',
          );
          this.loadCustomers();
          onComplete();
        },
      });
  }
}
