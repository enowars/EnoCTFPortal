using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnoLandingPageBackend.Models
{
    /// <summary>
    /// A DTO representing errors.
    /// </summary>
    public class ErrorDto
    {
        /// <summary>
        /// Gets or sets the HTTP StatusCode representing the error.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the error Message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a CorrelationId to find the issue in the log.
        /// </summary>
        public Guid CorrelationId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }
    }
}
