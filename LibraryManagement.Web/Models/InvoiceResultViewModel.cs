namespace LibraryManagement.Web.Models
{
    public class InvoiceResultViewModel
    {
        public bool Success { get; set; }
        public bool NotFound { get; set; }
        public bool Forbidden { get; set; }
        public string Error { get; set; } = string.Empty;
        public byte[] PdfBytes { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
    }
}
