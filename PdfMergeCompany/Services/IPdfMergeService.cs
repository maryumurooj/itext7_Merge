using PdfMergeCompany.Models;

namespace PdfMergeCompany.Services
{
    public interface IPdfMergeService
    {
        Task<PdfMergeResult> MergeAllCompanyPdfsAsync();
    }
}