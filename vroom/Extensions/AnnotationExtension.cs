using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vroom.Extensions
{
    public class YearRangeTillDateAttribute:RangeAttribute
    {
        public YearRangeTillDateAttribute(int StartYear):base(StartYear,DateTime.Today.Year)
        {

        }
    }
}
