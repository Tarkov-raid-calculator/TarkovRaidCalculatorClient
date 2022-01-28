using System;
using System.Collections.Generic;
using System.Text;

namespace TarkovRaidCalculatorClient.Dto
{
    public class RefreshAuthOut
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
