export interface Customer {
  customerId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  createdDate: string;
  updatedDate?: string;
  isActive: boolean;
  contact?: CustomerContact;
  employment?: CustomerEmployment;
  documents: CustomerDocument[];
}

export interface CustomerContact {
  contactId?: number;
  customerId?: number;
  email?: string;
  primaryPhone?: string;
  secondaryPhone?: string;
  address?: string;
  isActive: boolean;
}

export interface CustomerContactCreateUpdate {
  contactId?: number;
  email?: string;
  primaryPhone?: string;
  secondaryPhone?: string;
  address?: string;
  isActive?: boolean;
}

export interface CustomerEmployment {
  employmentId?: number;
  customerId?: number;
  industryId: number;
  industryName?: string;
  companyName?: string;
  jobTitle?: string;
  isActive: boolean;
}

export interface CustomerEmploymentCreateUpdate {
  employmentId?: number;
  industryId: number;
  companyName?: string;
  jobTitle?: string;
  isActive?: boolean;
}

export interface CustomerCreateUpdate {
  firstName: string;
  lastName: string;
  contact?: CustomerContactCreateUpdate;
  employment?: CustomerEmploymentCreateUpdate;
}

export interface CustomerDocument {
  documentId: number;
  customerId: number;
  documentType: string;
  fileName: string;
  filePath: string;
  contentType: string;
  fileSize: number;
  uploadedDate: string;
  isActive: boolean;
}

export interface Industry {
  industryId: number;
  industryName: string;
  isActive: boolean;
}

export interface CustomerListDto {
  customerId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  createdDate: string;
  email?: string;
  phone?: string;
  secondaryPhone?: string;
  address?: string;
  companyName?: string;
  industryName?: string;
  profileImageUrl?: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiErrorResponse {
  message?: string;
  title?: string;
  errors?: Record<string, string[]>;
}
