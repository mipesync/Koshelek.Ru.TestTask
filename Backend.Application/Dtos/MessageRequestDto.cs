namespace Backend.Application.Dtos;

public class MessageRequestDto
{
    public string Content { get; set; } = null!;
    public int OrderId { get; set; }
}