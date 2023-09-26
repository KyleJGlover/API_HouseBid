using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.RequestHelpers;

public class MapperProfiles : Profile
{
    private readonly IMapper _mapper;

    public MapperProfiles(IMapper mapper)
    {
        _mapper = mapper;
    }
    public MapperProfiles() 
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }
}
