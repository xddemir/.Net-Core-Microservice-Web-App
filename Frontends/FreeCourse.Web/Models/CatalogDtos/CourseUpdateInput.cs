using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.CatalogDtos;

public class CourseUpdateInput
{
    public string Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }

    public decimal Price { get; set; }

    public string? Picture { get; set; }
    
    public string? UserId { get; set; }

    public FeatureViewModel Feature { get; set; }
    
    [Required]
    public string CategoryId { get; set; }
    
    public IFormFile PhotoFormFile { get; set; }
}