using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Dtos
{
    public class CreateGenreDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}