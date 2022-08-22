using System.Collections.Generic;

namespace backendWeb.Models.ViewModel
{
    public class viewModelRequestforExam
    {
        public string examine_no { get; set; }
        public bool forceTryForExam { get; set; }
        public string receive_id { get; set; }
        public string comment { get; set; }
        public IList<FileUpload> FileUploads { get; set; }
    }
}