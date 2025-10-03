using System.ComponentModel.DataAnnotations;

namespace APPR_Foudation.Models
{
    public class ResourceDonation
    {
        [Key]
        public int DonationId { get; set; }

        [Required]
        public int DonorUserId { get; set; }

        [Required(ErrorMessage = "Donation type is required")]
        [Display(Name = "Donation Type")]
        public DonationType DonationType { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Item name cannot be longer than 100 characters")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [StringLength(50)]
        public string Unit { get; set; } = "items";

        public string Description { get; set; } = string.Empty;

        public DateTime DonationDate { get; set; } = DateTime.Now;

        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        [StringLength(200)]
        [Display(Name = "Preferred Drop-off Location")]
        public string PreferredDropOffLocation { get; set; } = string.Empty;
    }

    public enum DonationType
    {
        Food,
        Clothing,
        MedicalSupplies,
        ShelterMaterials,
        Financial,
        Other
    }

    public enum DonationStatus
    {
        Pending,
        Received,
        InTransit,
        Distributed
    }
}