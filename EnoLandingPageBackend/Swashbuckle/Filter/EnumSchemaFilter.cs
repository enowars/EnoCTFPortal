// namespace EnoLandingPageBackend.Swashbuckle.Filter
// {
//     using System;
//     using System.Linq;
//     using global::Swashbuckle.AspNetCore.SwaggerGen;
//     using Microsoft.OpenApi.Any;
//     using Microsoft.OpenApi.Models;

//     public class EnumSchemaFilter : ISchemaFilter
//     {
//         public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//         {
//             if (context.Type.IsEnum)
//             {
//                 var array = new OpenApiArray();
//                 array.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
//                 // NSwag
//                 schema.Extensions.Add("x-enumNames", array);
//                 // Openapi-generator
//                 schema.Extensions.Add("x-enum-varnames", array);
//             }
//         }
//     }
// }