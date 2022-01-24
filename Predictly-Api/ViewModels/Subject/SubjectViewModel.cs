using System.Collections.Generic;

namespace Predictly_Api.ViewModels.Subject
{
    public class SubjectViewModel
    {
        public List<SubjectDataModel> CoreSubjects { get; set; }
        public List<SubjectDataModel> Bucket1 { get; set; }
        public List<SubjectDataModel> Bucket2 { get; set; }
        public List<SubjectDataModel> Bucket3 { get; set; }
    }

    public class SubjectDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
