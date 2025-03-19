using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit.Tnef;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {
        // Get all address book entries
        Task<IEnumerable<AddressBookEntity>> GetAllABsBL();

        // Get a single address book entry by ID
        Task<AddressBookEntity> GetABByIdBL(int id);

        // Add a new address book entry
        Task<AddressBookEntity> AddADBL(AddressBookCreateDTO addressBookCreateModel, int userId);

        // Update an existing address book entry
        Task<bool> UpdateADBL(int id, AddressBookUpdateDTO addressBookUpdateModel);

        // Delete an address book entry by ID
        Task<bool> DeleteADBL(int id);
    }
}
