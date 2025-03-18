using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Context;

namespace RepositoryLayer.Service
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly ApplicationDbContext _context;

        public AddressBookRL(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all address book entries
        public async Task<IEnumerable<AddressBookEntity>> GetAllABsRL()
        {
            try
            {
                return await _context.AddressBooks.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving address books", ex);
            }
        }

        // Get a single address book entry by ID
        public async Task<AddressBookEntity> GetABByIdRL(int id)
        {
            try
            {
                return await _context.AddressBooks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving address book with ID {id}", ex);
            }
        }

        // Add a new address book entry
        public async Task<AddressBookEntity> AddADRL(AddressBookCreateModel addressBookCreateModel)
        {
            try
            {
                Console.WriteLine("hi this is post");
                var newAddressBook = new AddressBookEntity
                {
                    Name = addressBookCreateModel.Name,
                    PhoneNumber = addressBookCreateModel.PhoneNumber,
                    Email = addressBookCreateModel.Email,
                    UserId = addressBookCreateModel.UserId,
                    Address = addressBookCreateModel.Address,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AddressBooks.Add(newAddressBook);
                await _context.SaveChangesAsync();
                return newAddressBook;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new address book entry", ex);
            }
        }

        // Update an existing address book entry
        public async Task<bool> UpdateADRL(int id, AddressBookUpdateModel addressBookUpdateModel)
        {
            try
            {
                var existingAddressBook = await _context.AddressBooks.FirstOrDefaultAsync(a => a.Id == id);
                if (existingAddressBook == null)
                    return false;

                existingAddressBook.Name = addressBookUpdateModel.Name;
                existingAddressBook.PhoneNumber = addressBookUpdateModel.PhoneNumber;
                existingAddressBook.Email = addressBookUpdateModel.Email;
                existingAddressBook.Address = addressBookUpdateModel.Address;
                existingAddressBook.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating address book with ID {id}", ex);
            }
        }

        // Delete an address book entry by ID
        public async Task<bool> DeleteADRL(int id)
        {
            try
            {
                var addressBook = await _context.AddressBooks.FirstOrDefaultAsync(a => a.Id == id);
                if (addressBook == null)
                    return false;

                _context.AddressBooks.Remove(addressBook);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting address book with ID {id}", ex);
            }
        }
    }
}
