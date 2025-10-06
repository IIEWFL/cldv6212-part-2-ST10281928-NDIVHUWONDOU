using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services.Storage;
using ST10281918_NDIVHUWONDOU_CLDV6212_PART1.Services;
namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var storageConnectionString = builder.Configuration.GetConnectionString("storageAccount")
                ?? throw new InvalidOperationException("Storage connection string is missing");

            builder.Services.AddSingleton(new CustomerService(storageConnectionString,"Customer"));

            builder.Services.AddSingleton(new OrderService(storageConnectionString, "Order"));
            // Register domain-specific services
            //var blobService = new BlobStorageService(storageConnectionString, "musicmanager-multimedia");
            //builder.Services.AddSingleton(blobService);

            var blobService = new BlobStorageService(storageConnectionString, "productphotos");
            builder.Services.AddSingleton(blobService);
            builder.Services.AddSingleton(new ProductService(storageConnectionString, "Product",blobService));

            //builder.Services.AddSingleton(new BlobStorageService(storageConnectionString, "customerphotos"));
            //builder.Services.AddSingleton(new BlobStorageService(storageConnectionString, "productphotos"));

            builder.Services.AddSingleton(new QueueStorageService(storageConnectionString, "orderqueue"));
            builder.Services.AddSingleton(new FileShareStorageService(storageConnectionString, "retail-log-file"));

            //Register the functionn service
            builder.Services.AddHttpClient<FunctionService>();
            builder.Services.AddSingleton<FunctionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
