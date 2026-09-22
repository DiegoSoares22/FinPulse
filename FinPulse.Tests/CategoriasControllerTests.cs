using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using FinPulse.Controllers;
using FinPulse.DTOs;
using FinPulse.Enums;
using FinPulse.Mappings;
using FinPulse.Models;
using FinPulse.Repositories;
using Xunit;

namespace FinPulse.Tests;

public class CategoriasControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUof;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly CategoriasController _controller;

    public CategoriasControllerTests()
    {
        // 1. Mock do Unit of Work
        _mockUof = new Mock<IUnitOfWork>();

        // 2. Instância real do AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FinPulseMappingProfile());
        });
        _mapper = mapperConfig.CreateMapper();

        // 3. Instância real de MemoryCache para os testes
        _cache = new MemoryCache(new MemoryCacheOptions());

        // 4. Instancia o Controller injetando o mock, mapper e cache
        _controller = new CategoriasController(_mockUof.Object, _mapper, _cache);
    }

    // ========================================================
    // TESTES DO MÉTODO GET
    // ========================================================

    [Fact]
    public async Task GetV1_DeveRetornarOkResult_ComListaDeCategoriasDTO()
    {
        // Arrange
        var categoriasFalsas = new List<Categoria>
        {
            new() { CategoriaId = 1, Nome = "Alimentação", Tipo = TipoTransacao.Despesa },
            new() { CategoriaId = 2, Nome = "Salário", Tipo = TipoTransacao.Receita }
        };

        _mockUof.Setup(u => u.CategoriaRepository.GetAllAsync())
                .ReturnsAsync(categoriasFalsas);

        // Act
        var resultado = await _controller.GetV1();

        // Assert
        var okResult = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categoriasDto = okResult.Value.Should().BeAssignableTo<IEnumerable<CategoriaDTO>>().Subject;

        categoriasDto.Should().HaveCount(2);
        categoriasDto.First().Nome.Should().Be("Alimentação");
        categoriasDto.First().Tipo.Should().Be("Despesa");
    }

    // ========================================================
    // TESTES DO MÉTODO GET POR ID
    // ========================================================

    [Fact]
    public async Task GetPorId_DeveRetornarOkResult_QuandoCategoriaExiste()
    {
        // Arrange
        var categoriaId = 1;
        var categoriaFalsa = new Categoria
        {
            CategoriaId = categoriaId,
            Nome = "Transporte",
            Tipo = TipoTransacao.Despesa
        };

        _mockUof.Setup(u => u.CategoriaRepository.GetAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(categoriaFalsa);

        // Act
        var resultado = await _controller.Get(categoriaId);

        // Assert
        var okResult = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categoriaDto = okResult.Value.Should().BeOfType<CategoriaDTO>().Subject;

        categoriaDto.CategoriaId.Should().Be(categoriaId);
        categoriaDto.Nome.Should().Be("Transporte");
    }

    [Fact]
    public async Task GetPorId_DeveRetornarNotFound_QuandoCategoriaNaoExiste()
    {
        // Arrange
        var categoriaIdInexistente = 999;

        _mockUof.Setup(u => u.CategoriaRepository.GetAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync((Categoria?)null);

        // Act
        var resultado = await _controller.Get(categoriaIdInexistente);

        // Assert
        resultado.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    // ========================================================
    // TESTES DO MÉTODO POST
    // ========================================================

    [Fact]
    public async Task Post_DeveRetornarCreatedAtActionResult_QuandoDadosForemValidos()
    {
        // Arrange
        var novaCategoriaDto = new CategoriaCreateDTO
        {
            Nome = "Educação",
            Tipo = TipoTransacao.Despesa,
            Icone = "graduation-cap",
            Cor = "#3B82F6"
        };

        var categoriaCriada = new Categoria
        {
            CategoriaId = 10,
            Nome = "Educação",
            Tipo = TipoTransacao.Despesa
        };

        _mockUof.Setup(u => u.CategoriaRepository.Create(It.IsAny<Categoria>()))
                .Returns(categoriaCriada);

        _mockUof.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

        // Act
        var resultado = await _controller.Post(novaCategoriaDto);

        // Assert
        var createdResult = resultado.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var dtoRetornado = createdResult.Value.Should().BeOfType<CategoriaDTO>().Subject;

        dtoRetornado.CategoriaId.Should().Be(10);
        dtoRetornado.Nome.Should().Be("Educação");
        _mockUof.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Post_DeveRetornarBadRequest_QuandoObjetoForNulo()
    {
        // Act
        var resultado = await _controller.Post(null!);

        // Assert
        resultado.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ========================================================
    // TESTES DO MÉTODO DELETE
    // ========================================================

    [Fact]
    public async Task Delete_DeveRetornarOkResult_QuandoCategoriaExiste()
    {
        // Arrange
        var categoriaExistente = new Categoria { CategoriaId = 1, Nome = "Lazer" };

        _mockUof.Setup(u => u.CategoriaRepository.GetAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync(categoriaExistente);

        _mockUof.Setup(u => u.CategoriaRepository.Delete(It.IsAny<Categoria>()))
                .Returns(categoriaExistente);

        _mockUof.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

        // Act
        var resultado = await _controller.Delete(1);

        // Assert
        resultado.Result.Should().BeOfType<OkObjectResult>();
        _mockUof.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoCategoriaNaoExiste()
    {
        // Arrange
        _mockUof.Setup(u => u.CategoriaRepository.GetAsync(It.IsAny<Expression<Func<Categoria, bool>>>()))
                .ReturnsAsync((Categoria?)null);

        // Act
        var resultado = await _controller.Delete(999);

        // Assert
        resultado.Result.Should().BeOfType<NotFoundObjectResult>();
        _mockUof.Verify(u => u.CommitAsync(), Times.Never);
    }
}