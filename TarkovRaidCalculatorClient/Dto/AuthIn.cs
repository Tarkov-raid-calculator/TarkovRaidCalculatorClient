using System;
using System.Collections.Generic;
using System.Text;

namespace TarkovRaidCalculatorClient.Dto
{
    public class AuthIn
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
