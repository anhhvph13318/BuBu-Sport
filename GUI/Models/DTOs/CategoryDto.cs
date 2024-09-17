namespace GUI.Models.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; 
    public int Status { get; set; }
    public string StatusText => Status == 0 ? "Hoạt động" : "Không hoạt động";
    public bool StatusFlag => Status == 0;
    public DateTime CreateDate { get; set; }
}
