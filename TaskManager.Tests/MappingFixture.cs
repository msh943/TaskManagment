using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Tests
{
    public static class MappingFixture
    {
        public static IMapper CreateMapper(params Type[] profileAssemblyMarkers)
        {
            var expr = new MapperConfigurationExpression();

            foreach (var t in profileAssemblyMarkers)
                expr.AddMaps(t.Assembly);

            var config = new MapperConfiguration(expr, NullLoggerFactory.Instance);

            return config.CreateMapper();
        }
    }
}
