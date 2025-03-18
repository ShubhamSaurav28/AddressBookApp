using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IEmailServiceBL
    {
        public void SendEmail(string toEmail, string subject, string body);

    }
}
