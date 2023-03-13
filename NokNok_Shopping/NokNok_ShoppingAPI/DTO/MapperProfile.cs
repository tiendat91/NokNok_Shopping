using AutoMapper;
using NokNok_ShoppingAPI.DTO;
using NokNok_ShoppingAPI.Models;

namespace PE_PRN231_Sum22B1.DTO
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<Product, ProductDTO>();
        }
    }
}
