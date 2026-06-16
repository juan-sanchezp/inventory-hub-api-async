using InventoryHub.DTOs;

using InventoryHub.Responses;
using InventoryHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryHub.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAll();

            return Ok(
                ResponseFactory.Success(
                    customers,
                    "Customers retrieved successfully"
                )
            );
        }

        [HttpGet("{id}", Name = "GetCustomerById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetById(id);

            if (customer == null)
            {
                return NotFound(
                    ResponseFactory.Fail<CustomerDTO>(
                        "Customer not found"
                    )
                );
            }

            return Ok(
                ResponseFactory.Success(
                    customer,
                    "Customer retrieved successfully"
                )
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SaveCustomer([FromBody] CustomerDTO customerDTO)
        {
            var customer = await _customerService.Save(customerDTO);

            if (customer == null)
            {
                return Conflict(
                    ResponseFactory.Fail<CustomerDTO>(
                        "Customer already exists"
                    )
                );
            }

            return CreatedAtRoute(
                "GetCustomerById",
                new { id = customer.Id },
                ResponseFactory.Success(
                    customer,
                    "Customer created successfully"
                )
            );
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDTO customerDTO)
        {
            var updated = await _customerService.Update(id, customerDTO);

            if (updated == null)
            {
                return NotFound(
                    ResponseFactory.Fail<CustomerDTO>(
                        "Customer not found"
                    )
                );
            }

            return Ok(
                ResponseFactory.Success(
                    updated,
                    "Customer updated successfully"
                )
            );
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var deleted = await _customerService.DeleteById(id);

            if (deleted == null)
            {
                return NotFound(
                    ResponseFactory.Fail<CustomerDTO>(
                        "Customer not found"
                    )
                );
            }

            return Ok(
                ResponseFactory.Success(
                    deleted,
                    "Customer deleted successfully"
                )
            );
        }
    }
}

