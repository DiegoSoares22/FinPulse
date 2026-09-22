using AutoMapper;
using FinPulse.DTOs;
using FinPulse.Models;

namespace FinPulse.Mappings;

public class FinPulseMappingProfile : Profile
{
    public FinPulseMappingProfile()
    {
        // ===== CATEGORIA =====
        // Entidade → DTO (para respostas GET)
        CreateMap<Categoria, CategoriaDTO>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()));

        // DTO → Entidade (para POST e PUT)
        CreateMap<CategoriaCreateDTO, Categoria>();

        // ===== CONTA BANCÁRIA =====
        CreateMap<ContaBancaria, ContaBancariaDTO>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()));

        CreateMap<ContaBancariaCreateDTO, ContaBancaria>();

        // ===== TRANSAÇÃO =====
        CreateMap<Transacao, TransacaoDTO>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
            .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria!.Nome))
            .ForMember(dest => dest.CategoriaCor, opt => opt.MapFrom(src => src.Categoria!.Cor))
            .ForMember(dest => dest.ContaBancariaNome, opt => opt.MapFrom(src => src.ContaBancaria!.Nome));

        CreateMap<TransacaoCreateDTO, Transacao>();
    }
}