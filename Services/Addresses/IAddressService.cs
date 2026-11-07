using newproj.DTO.Address;

namespace newproj.Services.Addresses;

public interface IAddressService
{
    Task<AddressResponseDTO?> Update(AddressUpdateDTO dto, int clientId);

    Task<bool> Delete(int clientId);
}