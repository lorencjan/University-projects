namespace Hotel.Command.Application.Clients.Dtos;

public class ClientDto : UpdateClientDto
{
    public string Email { get; set; }
    public string Phone { get; set; }
    public string IdentityCardNumber { get; set; }
}