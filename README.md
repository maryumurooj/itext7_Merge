# itext7_Merge

# PDF Merge API

A powerful ASP.NET Core Web API that automatically merges PDF files on a company-wise basis using iText7 with Smart Mode optimization.

## 🚀 Features

- **Company-wise PDF merging**: Automatically processes subdirectories in the `pdfs` folder
- **Smart Mode optimization**: Uses iText7's Smart Mode for efficient merging and smaller file sizes
- **Robust error handling**: Comprehensive logging and graceful error recovery
- **RESTful API**: Simple HTTP POST endpoint to trigger the merge process
- **Flexible file handling**: Handles single PDFs, multiple PDFs, and empty folders intelligently

## 📁 Project Structure

```
PdfMergeAPI/
├── Controllers/
│   └── PdfMergeController.cs      # API endpoints
├── Models/
│   └── PdfMergeResult.cs          # Response models
├── Services/
│   ├── IPdfMergeService.cs        # Service interface
│   └── PdfMergeService.cs         # Core merge logic
├── wwwroot/
│   ├── pdfs/                      # Input folder (company subdirectories)
│   │   ├── CompanyA/
│   │   │   ├── document1.pdf
│   │   │   ├── document2.pdf
│   │   │   └── report.pdf
│   │   ├── CompanyB/
│   │   │   └── invoice.pdf
│   │   └── CompanyC/
│   │       ├── contract.pdf
│   │       └── agreement.pdf
│   └── merged/                    # Output folder (merged PDFs)
│       ├── CompanyA_merged.pdf
│       ├── CompanyB_merged.pdf
│       └── CompanyC_merged.pdf
├── Program.cs                     # Application entry point
├── PdfMergeAPI.csproj            # Project file
└── appsettings.json              # Configuration
```

## 🛠️ Dependencies

- **itext7** (v8.0.5): Core PDF manipulation library
- **itext7.bouncy-castle-adapter** (v8.0.5): Required for Smart Mode functionality


## ⚡ Quick Start

### 1. Clone or Create Project
```bash
dotnet new webapi -n PdfMergeAPI
cd PdfMergeAPI
```

### 2. Install Required Packages
```bash
dotnet add package itext7 --version 8.0.5
dotnet add package itext7.bouncy-castle-adapter --version 8.0.5
dotnet add package Swashbuckle.AspNetCore --version 6.4.0
```

### 3. Create Folder Structure
```bash
mkdir -p wwwroot/pdfs
mkdir -p wwwroot/merged
mkdir Controllers Models Services
```

### 4. Add Your PDF Files
Create company folders inside `wwwroot/pdfs/` and place your PDF files:
```
wwwroot/pdfs/
├── CompanyA/
│   ├── file1.pdf
│   └── file2.pdf
└── CompanyB/
    └── file1.pdf
```

### 5. Run the Application
```bash
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:xxxx`
- **HTTP**: `http://localhost:xxxx`

## 🔗 API Endpoints

### POST `/api/pdfmerge/merge-all-companies`
Merges all PDFs for each company in the pdfs folder.

**Request:**
```http
POST /api/pdfmerge/merge-all-companies
Content-Type: application/json
```

**Response:**
```json
{
  "success": true,
  "message": "Processed 3 companies. Successful: 3, Failed: 0",
  "companyResults": [
    {
      "companyName": "CompanyA",
      "success": true,
      "message": "Successfully merged 3 PDFs for CompanyA.",
      "pdfCount": 3,
      "outputFilePath": "/path/to/wwwroot/merged/CompanyA_merged.pdf"
    },
    {
      "companyName": "CompanyB",
      "success": true,
      "message": "Single PDF copied for CompanyB.",
      "pdfCount": 1,
      "outputFilePath": "/path/to/wwwroot/merged/CompanyB_merged.pdf"
    }
  ],
  "totalCompaniesProcessed": 2,
  "successfulMerges": 2,
  "failedMerges": 0
}
```

### GET `/api/pdfmerge/health`
Health check endpoint to verify API is running.

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2024-06-19T10:30:00.000Z"
}
```

## 🧪 Testing the API

### Using cURL
```bash
# Merge all company PDFs
curl -X POST "https://localhost:7000/api/pdfmerge/merge-all-companies" \
     -H "Content-Type: application/json"
```

### Using PowerShell
```powershell
# Health check
Invoke-RestMethod -Uri "https://localhost:7000/api/pdfmerge/health" -Method Get

# Merge PDFs
Invoke-RestMethod -Uri "https://localhost:7000/api/pdfmerge/merge-all-companies" -Method Post
```


## 🔧 How It Works

1. **Scan Phase**: The API scans the `wwwroot/pdfs/` directory for company subdirectories
2. **Processing Phase**: For each company folder:
   - Discovers all `.pdf` files
   - Sorts files alphabetically for consistent ordering
   - Applies iText7 Smart Mode for optimal merging
3. **Output Phase**: Creates merged PDF files in `wwwroot/merged/` with the naming pattern: `{CompanyName}_merged.pdf`

## 📋 File Handling Logic

- **Multiple PDFs**: Merges all PDFs in the folder using Smart Mode
- **Single PDF**: Copies the file to the merged folder (no processing needed)
- **Empty folders**: Skips folders with no PDF files and logs the result
- **File ordering**: PDFs are sorted alphabetically before merging for consistent results

