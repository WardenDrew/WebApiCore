using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCore.Models
{
    public class WebApiCoreOptions
    {
        /// <summary>
        /// SerializerSettings to be used 
        /// </summary>
        public JsonSerializerSettings ResponseSerializerSettings { get; set; }

        /// <summary>
        /// New Default Options
        /// </summary>
        public WebApiCoreOptions()
        {
            ResponseSerializerSettings = new();
        }
    }
}
