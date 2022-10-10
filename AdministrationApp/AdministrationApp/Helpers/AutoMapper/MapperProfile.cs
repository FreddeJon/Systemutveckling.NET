using AdministrationApp.MVVM.Models;
using Core.Models;

namespace AdministrationApp.Helpers.AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<DeviceItem, DeviceModel>().ReverseMap();
    }
}