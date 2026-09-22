using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using FinPulse.DTOs;
using FinPulse.Models;
using FinPulse.Services;

namespace FinPulse.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Cadastra um novo usuário no sistema FinPulse.
    /// </summary>
    /// <param name="model">Dados para registro do usuário.</param>
    /// <returns>Objeto ResponseDTO com o status do cadastro.</returns>
    /// <response code="200">Usuário registrado com sucesso.</response>
    /// <response code="400">Nome de usuário ou dados já existentes/inválidos.</response>
    /// <response code="500">Erro interno ao criar o usuário.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var userExists = await _userManager.FindByNameAsync(model.UserName!);
        if (userExists != null)
            return StatusCode(StatusCodes.Status400BadRequest,
                new ResponseDTO { Status = "Error", Message = "Usuário já existe!" });

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            NomeCompleto = model.NomeCompleto
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDTO { Status = "Error", Message = "Erro ao criar usuário. Verifique os requisitos de senha!" });

        return Ok(new ResponseDTO { Status = "Success", Message = "Usuário criado com sucesso!" });
    }

    /// <summary>
    /// Autentica um usuário e gera o Token JWT com Refresh Token.
    /// </summary>
    /// <param name="model">Credenciais de acesso (usuário e senha).</param>
    /// <returns>Token de acesso JWT, Refresh Token e data de expiração.</returns>
    /// <response code="200">Login realizado com sucesso.</response>
    /// <response code="401">Usuário ou senha incorretos.</response>
    /// <response code="429">Limite de tentativas excedido (Rate Limit: 5 requisições/min).</response>
    [HttpPost("login")]
    [EnableRateLimiting("loginLimit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        return Unauthorized(new ResponseDTO { Status = "Error", Message = "Usuário ou senha inválidos." });
    }

    /// <summary>
    /// Renova um Token JWT expirado utilizando um Refresh Token válido.
    /// </summary>
    /// <param name="tokenModel">Objeto contendo o token expirado e o refresh token.</param>
    /// <returns>Novo Token JWT e novo Refresh Token gerados.</returns>
    /// <response code="200">Tokens renovados com sucesso.</response>
    /// <response code="400">Token de acesso ou refresh token inválidos/expirados.</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
    {
        if (tokenModel is null)
            return BadRequest("Requisição inválida.");

        string? accessToken = tokenModel.AccessToken;
        string? refreshToken = tokenModel.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);
        if (principal == null)
            return BadRequest("Token de acesso ou refresh token inválido.");

        string username = principal.Identity!.Name!;
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return BadRequest("Refresh Token expirado ou inválido.");

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims, _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    /// <summary>
    /// Revoga o Refresh Token de um usuário, forçando-o a fazer login novamente.
    /// </summary>
    /// <param name="username">Nome do usuário a ter o token revogado.</param>
    /// <response code="204">Token revogado com sucesso.</response>
    /// <response code="400">Usuário não encontrado.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [Authorize]
    [HttpPost("revoke/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("Nome de usuário inválido.");

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}