using System;
using System.Collections.Generic;
using System.Text;

namespace TarkovRaidCalculatorClient.Dto
{
    public class AuthOut
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
