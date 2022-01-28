using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TarkovRaidCalculatorClient
{
    interface INet
    {
        Task<Auth> AuthAsync(string email, string hash);
        Auth Auth(string email, string hash);
        Task<Dto.RefreshAuthOut> RefreshTokenAsync(string token, string refreshToken);
        Task SendImage(byte[] imageData,string token, string fileName, string raid = "pre");
    }
}
