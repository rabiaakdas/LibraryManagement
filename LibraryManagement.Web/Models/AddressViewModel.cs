namespace LibraryManagement.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AddressViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Adres başlığı zorunludur")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İl zorunludur")]
        public string City { get; set; }

        [Required(ErrorMessage = "İlçe zorunludur")]
        public string District { get; set; }

        [Required(ErrorMessage = "Posta kodu zorunludur")]
        [StringLength(10, ErrorMessage = "Posta kodu en fazla 10 karakter olabilir")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Detaylı adres girilmelidir")]
        public string FullAddress { get; set; }
    }



}
