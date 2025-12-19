using ApiSolutionTestVentas.Dto.Request;
using ApiSolutionTestVentas.Dto.Response;

namespace ApiSolutionTestVentas.Services.Abstracciones;

public interface IUserService
{
    Task<BaseResponseGeneric<RegisterUserResponseDto>> RegisterAsync(RegisterUserRequestDto request);
    
    //Recibirá un username (email o user) y password y retornará el token y el expirationdate
    Task<BaseResponseGeneric<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    
    //Recibirá como entrada una cuenta de correo y enviará un token por correo a dicha cuenta
    Task<BaseResponse> RequestTokenToResetPasswordAsync(ResetPasswordRequestDto request);
    
    //Recibirá como entrada una cuenta de correo, el token, una contraseña nueva para resetear la contraseña.
    Task<BaseResponse> ResetPasswordAsync(NewPasswordResetRequestDto request);

    //Recibirá como entrada la nueva y antigua contraseña para realizar el cambio de password.
    Task<BaseResponse> ChangePasswordAsync(string email, ChangePasswordRequestDto request);
}