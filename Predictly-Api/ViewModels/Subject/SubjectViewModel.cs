using System.Collections.Generic;

namespace Predictly_Api.ViewModels.Subject
{
    public class SubjectViewModel
    {
        public List<SubjectDataViewModel> CoreSubjects { get; set; }
        public List<SubjectDataViewModel> Bucket1 { get; set; }
        public List<SubjectDataViewModel> Bucket2 { get; set; }
        public List<SubjectDataViewModel> Bucket3 { get; set; }
    }

    public class SubjectDataViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
