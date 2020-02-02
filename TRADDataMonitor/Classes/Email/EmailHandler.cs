using System;
using System.Collections.Generic;
using System.Text;
// added
using System.Net.Sockets;
using System.IO;
using System.Net.Mail;
using System.Net;
using MailKit.Net.Imap;
using System.Threading;
using MailKit;
using MailKit.Search;

namespace TRADDataMonitor
{
    public class EmailHandler
    {
        #region private variables
        // Private variables used for sending SMTP mail from an SMTP server (in this case gmail from Google's SMTP server)
        string recipientEmailAddress, senderEmailAddress, senderEmailPassword, senderEmailSmtpAddress, senderEmailSmtpPort;
        #endregion

        #region constructor
        // Constructor: parameters passed from the public properties when instantiated in the MainWindowViewModel class
        public EmailHandler(string recipientEmailAddress, string senderEmailAddress, string senderEmailPassword, string senderSmtpAddress, string senderSmptPort)
        {
            this.recipientEmailAddress = recipientEmailAddress;
            this.senderEmailAddress = senderEmailAddress;
            this.senderEmailPassword = senderEmailPassword;
            this.senderEmailSmtpAddress = senderSmtpAddress;
            this.senderEmailSmtpPort = senderSmptPort;
        }
        #endregion

        #region public methods
        // Public methods for validating and sending emails

        #region validation methods
        // Checks the validity of a given email address
        // Code from: https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
        public bool IsValidEmail(string email)
        {
            try
            {
                // Creates a new MailAddress object from the parameter email
                var addr = new MailAddress(email);
                // Checks if MailAddress property address matches the parameter email and then returns the result
                return addr.Address == email;
            }
            // If the parameter email does not match the built in regex for email then return false
            catch
            {
                return false;
            }
        }

        // Checks the validity of a given SMTP address and port number
        // Code from: https://stackoverflow.com/questions/955431/how-to-validate-smtp-server
        public bool IsValidSmtp(string hostAddress, int portNumber)
        {
            bool isValid = false;
            try
            {
                // Creates a new SMTP client using TCP
                TcpClient smtpTest = new TcpClient();
                // Attempts to connect to the SMTP server
                smtpTest.Connect(hostAddress, portNumber);

                // If it does connect...
                if (smtpTest.Connected)
                {
                    NetworkStream ns = smtpTest.GetStream();
                    StreamReader sr = new StreamReader(ns);
                    // Check if the response to the request contains the code '220'
                    if (sr.ReadLine().Contains("220"))
                    {
                        // If it does then the SMTP address is valid
                        isValid = true;
                    }
                    smtpTest.Close();
                }
            }
            // Otherwise the SMTP address is not valid
            catch
            {
                isValid = false;
                return isValid;
            }
            return isValid;
        }
        #endregion

        #region send and read mail methods
        /* Sends an email to a given email address for testing purposes, when the threshold on a phidget sensor is broken, or when the threshold on a phidget 
         * sensor is fixed */
        public void SendEmailAlert(double minThresh, double maxThresh, string hubName, string sensor, int portID, double val, string emailType)
        {
            string subject = "debug";
            string message = "debug";

            // Checks the type of email to be sent and sets the subject and message of the email appropriately
            if (emailType == "test")
            {
                subject = $"Test Alert";
                message = $"Automated Alert: This is a test alert. If you are recieving this then the email alert system is configured correctly";
            }
            else if (emailType == "broken")
            {
                subject = $"Threshold Broken Alert: {sensor}";
                message = $"Automated Alert: Data from the {sensor} sensor connected to port {portID} on hub {hubName} exited the allowable range of {minThresh} to {maxThresh} with a value of {val}.";
            }
            else
            {
                subject = $"Threshold Fixed Alert: {sensor}";
                message = $"Automated Alert: Data from the {sensor} sensor connected to port {portID} on hub {hubName} re-entered the allowable range of {minThresh} to {maxThresh} with a value of {val}.";
            }

            // Creates and SMTP client to send the email
            SmtpClient smtp01 = new SmtpClient(senderEmailSmtpAddress, Convert.ToInt32(senderEmailSmtpPort));
            NetworkCredential netCred = new NetworkCredential(senderEmailAddress, senderEmailPassword);

            // Sends the email via SMTP to the given recipient from the given sender with the correct subject and message
            System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            smtp01.Credentials = netCred;
            smtp01.EnableSsl = true;
            MailMessage msg = new MailMessage(senderEmailAddress, recipientEmailAddress, subject, message);
            smtp01.Send(msg);
        }

        // Overload for SendEmailAlert
        // Sends an email to a given email address when the threshold on a VOC sensor is broken or when the threshold on a VOC sensor is fixed
        // The VOC is hosted through a web server and not a phidget hub, hence the need for an overloaded method
        public void SendEmailAlert(double minThresh, double maxThresh, string sensor, double val, string emailType)
        {
            string subject;
            string message;

            // Checks the type of email to be sent and sets the subject and message of the email appropriately
            if (emailType == "fixed")
            {
                subject = $"Threshold Broken Alert: {sensor}";
                message = $"Automated Alert: Data from the {sensor} sensor exited the allowable range of {minThresh} to {maxThresh} with a value of {val}.";
            }
            else
            {
                subject = $"Threshold Fixed Alert: {sensor}";
                message = $"Automated Alert: Data from the {sensor} sensor re-entered the allowable range of {minThresh} to {maxThresh} with a value of {val}.";
            }

            SmtpClient smtp01 = new SmtpClient(senderEmailSmtpAddress, Convert.ToInt32(senderEmailSmtpPort));
            NetworkCredential netCred = new NetworkCredential(senderEmailAddress, senderEmailPassword);

            System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            smtp01.Credentials = netCred;
            smtp01.EnableSsl = true;
            MailMessage msg = new MailMessage(senderEmailAddress, recipientEmailAddress, subject, message);
            smtp01.Send(msg);
        }

        // Overload for SendEmailAlert
        // Sends an email to a given email address when the threshold on a VOC sensor is broken or when the threshold on a VOC sensor is fixed
        // The VOC is hosted through a web server and not a phidget hub, hence the need for an overloaded method
        public void SendEmailAlert(double distanceThreshold, string sensor, double lat, double lng, double val)
        {
            string subject;
            string message;

            subject = $"Threshold Broken Alert: {sensor}";
            message = $"Automated Alert: Data from the {sensor} sensor exited the allowable radius of {distanceThreshold} km with a value of {val} km. The current latitude and longitiude values are: {lat}, {lng}.";

            // Creates and SMTP client to send the email
            SmtpClient smtp01 = new SmtpClient(senderEmailSmtpAddress, Convert.ToInt32(senderEmailSmtpPort));
            NetworkCredential netCred = new NetworkCredential(senderEmailAddress, senderEmailPassword);

            // Sends the email via SMTP to the given recipient from the given sender with the correct subject and message
            System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            smtp01.Credentials = netCred;
            smtp01.EnableSsl = true;
            MailMessage msg = new MailMessage(senderEmailAddress, recipientEmailAddress, subject, message);
            smtp01.Send(msg);
        }

        /* Method to read emails from the inbox of a given email address, filter them for the dateTime the alert was sent, and looks for a reply email
         * to the respective alert email */
        // Code from: https://stackoverflow.com/questions/545724/using-c-sharp-net-libraries-to-check-for-imap-messages-from-gmail-servers)
        public string RetrieveEmailReply(DateTime alertSent, string alertSubject)
        {
            try
            {
                string ret = "";

                // Creates an new IMAP client
                // IMAP is used for parsing emails while SMTP is used for sending them
                using (var client = new ImapClient())
                {
                    using (var cancel = new CancellationTokenSource())
                    {
                        client.Connect("imap.gmail.com", 993, true, cancel.Token);

                        client.Authenticate(this.senderEmailAddress, this.senderEmailPassword, cancel.Token);

                        var inbox = client.Inbox;
                        inbox.Open(FolderAccess.ReadOnly, cancel.Token);

                        // Downloads each message based on the message index
                        for (int i = 0; i < inbox.Count; i++)
                        {
                            var message = inbox.GetMessage(i, cancel.Token);
                            Console.WriteLine("Subject: {0}", message.Subject);
                        }

                        // Searches for the correct reply using a LINQ query
                        var query = SearchQuery.DeliveredAfter(alertSent)
                            .And(SearchQuery.SubjectContains(alertSubject));

                        // Sets the return string to the value of the found message
                        foreach (var uid in inbox.Search(query, cancel.Token))
                        {
                            var message = inbox.GetMessage(uid, cancel.Token);
                            ret = message.Body.ToString();
                        }

                        client.Disconnect(true, cancel.Token);

                        return ret;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion
    }
}
