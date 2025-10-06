using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;
using System.Threading.Tasks;
using Order = ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Models.Order;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        //private readonly ProductService _productService;
        private readonly CustomerService _customerService;
        private readonly ProductService _productService;
        private readonly QueueStorageService _queueStorageService;
        private readonly FileShareStorageService _fileShareService;

        public OrderController(OrderService orderService, CustomerService customerService, ProductService productService, QueueStorageService queueStorageService, FileShareStorageService fileShareStorageService)
        {
            _customerService = customerService;
            _productService = productService;
            _orderService = orderService;
            _queueStorageService = queueStorageService;
            _fileShareService = fileShareStorageService;

        }

        public async Task<IActionResult> Index()
        {
            var customer = await _customerService.GetAllCustomersAsync();
            var product = await _productService.GetAllProductAsync();
            var order = await _orderService.GetAllOrdersAsync();

            var orderList = order.Select(o => new
            {
                Order = o,
                CustomerName = customer.FirstOrDefault(c => c.RowKey == o.CustomerId)?.CustomerFirstName ?? "Unknown",
                ProductName = product.FirstOrDefault(p => p.RowKey == o.ProductId)?.ProductName ?? "Unknown",
            });

            ViewBag.Orders = orderList;
            return View(order);
        }

        // GET: /Releases/Details/{partitionKey}/{rowKey}
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var order = await _orderService.GetOrderAsync(partitionKey, rowKey);
            if (order == null) return NotFound();

            var customer = await _customerService.GetCustomerAsync("customer", order.CustomerId!);
            var product = await _productService.GetProductAsync("product", order.ProductId!);

            ViewBag.CustomerName = customer?.CustomerFirstName ?? "Unknown";
            ViewBag.ProductName = product?.ProductName ?? "Unknown";

            return View(order);
        }

        // GET: /Releases/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: /Releases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                order.PartitionKey = "order";
                order.RowKey = Guid.NewGuid().ToString();
                order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
                //order.OrderDate = DateTime.UtcNow;

                await _orderService.AddOrderAsync(order);

                // Audit log
                await _queueStorageService.SendMessagesAsync(new
                {
                    Action = "Create Order",
                    Entity = "Order",
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    Status = order.OrderStatus,
                    Customer = order.CustomerId,
                    Product = order.ProductId,
                    Timestamp = DateTime.UtcNow
                });

                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns();
            return View(order);
        }

        // GET: /Releases/Edit/{partitionKey}/{rowKey}
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var release = await _orderService.GetOrderAsync(partitionKey, rowKey);
            if (release == null) return NotFound();

            await PopulateDropdowns();
            return View(release);
        }

        //Code Attribution
        //This method was taken from stackoverflow
        //https://stackoverflow.com/questions/7984379/how-can-i-make-an-mvc-post-return-me-to-the-previous-page
        //Samantha J T Star
        //https://stackoverflow.com/users/975566/samantha-j-t-star

        // POST: /Release/Edit/{partitionKey}/{rowKey}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
                await _orderService.UpdateOrderAsync(order);

                // Audit log
                await _queueStorageService.SendMessagesAsync(new
                {
                    Action = "Edit Order",
                    Entity = "Order",
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    Status = order.OrderStatus,
                    Customer = order.CustomerId,
                    Product = order.ProductId,
                    Timestamp = DateTime.UtcNow
                });

                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdowns();
            return View(order);
        }

        // GET: /Releases/Delete/{partitionKey}/{rowKey}
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var order = await _orderService.GetOrderAsync(partitionKey, rowKey);
            if (order == null) return NotFound();

            var customer = await _customerService.GetCustomerAsync("customer", order.CustomerId!);
            var product = await _productService.GetProductAsync("product", order.ProductId!);

            ViewBag.CustomerName = customer?.CustomerFirstName ?? "Unknown";
            ViewBag.ProductName = product?.ProductName ?? "Unknown";

            return View(order);
        }

        // POST: /Releases/DeleteConfirmed/{partitionKey}/{rowKey}
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            await _orderService.DeleteOrderAsync(partitionKey, rowKey);

            // Audit log
            await _queueStorageService.SendMessagesAsync(new
            {
                Action = "Delete Order",
                Entity = "Order",
                Timestamp = DateTime.UtcNow
            });

            return RedirectToAction(nameof(Index));
        }

        // Helper: populate dropdown lists
        private async Task PopulateDropdowns()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var products = await _productService.GetAllProductAsync();

            ViewBag.Customers = new SelectList(customers, "RowKey", "CustomerFirstName");
            ViewBag.Products = new SelectList(products, "RowKey", "ProductName");
        }
    }
}

