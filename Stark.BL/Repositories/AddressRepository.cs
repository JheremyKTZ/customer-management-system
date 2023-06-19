using Stark.Common.Models;
using System.Collections.Generic;

namespace Stark.BL.Repositories
{
    public class AddressRepository
    {
        public Address Retrieve(int addressId)
        {
            Address address = new Address();

            if (addressId == 1)
            {
                address.AddressType = 1;
                address.AddressLine1 = "Bag End";
                address.AddressLine2 = "Bagshot Raw";
                address.City = "Hobbiton";
                address.State = "Shire";
                address.Country = "Middle Earth";
                address.PostalCode = "144";
            }
            return address;
        }

        public IEnumerable<Address> RetrieveByCustomerId(int customerId)
        {
            var addressList = new List<Address>();
            
            return addressList;
        }

        public bool Save(Address address)
        {
            return true;
        }
    }
}
