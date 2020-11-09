using System;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using EmailReaderWeb.Models;

[assembly: InternalsVisibleTo("EmailSenderWebTests")]
namespace EmailReaderWeb
{
    class Validator
    {
        public static bool ChekParams(Receiver receiver, Receiver hiddenReceiver, Letter letter)
        {
            //check email format for receiver
            if (CheckEmailFormat(receiver.EmailAddress))
            {
                string err = $"Receiver address: {receiver.EmailAddress } has format exception";
                receiver.Status = err;
                throw new Exception(err);
            }
            //check email format for hidden receiver, this value can be null
            if ((hiddenReceiver.EmailAddress != null))
            {
                if (CheckEmailFormat(hiddenReceiver.EmailAddress))
                {
                    string err = $"Hidden email address: {hiddenReceiver.EmailAddress} has format exception";
                    hiddenReceiver.Status = err;
                    throw new Exception(err);
                }
            }
            //check not null strings to send
            if (letter.Subject.Length < 1)
            {
                string err = $"subject: \"{letter.Subject}\" -- has format exception";
                letter.Status = err;
                throw new Exception(err);
            }
            if (letter.Text.Length < 1)
            {
                string err = $"Sending text: \"{letter.Text}\" -- has format exception";
                letter.Status = err;
                throw new Exception(err);
            }
            return true;
        }
        
        private static bool CheckEmailFormat(string email)
        {
            try
            {
                string address = new MailAddress(email).Address;
                email = email.Replace(" ", "");
            }
            catch
            {
                return true;
            }
            return false;
        } 
        
    }
}
