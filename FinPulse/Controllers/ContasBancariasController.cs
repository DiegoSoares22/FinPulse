using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinPulse.DTOs;
using FinPulse.Models;
using FinPulse.Repositories;

namespace FinPulse.Controllers;

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Produces("application/json")]
public class ContasBancariasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public ContasBancariasController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém todas as contas bancárias cadastradas.
    /// </summary>
    /// <returns>Lista de contas bancárias em formato ContaBancariaDTO.</returns>
    /// <response code="200">Lista obtida com sucesso.</response>
    /// <response code="401">Usuário não autenticado via JWT.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ContaBancariaDTO>>> Get()
    {
        var contas = await _uof.ContaBancariaRepository.GetAllAsync();
        var contasDto = _mapper.Map<IEnumerable<ContaBancariaDTO>>(contas);
        return Ok(contasDto);
    }

    /// <summary>
    /// Obtém os detalhes de uma conta bancária específica pelo seu identificador ID.
    /// </summary>
    /// <param name="id">ID da conta bancária.</param>
    /// <returns>O objeto ContaBancariaDTO correspondente.</returns>
    /// <response code="200">Conta bancária encontrada com sucesso.</response>
    /// <response code="404">Conta bancária não encontrada para o ID informado.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContaBancariaDTO>> Get(int id)
    {
        var conta = await _uof.ContaBancariaRepository.GetAsync(c => c.ContaBancariaId == id);

        if (conta == null)
            return NotFound($"Conta bancária com id={id} não encontrada.");

        var contaDto = _mapper.Map<ContaBancariaDTO>(conta);
        return Ok(contaDto);
    }

    /// <summary>
    /// Cria uma nova conta bancária ou cartão.
    /// </summary>
    /// <param name="contaCreateDto">Dados para criação da conta.</param>
    /// <returns>A conta bancária recém-criada.</returns>
    /// <response code="201">Conta bancária criada com sucesso.</response>
    /// <response code="400">Dados inválidos informados.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContaBancariaDTO>> Post(ContaBancariaCreateDTO contaCreateDto)
    {
        if (contaCreateDto == null)
            return BadRequest("Dados inválidos.");

        var conta = _mapper.Map<ContaBancaria>(contaCreateDto);
        _uof.ContaBancariaRepository.Create(conta);
        await _uof.CommitAsync();

        var contaDto = _mapper.Map<ContaBancariaDTO>(conta);
        return CreatedAtAction(nameof(Get), new { id = contaDto.ContaBancariaId }, contaDto);
    }

    /// <summary>
    /// Atualiza os dados de uma conta bancária existente.
    /// </summary>
    /// <param name="id">ID da conta a ser atualizada.</param>
    /// <param name="contaUpdateDto">Novos dados da conta bancária.</param>
    /// <response code="204">Atualização realizada com sucesso.</response>
    /// <response code="400">ID informado é inválido ou divergente.</response>
    /// <response code="404">Conta bancária não encontrada.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Put(int id, ContaBancariaCreateDTO contaUpdateDto)
    {
        var contaExistente = await _uof.ContaBancariaRepository.GetAsync(c => c.ContaBancariaId == id);

        if (contaExistente == null)
            return NotFound($"Conta bancária com id={id} não encontrada.");

        var conta = _mapper.Map<ContaBancaria>(contaUpdateDto);
        conta.ContaBancariaId = id;

        _uof.ContaBancariaRepository.Update(conta);
        await _uof.CommitAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove uma conta bancária do sistema pelo seu identificador ID.
    /// </summary>
    /// <param name="id">ID da conta bancária a ser excluída.</param>
    /// <response code="200">Conta excluída com sucesso.</response>
    /// <response code="404">Conta bancária não encontrada.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ContaBancariaDTO>> Delete(int id)
    {
        var conta = await _uof.ContaBancariaRepository.GetAsync(c => c.ContaBancariaId == id);

        if (conta == null)
            return NotFound($"Conta bancária com id={id} não encontrada.");

        _uof.ContaBancariaRepository.Delete(conta);
        await _uof.CommitAsync();

        var contaDto = _mapper.Map<ContaBancariaDTO>(conta);
        return Ok(contaDto);
    }
}