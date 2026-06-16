using JustCRM.API.DTOs;
using JustCRM.API.Entities;
using JustCRM.API.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace JustCRM.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IIndustryRepository _industryRepository;
        private readonly IWebHostEnvironment _environment;

        public CustomerService(
            ICustomerRepository customerRepository,
            IIndustryRepository industryRepository,
            IWebHostEnvironment environment)
        {
            _customerRepository = customerRepository;
            _industryRepository = industryRepository;
            _environment = environment;
        }

        public async Task<PagedResult<CustomerListDto>> GetPagedCustomersAsync(
            string? search, long? industryId, string? sortBy, bool isDescending, int pageNumber, int pageSize)
        {
            var (customers, totalCount) = await _customerRepository.GetPagedCustomersAsync(
                search, industryId, sortBy, isDescending, pageNumber, pageSize);

            var customerDtos = customers.Select(c => new CustomerListDto
            {
                CustomerId = c.CustomerId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                FullName = c.FullName,
                CreatedDate = c.CreatedDate,
                Email = c.CustomerContact?.Email,
                Phone = c.CustomerContact?.PrimaryPhone,
                SecondaryPhone = c.CustomerContact?.SecondaryPhone,
                Address = c.CustomerContact?.Address,
                CompanyName = c.CustomerEmployment?.CompanyName,
                IndustryName = c.CustomerEmployment?.Industry?.IndustryName,
                ProfileImageUrl = c.CustomerDocuments
                    .Where(d => d.IsActive && d.DocumentType == "ProfileImage")
                    .OrderByDescending(d => d.UploadedDate)
                    .Select(d => d.FilePath)
                    .FirstOrDefault()
            }).ToList();

            return new PagedResult<CustomerListDto>
            {
                Items = customerDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(long id)
        {
            var customer = await _customerRepository.GetCustomerDetailsAsync(id);
            if (customer == null)
                return null;

            return MapToCustomerDto(customer);
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerCreateUpdateDto dto)
        {
            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FullName = $"{dto.FirstName} {dto.LastName}".Trim(),
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            // Map contact (One-to-One)
            if (dto.Contact != null)
            {
                customer.CustomerContact = new CustomerContact
                {
                    Email = dto.Contact.Email,
                    PrimaryPhone = dto.Contact.PrimaryPhone,
                    SecondaryPhone = dto.Contact.SecondaryPhone,
                    Address = dto.Contact.Address,
                    IsActive = dto.Contact.IsActive
                };
            }

            // Map employment (One-to-One)
            if (dto.Employment != null)
            {
                // Validate Industry
                if (!await _industryRepository.IndustryExistsAsync(dto.Employment.IndustryId))
                    throw new ArgumentException($"Industry with Id {dto.Employment.IndustryId} does not exist.");

                customer.CustomerEmployment = new CustomerEmployment
                {
                    IndustryId = dto.Employment.IndustryId,
                    CompanyName = dto.Employment.CompanyName,
                    JobTitle = dto.Employment.JobTitle,
                    IsActive = dto.Employment.IsActive
                };
            }

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            // Reload customer details
            var createdCustomer = await _customerRepository.GetCustomerDetailsAsync(customer.CustomerId);
            return MapToCustomerDto(createdCustomer!);
        }

        public async Task<CustomerDto?> UpdateCustomerAsync(long id, CustomerCreateUpdateDto dto)
        {
            var customer = await _customerRepository.GetCustomerDetailsAsync(id);
            if (customer == null)
                return null;

            // Update basic fields
            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.FullName = $"{dto.FirstName} {dto.LastName}".Trim();
            customer.UpdatedDate = DateTime.UtcNow;

            // Update Contact (One-to-One)
            if (dto.Contact != null)
            {
                if (customer.CustomerContact != null)
                {
                    customer.CustomerContact.Email = dto.Contact.Email;
                    customer.CustomerContact.PrimaryPhone = dto.Contact.PrimaryPhone;
                    customer.CustomerContact.SecondaryPhone = dto.Contact.SecondaryPhone;
                    customer.CustomerContact.Address = dto.Contact.Address;
                    customer.CustomerContact.IsActive = dto.Contact.IsActive;
                }
                else
                {
                    customer.CustomerContact = new CustomerContact
                    {
                        Email = dto.Contact.Email,
                        PrimaryPhone = dto.Contact.PrimaryPhone,
                        SecondaryPhone = dto.Contact.SecondaryPhone,
                        Address = dto.Contact.Address,
                        IsActive = dto.Contact.IsActive
                    };
                }
            }
            else
            {
                customer.CustomerContact = null;
            }

            // Update Employment (One-to-One)
            if (dto.Employment != null)
            {
                // Validate Industry
                if (!await _industryRepository.IndustryExistsAsync(dto.Employment.IndustryId))
                    throw new ArgumentException($"Industry with Id {dto.Employment.IndustryId} does not exist.");

                if (customer.CustomerEmployment != null)
                {
                    customer.CustomerEmployment.IndustryId = dto.Employment.IndustryId;
                    customer.CustomerEmployment.CompanyName = dto.Employment.CompanyName;
                    customer.CustomerEmployment.JobTitle = dto.Employment.JobTitle;
                    customer.CustomerEmployment.IsActive = dto.Employment.IsActive;
                }
                else
                {
                    customer.CustomerEmployment = new CustomerEmployment
                    {
                        IndustryId = dto.Employment.IndustryId,
                        CompanyName = dto.Employment.CompanyName,
                        JobTitle = dto.Employment.JobTitle,
                        IsActive = dto.Employment.IsActive
                    };
                }
            }
            else
            {
                customer.CustomerEmployment = null;
            }

            await _customerRepository.SaveChangesAsync();

            // Reload updated customer details
            var updatedCustomer = await _customerRepository.GetCustomerDetailsAsync(id);
            return MapToCustomerDto(updatedCustomer!);
        }

        public async Task<bool> SoftDeleteCustomerAsync(long id)
        {
            var customer = await _customerRepository.GetCustomerDetailsAsync(id);
            if (customer == null)
                return false;

            customer.IsActive = false;
            if (customer.CustomerContact != null)
            {
                customer.CustomerContact.IsActive = false;
            }
            if (customer.CustomerEmployment != null)
            {
                customer.CustomerEmployment.IsActive = false;
            }
            foreach (var doc in customer.CustomerDocuments)
            {
                doc.IsActive = false;
            }

            return await _customerRepository.SaveChangesAsync();
        }



        public async Task<CustomerDocumentDto?> UploadCustomerDocumentAsync(long customerId, string documentType, IFormFile file)
        {
            var customer = await _customerRepository.GetCustomerDetailsAsync(customerId);
            if (customer == null)
                return null;

            // Validate file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("Document file size must not exceed 10MB.");

            // Create Uploads/Documents directory if it doesn't exist
            var docsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "Documents");
            if (!Directory.Exists(docsFolder))
                Directory.CreateDirectory(docsFolder);

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName).ToLower();
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(docsFolder, fileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/Uploads/Documents/{fileName}";

            if (documentType == "ProfileImage")
            {
                foreach (var existingProfileImage in customer.CustomerDocuments
                    .Where(d => d.IsActive && d.DocumentType == "ProfileImage"))
                {
                    existingProfileImage.IsActive = false;
                }
            }

            var document = new CustomerDocument
            {
                CustomerId = customerId,
                DocumentType = documentType,
                FileName = file.FileName,
                FilePath = relativePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadedDate = DateTime.UtcNow,
                IsActive = true
            };

            customer.CustomerDocuments.Add(document);
            await _customerRepository.SaveChangesAsync();

            return new CustomerDocumentDto
            {
                DocumentId = document.DocumentId,
                CustomerId = document.CustomerId,
                DocumentType = document.DocumentType,
                FileName = document.FileName,
                FilePath = document.FilePath,
                ContentType = document.ContentType,
                FileSize = document.FileSize,
                UploadedDate = document.UploadedDate,
                IsActive = document.IsActive
            };
        }

        public async Task<bool> SoftDeleteCustomerDocumentAsync(long customerId, long documentId)
        {
            var customer = await _customerRepository.GetCustomerDetailsAsync(customerId);
            if (customer == null)
                return false;

            var document = customer.CustomerDocuments.FirstOrDefault(d => d.DocumentId == documentId);
            if (document == null || !document.IsActive)
                return false;

            document.IsActive = false;
            return await _customerRepository.SaveChangesAsync();
        }

        private CustomerDto MapToCustomerDto(Customer customer)
        {
            return new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                FullName = customer.FullName,
                CreatedDate = customer.CreatedDate,
                UpdatedDate = customer.UpdatedDate,
                IsActive = customer.IsActive,
                Contact = customer.CustomerContact != null ? new CustomerContactDto
                {
                    ContactId = customer.CustomerContact.ContactId,
                    CustomerId = customer.CustomerContact.CustomerId,
                    Email = customer.CustomerContact.Email,
                    PrimaryPhone = customer.CustomerContact.PrimaryPhone,
                    SecondaryPhone = customer.CustomerContact.SecondaryPhone,
                    Address = customer.CustomerContact.Address,
                    IsActive = customer.CustomerContact.IsActive
                } : null,
                Employment = customer.CustomerEmployment != null ? new CustomerEmploymentDto
                {
                    EmploymentId = customer.CustomerEmployment.EmploymentId,
                    CustomerId = customer.CustomerEmployment.CustomerId,
                    IndustryId = customer.CustomerEmployment.IndustryId,
                    IndustryName = customer.CustomerEmployment.Industry?.IndustryName,
                    CompanyName = customer.CustomerEmployment.CompanyName,
                    JobTitle = customer.CustomerEmployment.JobTitle,
                    IsActive = customer.CustomerEmployment.IsActive
                } : null,
                Documents = customer.CustomerDocuments
                    .OrderByDescending(d => d.UploadedDate)
                    .Select(d => new CustomerDocumentDto
                    {
                        DocumentId = d.DocumentId,
                        CustomerId = d.CustomerId,
                        DocumentType = d.DocumentType,
                        FileName = d.FileName,
                        FilePath = d.FilePath,
                        ContentType = d.ContentType,
                        FileSize = d.FileSize,
                        UploadedDate = d.UploadedDate,
                        IsActive = d.IsActive
                    }).ToList()
            };
        }
    }
}
