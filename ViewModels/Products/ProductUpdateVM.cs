using System.ComponentModel.DataAnnotations;
using UniqloMvc.Models;

namespace UniqloMvc.ViewModels.Products;

public class ProductUpdateVM
{
    public int Id { get; set; }
    [MaxLength(64, ErrorMessage = "Name must be less than 64 characters."), Required]
    public string Name { get; set; }

    [MaxLength(512, ErrorMessage = "Description must be less than 512 characters."), Required]
    public string Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost Price must be a positive value."), Required]
    public decimal CostPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Sell Price must be a positive value."), Required]
    public decimal SellPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than 0."), Required]
    public int Quantity { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public int Discount { get; set; }
    public int? BrandId { get; set; }

    public string? FileUrl { get; set; }
    public IFormFile? File { get; set; }

    public IEnumerable<string>? OtherFilesUrls { get; set; }
    public ICollection<IFormFile>? OtherFiles { get; set; }

    public static implicit operator ProductUpdateVM(Product product)
    {
        ProductUpdateVM vm = new ProductUpdateVM
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            FileUrl = product.CoverImage,
            OtherFilesUrls = product.Images?.Select(x => x.ImageUrl),
            CostPrice = product.CostPrice,
            SellPrice = product.SellPrice,
            Discount = product.Discount,
            Quantity = product.Quantity,
            BrandId = product.BrandId
        };

        return vm;
    }
}
