using Microsoft.AspNetCore.Mvc;
using PdfMergeCompany.Services;

namespace PdfMergeCompany.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfMergeController : ControllerBase
    {
        private readonly IPdfMergeService _pdfMergeService;
        private readonly ILogger<PdfMergeController> _logger;

        public PdfMergeController(IPdfMergeService pdfMergeService, ILogger<PdfMergeController> logger)
        {
            _pdfMergeService = pdfMergeService;
            _logger = logger;
        }

        [HttpPost("merge-all-companies")]
        public async Task<IActionResult> MergeAllCompanies()
        {
            try
            {
                _logger.LogInformation("Starting PDF merge process for all companies");
                
                var result = await _pdfMergeService.MergeAllCompanyPdfsAsync();
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in MergeAllCompanies endpoint");
                return StatusCode(500, new { 
                    Success = false, 
                    Message = "An unexpected error occurred during the merge process.",
                    Error = ex.Message 
                });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
        }
    }
}