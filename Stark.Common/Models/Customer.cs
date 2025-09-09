using System.Collections.Generic;

namespace Stark.Common.Models
{
    public class Customer : EntityBase, ILoggable
    {
        public Customer() : this(0)
        {
        }

        public Customer(int customerId)
        {
            CustomerId = customerId;
        }

        public int CustomerId { get; private set; }
        public int CustomerType { get; set; }
        public string FirstName { get; set; }
        public List<Address> AddressList { get; set; } = new List<Address>();
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                string fullName = LastName;
                if (!string.IsNullOrWhiteSpace(FirstName))
                {
                    if (!string.IsNullOrWhiteSpace(fullName))
                    {
                        fullName += ", ";
                    }
                    fullName += FirstName;
                }

                return fullName;
            }
        }
        public string Email { get; set; }

        public static int InstanceCount { get; set; }

        public string Log() => $"{CustomerId}: {FullName} Email: {Email} Status: {EntityState}";

        public override string ToString() => FullName;

        public override bool Validate()
        {
            bool isValid = string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Email) ? false : true;
            return isValid;
        }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
