using System;

namespace HiPets.Domain.Helpers.ValueObjects
{
    public class ContactInfo
    {
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }

        public string FormatPhoneNumber
        {
            get
            {
                var phoneNumber = PhoneNumber?.Replace(" ", "");


                return string.IsNullOrEmpty(phoneNumber) ? "" : (phoneNumber.Length > 10 ? Convert.ToUInt64(phoneNumber).ToString(@"(00) 00000\-0000") : Convert.ToUInt64(phoneNumber).ToString(@"(00) 0000\-0000"));
            }
        }

        public ContactInfo() { }

        public ContactInfo(string phoneNumber, string email)
        {
            PhoneNumber = phoneNumber?.Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty).Trim();
            
            Email = email;
        }

        public void Update(string phoneNumber, string email)
        {
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
