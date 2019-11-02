using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using vroom.Controllers.Resources;
using vroom.Models;

namespace vroom.MappingProfiles
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Model, ModelResources>();            
        }
    }
}
