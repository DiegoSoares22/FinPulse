using System.Text.Json;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinPulse.DTOs;
using FinPulse.Models;
using FinPulse.Pagination;
using FinPulse.Repositories;

namespace FinPulse.Controllers;

[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Produces("application/json")]
public class TransacoesController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;

    public TransacoesController(IUnitOfWork uof, IMapper mapper)
    {
        _uof = uof;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtém uma lista paginada e filtrada de transações financeiras (receitas e despesas).
    /// </summary>
    /// <remarks>
    /// Os metadados de paginação (TotalCount, CurrentPage, TotalPages, etc.) são retornados no Header HTTP "X-Pagination".
    /// </remarks>
    /// <param name="parameters">Parâmetros de paginação e filtros (PageNumber, PageSize, Tipo, CategoriaId, etc.).</param>
    /// <returns>Lista de transações no formato TransacaoDTO.</returns>
    /// <response code="200">Lista paginada obtida com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<TransacaoDTO>>> Get([FromQuery] TransacoesParameters parameters)
    {
        var transacoes = await _uof.TransacaoRepository.GetTransacoesPaginadasAsync(parameters);

        var metadata = new
        {
            transacoes.TotalCount,
            transacoes.PageSize,
            transacoes.CurrentPage,
            transacoes.TotalPages,
            transacoes.HasNext,
            transacoes.HasPrevious
        };

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

        var transacoesDto = _mapper.Map<IEnumerable<TransacaoDTO>>(transacoes);
        return Ok(transacoesDto);
    }

    /// <summary>
    /// Obtém os detalhes completos de uma transação específica pelo seu ID.
    /// </summary>
    /// <param name="id">ID da transação.</param>
    /// <returns>O objeto TransacaoDTO com dados da categoria e conta bancária associadas.</returns>
    /// <response code="200">Transação encontrada com sucesso.</response>
    /// <response code="404">Transação não encontrada para o ID informado.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TransacaoDTO>> Get(int id)
    {
        var transacao = await _uof.TransacaoRepository.GetTransacaoComDetalhesAsync(id);

        if (transacao == null)
            return NotFound($"Transação com id={id} não encontrada.");

        var transacaoDto = _mapper.Map<TransacaoDTO>(transacao);
        return Ok(transacaoDto);
    }

    /// <summary>
    /// Registra uma nova transação financeira (Receita ou Despesa).
    /// </summary>
    /// <param name="transacaoCreateDto">Dados da transação a ser criada.</param>
    /// <returns>A transação recém-registrada.</returns>
    /// <response code="201">Transação criada com sucesso.</response>
    /// <response code="400">Dados inválidos (ex: data futura ou conta inexistente).</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TransacaoDTO>> Post(TransacaoCreateDTO transacaoCreateDto)
    {
        if (transacaoCreateDto == null)
            return BadRequest("Dados inválidos.");

        var transacao = _mapper.Map<Transacao>(transacaoCreateDto);
        _uof.TransacaoRepository.Create(transacao);
        await _uof.CommitAsync();

        var transacaoCriada = await _uof.TransacaoRepository.GetTransacaoComDetalhesAsync(transacao.TransacaoId);
        var transacaoDto = _mapper.Map<TransacaoDTO>(transacaoCriada);

        return CreatedAtAction(nameof(Get), new { id = transacaoDto.TransacaoId }, transacaoDto);
    }

    /// <summary>
    /// Atualiza os dados de uma transação existente.
    /// </summary>
    /// <param name="id">ID da transação a ser atualizada.</param>
    /// <param name="transacaoUpdateDto">Novos dados da transação.</param>
    /// <response code="204">Transação atualizada com sucesso.</response>
    /// <response code="400">ID informado é inválido ou divergente.</response>
    /// <response code="404">Transação não encontrada.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Put(int id, TransacaoCreateDTO transacaoUpdateDto)
    {
        var transacaoExistente = await _uof.TransacaoRepository.GetAsync(t => t.TransacaoId == id);

        if (transacaoExistente == null)
            return NotFound($"Transação com id={id} não encontrada.");

        var transacao = _mapper.Map<Transacao>(transacaoUpdateDto);
        transacao.TransacaoId = id;

        _uof.TransacaoRepository.Update(transacao);
        await _uof.CommitAsync();

        return NoContent();
    }

    /// <summary>
    /// Remove uma transação financeira do sistema pelo seu identificador ID.
    /// </summary>
    /// <param name="id">ID da transação a ser excluída.</param>
    /// <response code="200">Transação excluída com sucesso.</response>
    /// <response code="404">Transação não encontrada.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TransacaoDTO>> Delete(int id)
    {
        var transacao = await _uof.TransacaoRepository.GetAsync(t => t.TransacaoId == id);

        if (transacao == null)
            return NotFound($"Transação com id={id} não encontrada.");

        _uof.TransacaoRepository.Delete(transacao);
        await _uof.CommitAsync();

        var transacaoDto = _mapper.Map<TransacaoDTO>(transacao);
        return Ok(transacaoDto);
    }
}