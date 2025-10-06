using Microsoft.AspNetCore.Mvc;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerService _customerService;
        private readonly FunctionService _functionService;
        //private readonly BlobStorageService _blobStorageService;
        private readonly QueueStorageService _queueStorageService;
        private readonly FileShareStorageService _fileShareService;

        public CustomerController(CustomerService customerService,QueueStorageService queueStorageService, FileShareStorageService fileShareStorageService, FunctionService functionService)
        {
            _customerService = customerService;
            _queueStorageService = queueStorageService;
            _fileShareService = fileShareStorageService;
            _functionService = functionService;
        }
        public async Task<IActionResult> Index()
        {
            var customer = await _functionService.GetCustomersAsync();
            return View(customer);
        }

        //GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var customerValue = new Customer
                {
                    PartitionKey = "customer",
                    RowKey = Guid.NewGuid().ToString(),
                    CustomerID = Guid.NewGuid().ToString(),
                    CustomerFirstName = customer.CustomerFirstName,
                    CustomerLastName = customer.CustomerLastName,
                    CustomerEmail = customer.CustomerEmail,
                    CustomerPhone = customer.CustomerPhone
                };
                await _customerService.AddCustomerAsync(customerValue);

                // Audit log
                await _queueStorageService.SendMessagesAsync(new
                {
                    Action = "Create Customer",
                    Entity = "Customer",
                    CustomerId = customer.CustomerID,
                    CustomerName = customer.CustomerFirstName,
                    CustomerRowKey = customer.RowKey,
                    Timestamp = DateTime.UtcNow
                });
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        //GET: Customer/Details
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var customer = await _customerService.GetCustomerAsync(partitionKey, rowKey);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        //GET: Customer/Edit
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var customer = await _customerService.GetCustomerAsync(partitionKey, rowKey);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);  
        }

        //POST: Customer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(string.IsNullOrEmpty(customer.PartitionKey) || string.IsNullOrEmpty(customer.RowKey))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid customer data");
                        return View(customer);
                    }

                    var existingCustomer = await _customerService.GetCustomerAsync(customer.PartitionKey, customer.RowKey);
                    if (existingCustomer == null)
                    {
                        return NotFound();
                    }
                    await _customerService.UpdateCustomerAsync(customer);

                    // Audit log
                    await _queueStorageService.SendMessagesAsync(new
                    {
                        Action = "Edit Customer",
                        Entity = "Customer",
                        CustomerId = customer.CustomerID,
                        CustomerName = customer.CustomerFirstName,
                        CustomerRowKey = customer.RowKey,
                        Timestamp = DateTime.UtcNow
                    });


                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error has occured while updating the student. {ex.Message}");
                }

            }
            return View(customer);  
        }

        //GET: Customer/Delete
        public async Task<IActionResult> Delete(string partitionKey,  string rowKey)
        {
            var customer = await _customerService.GetCustomerAsync(partitionKey, rowKey);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        //POST: Customer/Delete
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            var customer = await _customerService.GetCustomerAsync(partitionKey, rowKey);
 
            if (customer != null)
            {
                await _customerService.DeleteCustomerAsync(partitionKey, rowKey);
            }
            // Audit log
            await _queueStorageService.SendMessagesAsync(new
            {
                Action = "Delete Customer",
                Entity = "Customer",
                CustomerId = customer.CustomerID,
                CustomerName = customer.CustomerFirstName,
                CustomerRowKey = customer.RowKey,
                Timestamp = DateTime.UtcNow
            });

            return RedirectToAction(nameof(Index));
 
        }
    }
}
