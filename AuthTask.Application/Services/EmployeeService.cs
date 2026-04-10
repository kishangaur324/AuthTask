using System.Security.Claims;
using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuthTask.Application.Services
{
    /// <summary>
    /// Implements employee-related business logic.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<CreateEmployeeDto> _createEmployeeValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="employeeRepository">Employee repository.</param>
        /// <param name="unitOfWork">Unit-of-work abstraction.</param>
        /// <param name="mapper">Object mapper.</param>
        /// <param name="logger">Logger instance.</param>
        /// <param name="httpContextAccessor">HTTP context accessor.</param>
        /// <param name="createEmployeeValidator">Create employee validator.</param>
        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<EmployeeService> logger,
            IHttpContextAccessor httpContextAccessor,
            IValidator<CreateEmployeeDto> createEmployeeValidator
        )
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _createEmployeeValidator = createEmployeeValidator;
        }

        /// <inheritdoc />
        public async Task<Result<Guid>> AddAsync(
            CreateEmployeeDto createEmployee,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Creating employee {Email}", createEmployee.Email);

            var validationResult = await _createEmployeeValidator.ValidateAsync(
                createEmployee,
                cancellationToken
            );
            if (!validationResult.IsValid)
            {
                var error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Create employee request is invalid: {error}", error);
                return Result<Guid>.Failure(error);
            }

            var employee = _mapper.Map<Employee>(createEmployee);
            await _employeeRepository.AddAsync(employee, cancellationToken);
            return Result<Guid>.Success(employee.Id);
        }

        /// <inheritdoc />
        public async Task<Result<string>> UpdateAsync(
            UpdateEmployeeDto updateEmployee,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Updating employee {Email}", updateEmployee.Email);
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(
                    updateEmployee.Id,
                    cancellationToken
                );
                if (employee is null)
                    return Result<string>.NotFound("Employee not found");

                employee!.FirstName = updateEmployee.FirstName;
                employee!.LastName = updateEmployee.LastName;
                employee!.PhoneNumber = updateEmployee.PhoneNumber;
                employee!.Email = updateEmployee.Email;
                employee!.EmployeeCode = updateEmployee.EmployeeCode;
                employee!.IsActive = updateEmployee.IsActive;
                employee!.DepartmentId = updateEmployee.DepartmentId;
                employee!.ManagerId = updateEmployee.ManagerId;
                employee!.DateOfJoining = updateEmployee.DateOfJoining;
                employee!.DateOfLeaving = updateEmployee.DateOfLeaving;

                await _employeeRepository.UpdateAsync(employee);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to update the employee {id} data. {ex}",
                    updateEmployee.Id,
                    ex
                );
                return Result<string>.Failure("Failed to update the employee data.");
            }
        }

        /// <inheritdoc />
        public async Task<Result<string>> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Deleting employee {id}", id);
            try
            {
                await _employeeRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<string>.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete the employee {id} data. {ex}", id, ex);
                return Result<string>.Failure("Failed to delete the employee data.");
            }
        }

        /// <inheritdoc />
        public async Task<Result<EmployeeDto>> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Getting employee {id}", id);
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken);
                var currentUserId = _httpContextAccessor
                    .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                    ?.Value;
                var isAdmin = _httpContextAccessor.HttpContext.User.IsInRole("admin");

                // Users can fetch their own record; admins can fetch any record.
                return isAdmin || (employee != null && employee.UserId == currentUserId)
                        ? Result<EmployeeDto>.Success(_mapper.Map<EmployeeDto>(employee))
                    : employee == null ? Result<EmployeeDto>.NotFound("User not found.")
                    : Result<EmployeeDto>.Unauthorized("User is unauthorized.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to fetch the employee {id} data. {ex}", id, ex);
                return Result<EmployeeDto>.Failure("Failed to fetch the employee data.");
            }
        }

        /// <inheritdoc />
        public async Task<Result<PaginationResponse<EmployeeDto>>> GetAllAsync(
            PaginationRequest pagination,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Getting employees {PageNumber}, {PageSize}, {Search}",
                pagination.PageNumber,
                pagination.PageSize,
                pagination.Search
            );
            try
            {
                var (employees, totalCount) = await _employeeRepository.GetAllAsync(
                    pagination.Skip,
                    pagination.PageSize,
                    pagination.Search,
                    cancellationToken
                );

                var response = new PaginationResponse<EmployeeDto>
                {
                    TotalCount = totalCount,
                    Items = _mapper.Map<List<EmployeeDto>>(employees),
                };

                return Result<PaginationResponse<EmployeeDto>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to fetch the employees data. {ex}", ex);
                return Result<PaginationResponse<EmployeeDto>>.Failure(
                    "Failed to fetch the employees data."
                );
            }
        }
    }
}
