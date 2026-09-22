using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // <-- Using do Cache
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
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache; // 1. Injeção do Cache
    private const string CategoriasCacheKey = "CategoriasCacheKey"; // Chave única do Cache

    public CategoriasController(IUnitOfWork uof, IMapper mapper, IMemoryCache cache)
    {
        _uof = uof;
        _mapper = mapper;
        _cache = cache;
    }

    /// <summary>
    /// Obtém a lista de categorias utilizando Cache em Memória (v1.0).
    /// </summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetV1()
    {
        // 1. Tenta buscar do Cache em Memória RAM
        if (!_cache.TryGetValue(CategoriasCacheKey, out IEnumerable<CategoriaDTO>? categoriasDto))
        {
            // Cache Miss: Não estava na memória, busca do banco de dados SQL Server
            var categorias = await _uof.CategoriaRepository.GetAllAsync();
            categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            // Configura as opções de expiração do Cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)) // Renova se for consultado a cada 5 min
                .SetAbsoluteExpiration(TimeSpan.FromHours(1))  // Expira definitivamente em 1 hora
                .SetPriority(CacheItemPriority.Normal);

            // Salva na memória RAM
            _cache.Set(CategoriasCacheKey, categoriasDto, cacheEntryOptions);
        }

        return Ok(categoriasDto);
    }

    /// <summary>
    /// Obtém a lista de categorias ordenadas alfabeticamente (v2.0).
    /// </summary>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetV2()
    {
        var categorias = await _uof.CategoriaRepository.GetAllAsync();
        var categoriasOrdenadas = categorias.OrderBy(c => c.Nome);
        var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categoriasOrdenadas);
        return Ok(categoriasDto);
    }

    /// <summary>
    /// Obtém uma categoria específica por ID (com cache individual).
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        string cacheKey = $"Categoria_{id}";

        if (!_cache.TryGetValue(cacheKey, out CategoriaDTO? categoriaDto))
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria == null)
                return NotFound($"Categoria com id={id} não encontrada.");

            categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(cacheKey, categoriaDto, cacheOptions);
        }

        return Ok(categoriaDto);
    }

    /// <summary>
    /// Cria uma nova categoria e invalida o cache.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaCreateDTO categoriaCreateDto)
    {
        if (categoriaCreateDto == null)
            return BadRequest("Dados inválidos.");

        var categoria = _mapper.Map<Categoria>(categoriaCreateDto);
        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();

        // ⚡ INVALIDAÇÃO DE CACHE: Limpa a lista em memória para que o novo item apareça nas próximas consultas!
        _cache.Remove(CategoriasCacheKey);

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoriaCriada);
        return CreatedAtAction(nameof(Get), new { id = categoriaDto.CategoriaId }, categoriaDto);
    }

    /// <summary>
    /// Atualiza uma categoria e invalida os caches relacionados.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Put(int id, CategoriaCreateDTO categoriaUpdateDto)
    {
        var categoriaExistente = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoriaExistente == null)
            return NotFound($"Categoria com id={id} não encontrada.");

        var categoria = _mapper.Map<Categoria>(categoriaUpdateDto);
        categoria.CategoriaId = id;

        _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();

        // ⚡ INVALIDAÇÃO DE CACHE: Limpa a lista geral e o item individual
        _cache.Remove(CategoriasCacheKey);
        _cache.Remove($"Categoria_{id}");

        return NoContent();
    }

    /// <summary>
    /// Exclui uma categoria e limpa o cache.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria == null)
            return NotFound($"Categoria com id={id} não encontrada.");

        _uof.CategoriaRepository.Delete(categoria);
        await _uof.CommitAsync();

        // ⚡ INVALIDAÇÃO DE CACHE:
        _cache.Remove(CategoriasCacheKey);
        _cache.Remove($"Categoria_{id}");

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
        return Ok(categoriaDto);
    }
}