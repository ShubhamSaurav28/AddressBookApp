using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Context;
using StackExchange.Redis;

namespace RepositoryLayer.Service
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly ApplicationDbContext _context;
        private readonly IDatabase _redisDb;

        public AddressBookRL(ApplicationDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redisDb = redis.GetDatabase();
        }

        // Get all address book entries with caching
        public async Task<IEnumerable<AddressBookEntity>> GetAllABsRL()
        {
            string cacheKey = "AddressBooks";
            // Check Redis cache first
            var cachedData = await _redisDb.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                Console.WriteLine($"✅ Data found in Redis: {cachedData}");
                return JsonSerializer.Deserialize<List<AddressBookEntity>>(cachedData);
            }

            Console.WriteLine("❌ No cache found, fetching from database...");

            // Fetch from DB
            var addressBooks = await _context.AddressBooks.AsNoTracking().ToListAsync();

            // Serialize & Store in Redis
            string serializedData = JsonSerializer.Serialize(addressBooks);
            Console.WriteLine($"🔄 Storing in Redis: {serializedData}");

            await _redisDb.StringSetAsync(cacheKey, serializedData, TimeSpan.FromMinutes(10));

            return addressBooks;
        }


        // Get a single address book entry by ID with caching
        public async Task<AddressBookEntity> GetABByIdRL(int id)
        {
            string cacheKey = $"AddressBook:{id}";

            // Check Redis cache
            var cachedData = await _redisDb.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<AddressBookEntity>(cachedData);
            }

            // Fetch from DB and store in Redis
            var addressBook = await _context.AddressBooks.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (addressBook != null)
            {
                await _redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(addressBook), TimeSpan.FromMinutes(10)); // Cache for 10 mins
            }

            return addressBook;
        }

        // Add a new address book entry and clear cache
        public async Task<AddressBookEntity> AddADRL(AddressBookCreateDTO addressBookCreateModel, int userId)
        {
            try
            {
                var newAddressBook = new AddressBookEntity
                {
                    Name = addressBookCreateModel.Name,
                    PhoneNumber = addressBookCreateModel.PhoneNumber,
                    Email = addressBookCreateModel.Email,
                    UserId = userId,
                    Address = addressBookCreateModel.Address,
                    CreatedAt = DateTime.UtcNow
                };

                _context.AddressBooks.Add(newAddressBook);
                await _context.SaveChangesAsync();

                // Invalidate cache
                await _redisDb.KeyDeleteAsync("AddressBooks");

                return newAddressBook;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding new address book entry", ex);
            }
        }

        // Update an existing address book entry and clear cache
        public async Task<bool> UpdateADRL(int id, AddressBookUpdateDTO addressBookUpdateModel)
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

                // Invalidate cache
                await _redisDb.KeyDeleteAsync($"AddressBook:{id}");
                await _redisDb.KeyDeleteAsync("AddressBooks");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating address book with ID {id}", ex);
            }
        }

        // Delete an address book entry and clear cache
        public async Task<bool> DeleteADRL(int id)
        {
            try
            {
                var addressBook = await _context.AddressBooks.FirstOrDefaultAsync(a => a.Id == id);
                if (addressBook == null)
                    return false;

                _context.AddressBooks.Remove(addressBook);
                await _context.SaveChangesAsync();

                // Invalidate cache
                await _redisDb.KeyDeleteAsync($"AddressBook:{id}");
                await _redisDb.KeyDeleteAsync("AddressBooks");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting address book with ID {id}", ex);
            }
        }
    }
}
