using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using PdfMergeCompany.Models;

namespace PdfMergeCompany.Services
{
    public class PdfMergeService : IPdfMergeService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PdfMergeService> _logger;

        public PdfMergeService(IWebHostEnvironment environment, ILogger<PdfMergeService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<PdfMergeResult> MergeAllCompanyPdfsAsync()
        {
            var result = new PdfMergeResult();
            
            try
            {
                var pdfsPath = Path.Combine(_environment.WebRootPath, "pdfs");
                var mergedPath = Path.Combine(_environment.WebRootPath, "merged");

                // Ensure directories exist
                if (!Directory.Exists(pdfsPath))
                {
                    result.Success = false;
                    result.Message = $"PDFs directory not found: {pdfsPath}";
                    return result;
                }

                // Create merged directory if it doesn't exist
                Directory.CreateDirectory(mergedPath);

                // Get all company directories
                var companyDirectories = Directory.GetDirectories(pdfsPath);
                
                if (companyDirectories.Length == 0)
                {
                    result.Success = true;
                    result.Message = "No company directories found to process.";
                    return result;
                }

                result.TotalCompaniesProcessed = companyDirectories.Length;

                foreach (var companyDir in companyDirectories)
                {
                    var companyName = Path.GetFileName(companyDir);
                    var companyResult = await ProcessCompanyPdfsAsync(companyDir, companyName, mergedPath);
                    
                    result.CompanyResults.Add(companyResult);
                    
                    if (companyResult.Success)
                        result.SuccessfulMerges++;
                    else
                        result.FailedMerges++;
                }

                result.Success = result.SuccessfulMerges > 0;
                result.Message = $"Processed {result.TotalCompaniesProcessed} companies. " +
                               $"Successful: {result.SuccessfulMerges}, Failed: {result.FailedMerges}";

                _logger.LogInformation(result.Message);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error during PDF merge process: {ex.Message}";
                _logger.LogError(ex, "Error during PDF merge process");
            }

            return result;
        }

        private async Task<CompanyMergeResult> ProcessCompanyPdfsAsync(string companyDir, string companyName, string mergedPath)
        {
            var result = new CompanyMergeResult
            {
                CompanyName = companyName
            };

            try
            {
                // Get all PDF files in the company directory
                var pdfFiles = Directory.GetFiles(companyDir, "*.pdf", SearchOption.TopDirectoryOnly);
                
                if (pdfFiles.Length == 0)
                {
                    result.Success = true;
                    result.Message = $"No PDF files found in {companyName} directory.";
                    return result;
                }

                if (pdfFiles.Length == 1)
                {
                    // If only one PDF, just copy it to merged folder
                    var singlePdfPath = Path.Combine(mergedPath, $"{companyName}_merged.pdf");
                    File.Copy(pdfFiles[0], singlePdfPath, true);
                    
                    result.Success = true;
                    result.PdfCount = 1;
                    result.OutputFilePath = singlePdfPath;
                    result.Message = $"Single PDF copied for {companyName}.";
                    return result;
                }

                // Sort files by name for consistent ordering
                Array.Sort(pdfFiles);
                result.PdfCount = pdfFiles.Length;

                var outputPath = Path.Combine(mergedPath, $"{companyName}_merged.pdf");
                result.OutputFilePath = outputPath;

                // Perform the merge using iText7 with Smart Mode
                await MergePdfsAsync(pdfFiles, outputPath);

                result.Success = true;
                result.Message = $"Successfully merged {pdfFiles.Length} PDFs for {companyName}.";
                
                _logger.LogInformation($"Merged {pdfFiles.Length} PDFs for company: {companyName}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error processing {companyName}: {ex.Message}";
                _logger.LogError(ex, $"Error processing company: {companyName}");
            }

            return result;
        }

        private async Task MergePdfsAsync(string[] inputPaths, string outputPath)
        {
            await Task.Run(() =>
            {
                using var outputPdf = new PdfDocument(new PdfWriter(outputPath));
                var merger = new PdfMerger(outputPdf);

                // Enable Smart Mode for better performance and smaller file sizes
                merger.SetCloseSourceDocuments(true);

                foreach (var inputPath in inputPaths)
                {
                    try
                    {
                        using var inputPdf = new PdfDocument(new PdfReader(inputPath));
                        merger.Merge(inputPdf, 1, inputPdf.GetNumberOfPages());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to merge PDF: {inputPath}. Error: {ex.Message}");
                        throw new InvalidOperationException($"Failed to merge PDF: {Path.GetFileName(inputPath)}", ex);
                    }
                }

                outputPdf.Close();
            });
        }
    }
}