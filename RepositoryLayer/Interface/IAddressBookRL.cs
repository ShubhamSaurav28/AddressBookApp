using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        // Get all address book entries
        Task<IEnumerable<AddressBookEntity>> GetAllABsRL();

        // Get a single address book entry by ID
        Task<AddressBookEntity> GetABByIdRL(int id);

        // Add a new address book entry
        Task<AddressBookEntity> AddADRL(AddressBookCreateModel addressBookCreateModel);

        // Update an existing address book entry
        Task<bool> UpdateADRL(int id, AddressBookUpdateModel addressBookUpdateModel);

        // Delete an address book entry by ID
        Task<bool> DeleteADRL(int id);
    }
}
