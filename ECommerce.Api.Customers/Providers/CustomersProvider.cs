using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Customers.Providers
{
    public class CustomersProvider : ICustomersProvider
    {

        private readonly CustomersDbContext dbContext;
        private readonly ILogger<CustomersProvider> logger;
        private readonly IMapper mapper;

        public CustomersProvider(CustomersDbContext dbContext, ILogger<CustomersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }


        private void SeedData()
        {
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.Add(new Customer() { Id = 1, Name = "Jessica Smith", Address = "20 street" });
                dbContext.Customers.Add(new Customer() { Id = 2, Name = "John Smith", Address = "25 street" });
                dbContext.Customers.Add(new Customer() { Id = 3, Name = "William Smith", Address = "26 street" });
                dbContext.Customers.Add(new Customer() { Id = 4, Name = "Maj Spejson", Address = "28 street" });
                dbContext.SaveChanges();
            }


        }

        async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers, string ErrorMessage)> ICustomersProvider.GetCustomersAsync()
        {
            try
            {
                var customers = await dbContext.Customers.ToListAsync();
                if (customers != null && customers.Any())
                {
                    var result = mapper.Map<IEnumerable<Customer>, IEnumerable<Models.Customer>>(customers);
                    return (true, result, null);
                }

                return (false, null, "Not found");

            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }

        }

        async Task<(bool IsSuccess, Models.Customer Customer, string ErrorMessage)> ICustomersProvider.GetCustomerAsync(int id)
        {
            try
            {
                var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

                if (customer != null)
                {
                    var result = mapper.Map<Customer, Models.Customer>(customer);
                    return (true, result, null);
                }

                return (false, null, "Not found");

            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }

        }
    }
}
