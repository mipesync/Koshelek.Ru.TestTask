using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

/// <summary>
/// Message object
/// </summary>
public class Message
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Content is required")]
    [StringLength(128, ErrorMessage = "Content cannot be longer than 128 characters")]
    public string Content { get; set; } = null!;
    
    public DateTime Timestamp { get; set; }

    [Required(ErrorMessage = "OrderId is required")]
    public int OrderId { get; set; }
}