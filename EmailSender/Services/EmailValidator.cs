using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace EmailSender.Services
{
    public static class EmailValidator
    {
        public static string Validate(string emilAddress)
        {
            try
            {
                string addr = emilAddress.Replace(" ", "");
                //check the last symbol . / \
                var symb = addr.Substring(addr.Length - 1);
                if ((symb == ".") || (symb == "/") || (symb == "\\"))
                {
                    addr = addr.Substring(0, addr.Length - 1);
                }
                addr = new MailAddress(addr).Address;
                return addr.ToLower();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
