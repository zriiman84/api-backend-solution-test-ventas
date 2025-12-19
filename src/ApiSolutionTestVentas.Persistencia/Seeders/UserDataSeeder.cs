using ApiSolutionTestVentas.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ApiSolutionTestVentas.Persistencia.Seeders;

public class UserDataSeeder
{
    private readonly IServiceProvider _service;

    public UserDataSeeder(IServiceProvider service)
    {
        this._service = service;
    }
    public async Task SeedAsync()
    {
        //User repository
        var userManager = _service.GetRequiredService<UserManager<SpecificUserIdentity>>();
        //Role repository
        var roleManager = _service.GetRequiredService<RoleManager<IdentityRole>>();
        //Creating roles
        var rolAdmin = new IdentityRole(Constants.RoleAdmin);
        var rolCliente = new IdentityRole(Constants.RoleCustomer);
        var rolEmpleado = new IdentityRole(Constants.RoleEmployee);

        if (!await roleManager.RoleExistsAsync(Constants.RoleAdmin))
            await roleManager.CreateAsync(rolAdmin);

        if (!await roleManager.RoleExistsAsync(Constants.RoleCustomer))
            await roleManager.CreateAsync(rolCliente);
        
        if (!await roleManager.RoleExistsAsync(Constants.RoleEmployee))
            await roleManager.CreateAsync(rolEmpleado);

        //Admin user
        var adminUser = new SpecificUserIdentity()
        {
            FirstName = "System",
            LastName = "Administrator",
            UserName = "admin_user",
            Email = "admin_user@mailinator.com",
            PhoneNumber = "123456789",
            Age = 32,
            DocumentType = DocumentTypeEnum.Dni,
            DocumentNumber = "00112233",
            EmailConfirmed = true
        };
        
        //Customer user
        var customerUser = new SpecificUserIdentity()
        {
            FirstName = "Customer",
            LastName = "Customer",
            UserName = "customer_user",
            Email = "customer_user@mailinator.com",
            PhoneNumber = "015672233",
            Age = 35,
            DocumentType = DocumentTypeEnum.Dni,
            DocumentNumber = "42011332",
            EmailConfirmed = true
        };
        
        //Employee user
        var employeeUser = new SpecificUserIdentity()
        {
            FirstName = "Employee",
            LastName = "Employee",
            UserName = "employee_user",
            Email = "employee_user@mailinator.com",
            PhoneNumber = "015680000",
            Age = 38,
            DocumentType = DocumentTypeEnum.Dni,
            DocumentNumber = "42211330",
            EmailConfirmed = true
        };
        
        if (await userManager.FindByEmailAsync("admin_user@mailinator.com") is null)
        {
            var result = await userManager.CreateAsync(adminUser, "Admin1234*");
            if (result.Succeeded)
            {
                // Obtenemos el registro del usuario
                adminUser = await userManager.FindByEmailAsync(adminUser.Email);
                // Aqui agregamos el Rol de Administrador para el usuario "admin_user"
                if (adminUser is not null)
                    await userManager.AddToRoleAsync(adminUser, Constants.RoleAdmin);
            }
        }
        
        if (await userManager.FindByEmailAsync("customer_user@mailinator.com") is null)
        {
            var result = await userManager.CreateAsync(customerUser, "Customer.2025*!");
            if (result.Succeeded)
            {
                // Obtenemos el registro del usuario
                customerUser = await userManager.FindByEmailAsync(customerUser.Email);
                // Aqui agregamos el Rol de Customer para el usuario "customer_user"
                if (customerUser is not null)
                    await userManager.AddToRoleAsync(customerUser, Constants.RoleCustomer);
            }
        }
        
        if (await userManager.FindByEmailAsync("employee_user@mailinator.com") is null)
        {
            var result = await userManager.CreateAsync(employeeUser, "Employee.2025**");
            if (result.Succeeded)
            {
                // Obtenemos el registro del usuario
                employeeUser = await userManager.FindByEmailAsync(employeeUser.Email);
                // Aqui agregamos el Rol de Employee para el usuario "employee_user"
                if (employeeUser is not null)
                    await userManager.AddToRoleAsync(employeeUser, Constants.RoleEmployee);
            }
        }
    }
}