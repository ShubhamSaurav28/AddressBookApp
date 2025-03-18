using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;

        public AddressBookBL(IAddressBookRL addressBookRL)
        {
            _addressBookRL = addressBookRL;
        }

        public async Task<IEnumerable<AddressBookEntity>> GetAllABsBL()
        {
            try
            {
                return await _addressBookRL.GetAllABsRL();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving address books", ex);
            }
        }

        public async Task<AddressBookEntity> GetABByIdBL(int id)
        {
            try
            {
                return await _addressBookRL.GetABByIdRL(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving address book with ID {id}", ex);
            }
        }

        public async Task<AddressBookEntity> AddADBL(AddressBookCreateModel addressBookCreateModel)
        {
            try
            {
                return await _addressBookRL.AddADRL(addressBookCreateModel);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new address book", ex);
            }
        }

        public async Task<bool> UpdateADBL(int id, AddressBookUpdateModel addressBookUpdateModel)
        {
            try
            {
                return await _addressBookRL.UpdateADRL(id, addressBookUpdateModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating address book with ID {id}", ex);
            }
        }

        public async Task<bool> DeleteADBL(int id)
        {
            try
            {
                return await _addressBookRL.DeleteADRL(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting address book with ID {id}", ex);
            }
        }
    }
}
