using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HUFLITCOFFEE.ViewModels;

namespace HUFLITCOFFEE.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}