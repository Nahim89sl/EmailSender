using NUnit.Framework;
using EmailReaderWeb;
using EmailReaderWeb.Models;
using System;

namespace EmailSenderWebTests
{
    
    public class ValidatorTests
    {
        [Test]
        public void ChekParams_RightValues()
        {
            var receiver = new Receiver(){EmailAddress = "hello@mail.com"};
            var letter = new Letter()
            {
                Subject = "This is test status of this letter",
                Text = "This is text of this mail"
            };
            Assert.IsTrue(Validator.ChekParams(receiver, receiver, letter));
        }

        [Test]
        public void ChekParams_WrongEmail()
        {
            var receiver = new Receiver() { EmailAddress = "hellomail.com" };
            var letter = new Letter()
            {
                Subject = "This is test status of this letter",
                Text = "This is text of this mail"
            };
            var ex = Assert.Throws<Exception>(() => Validator.ChekParams(receiver, receiver, letter));
            Assert.That(ex.Message, Is.EqualTo($"Receiver address: {receiver.EmailAddress } has format exception"));
        }

    }
}
