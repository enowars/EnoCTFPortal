//using Swashbuckle.AspNetCore.SwaggerGen;
//using Swashbuckle.AspNetCore.Swagger;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Microsoft.EntityFrameworkCore.Internal;
//using System.Linq;
//using EnoLandingPageBackend.Models;
//using Swashbuckle.Swagger;
//using IDocumentFilter = Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter;
//using Microsoft.OpenApi.Models;

//namespace EnoLandingPageBackend.Swashbuckle.Filter
//{
//    internal class DefaultFilter : IDocumentFilter
//    {
//        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
//        {
//            foreach (var item in swaggerDoc.Paths.Values)
//            {
//                // 500 Error
//                Dictionary<string, ErrorDto> example500 = new Dictionary<string, ErrorDto>()
//                {
//                    {
//                        "application/json",  new ErrorDto
//                        {
//                        StatusCode = 500,
//                        CorrelationId = Guid.NewGuid(),
//                        Message = "This is an example Error Message.",
//                        }
//                    }
//                };

//                UpdateItem(item, "500", "Internal server error.", context.SchemaRegistry.GetOrRegister(typeof(ErrorDto)), example500);

//                // 400 Error
//                Dictionary<string, Dictionary<string, string[]>> example400 = new Dictionary<string, Dictionary<string, string[]>>()
//                                                              {
//                                                                  {
//                                                                      "application/json",
//                                                                      new Dictionary<string, string[]>()
//                                                                          {
//                                                                              { "PropertyName", new[] { "Error converting value {null} to type 'System.Boolean'. Path 'PropertyName', line 2, position 16." } }
//                                                                          }
//                                                                  }
//                                                              };

//                // 404 Error
//                Dictionary<string, string> example404 = new Dictionary<string, string>()
//                {
//                    {
//                        "text/plain", "The entity with the given Id was not found."
//                    }
//                };

//                UpdateItem(item, "400", "Bad request - probably bad JSON was sent to the server.", context.SchemaRegistry.GetOrRegister(typeof(ValidationResult)), example400);

//                if (item.Get?.Parameters.Any(c => c.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase)) ?? false)
//                {
//                    UpdateItem(item, "404", "Item with the given Id was not found", null, example404);
//                }
//            }
//        }

//        private void UpdateItem<T>(OpenApiPathItem item, string v1, string v2, object schema = null, Dictionary<string, T> examples = null)
//        {
//            TrySetValue(item.Get, key, description, schema, examples);
//            TrySetValue(item.Put, key, description, schema, examples);
//            TrySetValue(item.Post, key, description, schema, examples);
//            TrySetValue(item.Delete, key, description, schema, examples);
//        }

//        private static void TrySetValue<T>(Operation op, string key, string description, Schema schema, Dictionary<string, T> examples)
//        {
//            if ((op == null) || op.Responses.ContainsKey(key))
//            {
//                return;
//            }

//            op.Responses.Add(key, new Response
//            {
//                Description = description,
//                Schema = schema,
//                Examples = examples,
//            });
//        }


//    }
//}
