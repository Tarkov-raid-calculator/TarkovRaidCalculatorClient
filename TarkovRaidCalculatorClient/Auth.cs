using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TarkovRaidCalculatorClient
{
    public class Auth
    {
        private readonly INet _net;
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public Auth()
        {
            _net = new NetHttps();
            Task.Run(async () => {
                while (true)
                {
                    //await Task.Delay(1000 * 60 * 60 * 5);
                    await Task.Delay(1000);
                    Dto.RefreshAuthOut refresh = await _net.RefreshTokenAsync(Token, RefreshToken);
                    Token = refresh.Token;
                    RefreshToken = refresh.RefreshToken;
                }
            });
        }
    }
}