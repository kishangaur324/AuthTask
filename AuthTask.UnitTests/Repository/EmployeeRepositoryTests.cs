using AuthTask.Domain.Entities;
using AuthTask.Infrastructure.Data;
using AuthTask.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthTask.UnitTests.Repository
{
    public class EmployeeRepositoryTests
    {
        // Happy path scenarios

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyEmployeeRoleUsers_WhenPaginationIsValid()
        {
            await using var context = CreateContext();
            await SeedEmployeesWithRolesAsync(context);
            var repository = new EmployeeRepository(context);

            var (employees, totalCount) = await repository.GetAllAsync(
                skip: 0,
                take: 10,
                search: null,
                cancellationToken: CancellationToken.None
            );

            Assert.Equal(2, totalCount);
            Assert.Equal(2, employees.Count);
            Assert.DoesNotContain(employees, x => x.Email == "kishan.singh+admin@unthinkable.co");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEmployee_WhenEmployeeExists()
        {
            await using var context = CreateContext();
            var employee = CreateEmployee(Guid.NewGuid(), "user-1", "kishan.singh@unthinkable.co");
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            var repository = new EmployeeRepository(context);

            var result = await repository.GetByIdAsync(employee.Id, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(employee.Id, result!.Id);
            Assert.Equal(employee.Email, result.Email);
        }

        [Fact]
        public async Task AddAsync_SavesEmployee_WhenEntityIsValid()
        {
            await using var context = CreateContext();
            var repository = new EmployeeRepository(context);
            var employee = CreateEmployee(Guid.NewGuid(), "user-1", "kishan.singh@unthinkable.co");

            var result = await repository.AddAsync(employee, CancellationToken.None);
            await context.SaveChangesAsync();

            Assert.Equal(employee, result);
            Assert.Single(context.Employees);
            Assert.Equal(employee.Id, context.Employees.Single().Id);
        }

        [Fact]
        public async Task UpdateAsync_SavesChanges_WhenEmployeeExists()
        {
            await using var context = CreateContext();
            var employee = CreateEmployee(Guid.NewGuid(), "user-1", "kishan.singh@unthinkable.co");
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            var repository = new EmployeeRepository(context);

            employee.FirstName = "Updated";
            var result = await repository.UpdateAsync(employee);
            await context.SaveChangesAsync();

            Assert.Equal(employee, result);
            var persisted = await context.Employees.SingleAsync(x => x.Id == employee.Id);
            Assert.Equal("Updated", persisted.FirstName);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEmployee_WhenEmployeeExists()
        {
            await using var context = CreateContext();
            var employee = CreateEmployee(Guid.NewGuid(), "user-1", "kishan.singh@unthinkable.co");
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            var repository = new EmployeeRepository(context);

            await repository.DeleteAsync(employee.Id);
            await context.SaveChangesAsync();

            Assert.Empty(context.Employees);
        }

        // Edge case scenarios

        [Fact]
        public async Task GetAllAsync_AppliesSearchAndPaging_WhenCriteriaProvided()
        {
            await using var context = CreateContext();
            await SeedEmployeesWithRolesAsync(context);
            var repository = new EmployeeRepository(context);

            var (employees, totalCount) = await repository.GetAllAsync(
                skip: 0,
                take: 1,
                search: "+1",
                cancellationToken: CancellationToken.None
            );

            Assert.Equal(1, totalCount);
            Assert.Single(employees);
            Assert.Equal("kishan.singh+1@unthinkable.co", employees[0].Email);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenSearchDoesNotMatch()
        {
            await using var context = CreateContext();
            await SeedEmployeesWithRolesAsync(context);
            var repository = new EmployeeRepository(context);

            var (employees, totalCount) = await repository.GetAllAsync(
                skip: 0,
                take: 10,
                search: "does-not-exist",
                cancellationToken: CancellationToken.None
            );

            Assert.Empty(employees);
            Assert.Equal(0, totalCount);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenEmployeeDoesNotExist()
        {
            await using var context = CreateContext();
            var repository = new EmployeeRepository(context);

            var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenEmployeeDoesNotExist()
        {
            await using var context = CreateContext();
            var repository = new EmployeeRepository(context);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repository.DeleteAsync(Guid.NewGuid())
            );
        }

        private static AuthDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase($"auth-task-tests-{Guid.NewGuid()}")
                .Options;

            return new AuthDbContext(options);
        }

        private static async Task SeedEmployeesWithRolesAsync(AuthDbContext context)
        {
            var employeeRole = new IdentityRole
            {
                Id = "role-employee",
                Name = "employee",
                NormalizedName = "EMPLOYEE",
            };
            var adminRole = new IdentityRole
            {
                Id = "role-admin",
                Name = "admin",
                NormalizedName = "ADMIN",
            };
            var employeeUser1 = new User
            {
                Id = "user-1",
                UserName = "kishan1",
                NormalizedUserName = "KISHAN1",
                Email = "kishan.singh+1@unthinkable.co",
                NormalizedEmail = "KISHAN.SINGH+ALICE@UNTHINKABLE.CO",
            };
            var employeeUser2 = new User
            {
                Id = "user-2",
                UserName = "kishan2",
                NormalizedUserName = "kishan2",
                Email = "kishan.singh+2@unthinkable.co",
                NormalizedEmail = "KISHAN.SINGH+BOB@UNTHINKABLE.CO",
            };
            var adminUser = new User
            {
                Id = "user-admin",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "kishan.singh+admin@unthinkable.co",
                NormalizedEmail = "KISHAN.SINGH+ADMIN@UNTHINKABLE.CO",
            };

            context.Roles.AddRange(employeeRole, adminRole);
            context.Users.AddRange(employeeUser1, employeeUser2, adminUser);
            context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    UserId = employeeUser1.Id,
                    RoleId = employeeRole.Id,
                },
                new IdentityUserRole<string>
                {
                    UserId = employeeUser2.Id,
                    RoleId = employeeRole.Id,
                },
                new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = adminRole.Id }
            );
            context.Employees.AddRange(
                CreateEmployee(Guid.NewGuid(), employeeUser1.Id, employeeUser1.Email!),
                CreateEmployee(Guid.NewGuid(), employeeUser2.Id, employeeUser2.Email!),
                CreateEmployee(Guid.NewGuid(), adminUser.Id, adminUser.Email!)
            );

            await context.SaveChangesAsync();
        }

        private static Employee CreateEmployee(Guid id, string userId, string email)
        {
            return new Employee
            {
                Id = id,
                UserId = userId,
                Email = email,
                EmployeeCode = "EMP-001",
                FirstName = "Kishan",
                LastName = "Singh",
                PhoneNumber = "+919999999999",
                DepartmentId = Guid.NewGuid(),
                ManagerId = Guid.NewGuid(),
                DateOfJoining = DateTime.UtcNow.AddMonths(-6),
                DateOfLeaving = null,
                IsActive = true,
            };
        }
    }
}
