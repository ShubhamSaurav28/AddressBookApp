using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AddressBookApp.Controllers
{
    [ApiController]
    [Route("[controller]/api")]
    [Authorize]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookBL;

        public AddressBookController(IAddressBookBL addressBookBL)
        {
            _addressBookBL = addressBookBL;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllABs()
        {
            try
            {
                Console.WriteLine("this is con1");
                var addressBooks = await _addressBookBL.GetAllABsBL();
                Console.WriteLine("this is con2");
                return Ok(new ResponseModel<IEnumerable<AddressBookEntity>>
                {
                    Success = true,
                    Message = "Successfully retrieved all address books.",
                    Data = addressBooks
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetABById(int id)
        {
            try
            {
                var addressBook = await _addressBookBL.GetABByIdBL(id);
                if (addressBook == null)
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address book not found."
                    });

                return Ok(new ResponseModel<AddressBookEntity>
                {
                    Success = true,
                    Message = "Successfully retrieved address book.",
                    Data = addressBook
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<AddressBookEntity>>> AddAB([FromBody] AddressBookCreateDTO addressBookCreateModel)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Unauthorized: User ID not found in token"
                    });
                }

                var addressBook = await _addressBookBL.AddADBL(addressBookCreateModel, userId);

                return CreatedAtAction(nameof(GetABById), new { id = addressBook.UserId }, new ResponseModel<AddressBookEntity>
                {
                    Success = true,
                    Message = "Successfully added address book.",
                    Data = addressBook
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] AddressBookUpdateDTO addressBookUpdateModel)
        {
            try
            {
                var success = await _addressBookBL.UpdateADBL(id, addressBookUpdateModel);
                if (!success)
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address book not found."
                    });

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Address book updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                var success = await _addressBookBL.DeleteADBL(id);
                if (!success)
                    return NotFound(new ResponseModel<string>
                    {
                        Success = false,
                        Message = "Address book not found."
                    });

                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Address book deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                });
            }
        }
    }
}
