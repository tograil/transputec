using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CrisesControl.Api.Maintenance
{
    public class SwaggerParameterAttributeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<HttpGetAttribute>();

            if (attributes.Any())
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "pageNumber",
                    Description = "Page number",
                    In = ParameterLocation.Query,
                    Required = false,
                    Schema = new OpenApiSchema { Type = "Int" }
                });

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "pageSize",
                    Description = "Page size",
                    In = ParameterLocation.Query,
                    Required = false,
                    Schema = new OpenApiSchema { Type = "Int" }
                });
            }
        }
    }
}