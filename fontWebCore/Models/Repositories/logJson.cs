using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fontWebCore.Models.Repositories
{
    public class logJson
    {
        public System.Guid log_id { get; set; }
        public string company { get; set; }
        public string source { get; set; }
        public string request_json { get; set; }
        public Nullable<System.DateTime> request_time { get; set; }
        public string response_json { get; set; }
        public Nullable<System.DateTime> response_time { get; set; }
    }
}
