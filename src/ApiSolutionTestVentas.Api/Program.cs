using ApiSolutionTestVentas.Api.Filter;
using ApiSolutionTestVentas.Api.MinimalApi;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Persistencia.Seeders;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Repositories.Implementaciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using ApiSolutionTestVentas.Services.Implementaciones;
using ApiSolutionTestVentas.Services.Profiles;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


//Configuraci�n del log
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logPath,
        rollingInterval: RollingInterval.Day, //Indicamos que se cree un nuevo archivo cada d�a
        restrictedToMinimumLevel: LogEventLevel.Information) //M�nimo nivel de restricci�n ser� Information hacia arriba
    .CreateLogger();


try
{
    //Matriculo el log
    builder.Logging.AddSerilog(logger);
    //Agregando una l�nea inicial en el LOG indicando en qu� ambiente nos encontramos.
    logger.Information($"LOG INITIALIZED in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NO ENV"}");

    // ******************************************************
    // APLICAR ESTA CONFIGURACI�N (para . decinal)
    // 1. Crear una cultura
    var cultureInfo = CultureInfo.InvariantCulture;

    // 2. Establecer esta cultura como la predeterminada para el hilo actual y la interfaz de usuario.
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    // ******************************************************

    //Definir una pol�tica de CORS
    var corsConfiguration = "ApiSolucionTestVentasCors";
    builder.Services.AddCors(setup =>
    {
        setup.AddPolicy(corsConfiguration, policy =>
        {
            //policy.WithOrigins("https://app.miempresa.com") // dominio del frontend para darle permisos a un dominio espec�fico
            policy.AllowAnyOrigin(); // Que cualquier dominio puede consumir el API
            policy.AllowAnyHeader().WithExposedHeaders(new string[] { "TotalRegistros" });
            policy.AllowAnyMethod();
        });
    });

    //Options pattern for AppSettings
    builder.Services.Configure<AppSettings>(builder.Configuration);

    //Configurar swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    //builder.Services.AddControllers();
    builder.Services.AddControllers(options => { options.Filters.Add(typeof(FilterExceptions)); });

    //Estandarizar las repuestas de los ENDPOINTS en caso de Errores, basado en ApiBehaviorOptions
    //ApiBehaviorOptions: Es la clase de opciones que define c�mo deben comportarse los controllers de API.
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new BaseResponse
            {
                Success = false,
                Message = String.Empty,
                ErrorMessage = string.Join("; ", errors.ToArray()) // Une los mensajes de error en un solo string.
            };

            return new BadRequestObjectResult(response);
        };
    });


    // Configuring Context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    //Identity
    //Configura pol�ticas para la gesti�n de usuarios, roles, contrase�as y claims.
    builder.Services.AddIdentity<SpecificUserIdentity, IdentityRole>(policies =>
    {
        policies.Password.RequireDigit = true; //Las contrase�as deben contener al menos un d�gito num�rico (0-9).
        policies.Password.RequiredLength = 6; //M�nimo de 6 d�gitos
        policies.User.RequireUniqueEmail =
            true; //Cada usuario registrado debe tener un correo �nico. Si ingresa un correo existente arrojar� error.
    })
        .AddEntityFrameworkStores<
            ApplicationDbContext>() //Indica a Identity que debe usar EF Core para almacenar los datos en la BD definida por tu ApplicationDbContext
        .AddDefaultTokenProviders();

    //Autenticaci�n
    //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTKey"] ??
                                         throw new InvalidOperationException("No hay llave JWT configurada"));
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

    //Registering healthchecks: 2 HealthChecks --> pueden ser varios
    builder.Services.AddHealthChecks()
        .AddCheck("ApiSolutionTestVentascheck", () => HealthCheckResult.Healthy()) // valida que mi aplicaci�n est� ok
        .AddDbContextCheck<ApplicationDbContext>(); // valida que mi BD est� ok

    //Authorization
    builder.Services.AddAuthorization();

    //inyecci�n de dependencias
    //HttpContext
    builder.Services.AddHttpContextAccessor();

    //Repositorios
    builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
    builder.Services.AddScoped<IPuestoRepository, PuestoRepository>();
    builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
    builder.Services.AddScoped<ICategoriaProductoRepository, CategoriaProductoRepository>();
    builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
    builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
    builder.Services.AddScoped<IVentaRepository, VentaRepository>();

    //Services
    builder.Services.AddScoped<IPuestoService, PuestoService>();
    builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
    builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
    builder.Services.AddScoped<ICategoriaProductoService, CategoriaProductoService>();
    builder.Services.AddScoped<IProductoService, ProductoService>();
    builder.Services.AddScoped<IClienteService, ClienteService>();
    builder.Services.AddScoped<IVentaService, VentaService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IFileStorage, FileStorageAzure>();
    //builder.Services.AddScoped<IFileStorage, FileStorageLocal>();


    //DataSeeders
    builder.Services.AddScoped<UserDataSeeder>();
    builder.Services.AddScoped<DataSeederInicial>();

    //Registro de los mapper / profiles
    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<PuestoProfile>();
        config.AddProfile<DepartamentoProfile>();
        config.AddProfile<EmpleadoProfile>();
        config.AddProfile<ClienteProfile>();
        config.AddProfile<CategoriaProductoProfile>();
        config.AddProfile<ProductoProfile>();
        config.AddProfile<VentaProfile>();
        config.AddProfile<UserIdentityProfile>();
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        app.UseSwagger();
        //app.UseSwaggerUI();
        //Agregado para evitar problemas de pueerto cuando trabajmos con AKS (Kuberntes en Azure) y swagger
        app.UseSwaggerUI(c =>
        {
            // Esto obliga a Swagger a buscar el JSON usando rutas relativas 
            // sin importar si viene del puerto 80, 8080 o un proxy de Azure.
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SalesStore API V1");
        
            // Si quieres que cargue directo en la raíz de la IP:8080 sin poner /swagger
            // c.RoutePrefix = string.Empty; 
        });
    }

    app.UseHttpsRedirection();
    //app.UseStaticFiles(); //Para poder ver las im�genes almacenadas en wwwroot/products
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors(corsConfiguration); //Usar la pol�tica de CORS antes del mapeo de endpoints
    app.MapHome(); //Minimal API creado para el Home
    app.MapReporteVentasCliente(); //Minimal API creado para el Reporte de Venta por Cliente
    app.MapReporteVentasProducto(); //Minimal API creado para wl Reporte de Venta por Producto
    app.MapControllers(); //Mapeo de los controladores

    // Aplicar migraciones y sembrar datos (as�ncronamente)
    await ApplyMigrationsAndSeedDataAsync(app);

    //Configuring health checks
    //Minimal API - EndPoint para obtener health checks. http://dominio:puerto/healthcheck
    app.MapHealthChecks("/healthcheck", new()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Un error no controlado ha sucedido durante el API initialization.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}


//Ejecutar migraciones pendiengtes y DataSeeders
static async Task ApplyMigrationsAndSeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    //Aplicar las migraciones autom�ticas
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        //MigrateAsync
        //1. Verifica si existe la BD referenciada en ConnectionString. Si no existe, la crea.
        //2. Ejecuta migrations para crear y/o actualizar los objetos de BD
        await dbContext.Database.MigrateAsync();
    }

    //Aplicar los DataSeeders
    var userDataSeeder = scope.ServiceProvider.GetRequiredService<UserDataSeeder>();
    await userDataSeeder.SeedAsync();

    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeederInicial>();
    await dataSeeder.SeedAsync();
}