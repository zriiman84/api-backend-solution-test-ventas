using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;
using ApiSolutionTestVentas.Entities;
using ApiSolutionTestVentas.Persistencia;
using ApiSolutionTestVentas.Repositories.Abstracciones;
using ApiSolutionTestVentas.Services.Abstracciones;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiSolutionTestVentas.Services.Implementaciones;

public class UserService : IUserService
{
    private readonly UserManager<SpecificUserIdentity> _userManager;
    private readonly ILogger<UserService> _logger;
    private readonly IOptions<AppSettings> _options;
    private readonly IClienteRepository _clienteRepository;
    private readonly SignInManager<SpecificUserIdentity> _signInManager;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public UserService(UserManager<SpecificUserIdentity> userManager,
        ILogger<UserService> logger,
        IOptions<AppSettings> options,
        IClienteRepository clienteRepository,
        SignInManager<SpecificUserIdentity> signInManager,
        IMapper mapper,
        IEmailService emailService)
    {
        this._userManager = userManager;
        this._logger = logger;
        this._options = options;
        this._clienteRepository = clienteRepository;
        this._signInManager = signInManager;
        this._mapper = mapper;
        this._emailService = emailService;
    }

    public async Task<BaseResponseGeneric<RegisterUserResponseDto>> RegisterAsync(RegisterUserRequestDto request)
    {
        var response = new BaseResponseGeneric<RegisterUserResponseDto>();
        response.Success = false;

        try
        {
            //1. Registrar el usuario
            var user = _mapper.Map<SpecificUserIdentity>(request);
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                response.ErrorMessage =
                    $"Ocurrió un error al registrar el usuario: {String.Join(" ", result.Errors.Select(x => x.Description).ToArray())}";
                _logger.LogError(response.ErrorMessage);
                return response;
            }

            // Obtenemos el usuario registrado
            user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                response.ErrorMessage = "Ocurrió un error al registrar el usuario en la BD.";
                _logger.LogError(response.ErrorMessage);
                return response;
            }
            
            // 2. Asignamos el Rol Customer para el usuario creado
            await _userManager.AddToRoleAsync(user, Constants.RoleCustomer);

            //3. Registrar un nuevo cliente para el usuario
            var cliente = new Cliente
            {
                Nombre = request.FirstName,
                Apellidos = request.LastName,
                Email = request.Email,
            };

            if (await _clienteRepository.AddAsync(cliente) > 0)
            {
                //4. Enviar correo DE CONFIRMACIÓN de creación de usuario

                var templeatesEmail = _options.Value.EmailTemplates;
                var message = templeatesEmail.CreateUserEmail
                    .Replace("{FirstName}", request.FirstName)
                    .Replace("{LastName}", request.LastName);

                await _emailService.SendEmailAsync(request.Email, templeatesEmail.SubjectCreateUser, message);

                var tokenResponse = await CrearToken(user); //returning jwt
                response.Data = new RegisterUserResponseDto
                {
                    UserId = user.Id,
                    Token = tokenResponse.Token,
                    ExpirationDateTime = tokenResponse.ExpirationDateTime
                };
                response.Success = true;
                response.Message =
                    $"Se creó correctamente el nuevo usuario con email {request.Email} y usuario {user.UserName}...";
                _logger.LogInformation(response.Message);
            }
            else
            {
                response.ErrorMessage = "El usuario fue registrado exitosamente pero ocurrió un error al registrar el cliente en la BD.";
                _logger.LogError(response.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Ocurrió un error inesperado al registrar el usuario.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    //Validar que permita loguearse por username o email.
    //Desde el FrontEnd se enviará el usuario o el email (el frontend obliga que al registrarse sean valores diferentes)
    public async Task<BaseResponseGeneric<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var response = new BaseResponseGeneric<LoginResponseDto>();
        
        try
        {
            string usuarioIngresado = request.Username;
            
            //Validar si se envió usuario o email
             var usuario = await _userManager.FindByEmailAsync(usuarioIngresado);

             if (usuario is not null)
             {
                 usuarioIngresado = usuario.UserName ?? string.Empty;
             }
             
            var result = await _signInManager.PasswordSignInAsync(usuarioIngresado, request.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(usuarioIngresado);

                 response.Data = await CrearToken(user);
                 response.Success = true;
                 response.Message = $"Usuario {user.UserName} inició sesión correctamente...";
                 _logger.LogInformation(response.Message);
              
            }
            else
            {
                response.Success = false;
                response.Message = $"El usuario '{request.Username}' no existe o las credenciales no son correctas...";
                _logger.LogWarning(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Ocurrió un error en el LOGIN";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    private async Task<LoginResponseDto> CrearToken(SpecificUserIdentity user)
    {
        //creamos los claims, que son informaciones emitidas por una fuente confiable, pueden contener cualquier key/value que definamos y que son añadidas al TOKEN
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, user.Email ?? String.Empty),
            new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}")
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        //firmando el JWT
        var llave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.Value.Jwt
                .JWTKey)); //Generamos la llave codificada en UTF8, para generar la firma. Obtenida desde appsettings.Development.json
        var credenciales =
            new SigningCredentials(llave,
                SecurityAlgorithms.HmacSha256); //Indicaciones para la firma
        var expiracion =
            DateTime.UtcNow.AddSeconds(_options.Value.Jwt
                .LifetimeInSeconds); //Obtener la fecha y hora final en UTC en que expirará el token, basado en el tiempo en segundos configurado

        // Agregar la fecha y hora de expiración del tokem en el claim, en UTC
        claims.Add(new Claim(ClaimTypes.Expiration,
            expiracion.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)));
        
        //Agregar el username de usuario al token en el claim
        claims.Add(new Claim(ClaimTypes.Name, user.UserName?? String.Empty ));
        

        //Crear el token
        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
            signingCredentials: credenciales, expires: expiracion);

        var responseDto = new LoginResponseDto();
        responseDto.Token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        responseDto.ExpirationDateTime = expiracion.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        return responseDto;
    }

    //Recibirá como entrada una cuenta de correo y enviará un token por correo a dicha cuenta
    public async Task<BaseResponse> RequestTokenToResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var response = new BaseResponse();

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is not null)
            {
                //Generar token temporal para el reseteo de contraseña
                var tokenReset = await _userManager.GeneratePasswordResetTokenAsync(user);

                //Enviar correo
                var emailTemplate = _options.Value.EmailTemplates;
                var message = emailTemplate.ResetPasswordEmail
                    .Replace("{FirstName}", user.FirstName)
                    .Replace("{LastName}", user.LastName)
                    .Replace("{Token}", tokenReset);

                await _emailService.SendEmailAsync(request.Email, emailTemplate.SubjectResetPassword, message);

                response.Success = true;
                response.Message = $"Se ha enviado el token temporal al correo '{request.Email}'..";
                _logger.LogInformation(response.Message);
            }
            else
            {
                response.Success = false;
                response.Message = $"El usuario con cuenta de correo '{request.Email}' no está registrado...";
                _logger.LogWarning(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Ocurrió un error en la generación del Token o en el envió de correo.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }


    //Recibirá como entrada una cuenta de correo, el token, una nueva contraseña nueva para resetear su clave.
    public async Task<BaseResponse> ResetPasswordAsync(NewPasswordResetRequestDto request)
    {
        var response = new BaseResponse();

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is not null)
            {
                //Realizar el reseteo de la contraseña
                var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (result.Succeeded)
                {
                    //Enviar el correo con la confirmación del reseteo
                    var emailTemplate = _options.Value.EmailTemplates;
                    var message = emailTemplate.ChangePasswordEmail
                        .Replace("{FirstName}", user.FirstName)
                        .Replace("{LastName}", user.LastName);

                    await _emailService.SendEmailAsync(request.Email, emailTemplate.SubjectChangePassword, message);

                    response.Success = true;
                    response.Message =
                        $"Se ha enviado un correo a '{request.Email}' confirmando el reseteo de su clave..";
                    _logger.LogInformation(response.Message);
                }
                else
                {
                    throw new Exception(
                        $"Ocurrió un error al resetear la clave: {string.Join(",", result.Errors.Select(p => p.Description).ToArray())}");
                }
            }
            else
            {
                response.Success = false;
                response.Message = $"El usuario con cuenta de correo '{request.Email}' no está registrado...";
                _logger.LogWarning(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Ocurrió un error al resetear la clave por la nueva.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }

    //Recibirá como entrada la clave anterior ya la nueva y realizará el cambio de contraseña.
    //Aquí el usuario está logueado y tiene un token.
    public async Task<BaseResponse> ChangePasswordAsync(string email, ChangePasswordRequestDto request)
    {
        var response = new BaseResponse();

        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                //Realizar el reseteo de la contraseña
                var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

                if (result.Succeeded)
                {
                    //Enviar el correo con la confirmación del cambio de clave
                    var emailTemplate = _options.Value.EmailTemplates;
                    var message = emailTemplate.ChangePasswordEmail
                        .Replace("{FirstName}", user.FirstName)
                        .Replace("{LastName}", user.LastName);

                    await _emailService.SendEmailAsync(email, emailTemplate.SubjectChangePassword, message);

                    response.Success = true;
                    response.Message = $"Se ha enviado un correo a '{email}' confirmando el cambio de su clave..";
                    _logger.LogInformation(response.Message);
                }
                else
                {
                    throw new Exception(
                        $"Ocurrió un error al cambiar la clave: {string.Join(",", result.Errors.Select(p => p.Description).ToArray())}");
                }
            }
            else
            {
                response.Success = false;
                response.Message = $"El usuario con cuenta de correo '{email}' no está registrado...";
                _logger.LogWarning(response.Message);
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = "Ocurrió un error al cambiar la clave por la nueva.";
            _logger.LogError(ex, $"{response.ErrorMessage} - Mensaje: {ex.Message}");
        }

        return response;
    }
}