using Bogus;
using DotNetCore.CAP;

namespace CapWebAPI.Services
{
    public class CustomerProducer
    {
        private readonly ICapPublisher _capPublisher;

        public CustomerProducer(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task ProduceCustomersAsync(int count)
        {
            var faker = new Faker<Customer>(AppConsts.fakerCulture)
                .RuleFor(c => c.FirstName, f => f.Person.FirstName)
                .RuleFor(c => c.LastName, f => f.Person.LastName)
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.DateOfBirth, f => f.Date.Past(30, DateTime.Now.AddYears(-18)).Date);

            var customerList = faker.Generate(count);
            foreach (var item in customerList)
            {
                try
                {
                    await _capPublisher.PublishAsync(AppConsts.queueName, item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(AppConsts.errorMess, ex);
                }
            }
        }

        public async Task HiMessageAsync(string name)
        {
            try
            {
                await _capPublisher.PublishAsync(AppConsts.queueName, name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(AppConsts.errorMess, ex);
            }
        }
    }
}
