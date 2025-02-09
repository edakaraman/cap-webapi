using System.Text;
using System.Text.Json;
using CapWebAPI.Data;
using DotNetCore.CAP;
using Newtonsoft.Json;

namespace CapWebAPI.Services
{
    public class CustomerConsumer : ICapSubscribe
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [CapSubscribe(AppConsts.queueName)]
        public async Task ConsumeCustomerAsync(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            _dbContext.SaveChangesAsync();
            // string c = JsonConvert.SerializeObject(customer.FirstName);
            // Console.WriteLine(c);
        }

    }
}
