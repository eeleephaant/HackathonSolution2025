namespace StartupService.Models;

public class StartupDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserDTO OwnerUser { get; set; }
    public List<UserDTO> Workers { get; set; }
    public List<UserDTO> Scientists { get; set; }
    public List<UserDTO> Investors { get; set; }
}