namespace PdfMergeCompany.Models
{
    public class PdfMergeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CompanyMergeResult> CompanyResults { get; set; } = new();
        public int TotalCompaniesProcessed { get; set; }
        public int SuccessfulMerges { get; set; }
        public int FailedMerges { get; set; }
    }

    public class CompanyMergeResult
    {
        public string CompanyName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PdfCount { get; set; }
        public string OutputFilePath { get; set; } = string.Empty;
    }
}