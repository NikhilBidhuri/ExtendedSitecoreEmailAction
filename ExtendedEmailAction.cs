using System;
using System.Net.Mail;
using System.Web;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Security;
using Sitecore.Security.Accounts;
using Sitecore.Workflows;
using Sitecore.Workflows.Simple;

namespace CodeWithNikhil.EmailAction
{
    public class EnhancedEmailAction 
    {
        public void Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            ProcessorItem processorItem = args.ProcessorItem;
            if (processorItem != null)
            {
                try
                {
                    Item innerItem = processorItem.InnerItem;
                    string fullPath = innerItem.Paths.FullPath;
                    string from = GetText(innerItem, "from", args);
                    string to = GetText(innerItem, "to", args);
                    string host = GetText(innerItem, "mail server", args);
                    string subject = GetText(innerItem, "subject", args);
                    string body = GetText(innerItem, "message", args);
                    Error.Assert(to.Length > 0, "The 'To' field is not specified in the mail action item: " + fullPath);
                    Error.Assert(from.Length > 0, "The 'From' field is not specified in the mail action item: " + fullPath);
                    Error.Assert(subject.Length > 0,
                    "The 'Subject' field is not specified in the mail action item: " + fullPath);
                    Error.Assert(host.Length > 0,
                    "The 'Mail server' field is not specified in the mail action item: " + fullPath);
                    MailMessage message = new MailMessage(from, to)
                    {
                        Subject = subject,
                        Body = body
                    };
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = host;
                    
                    smtp.UseDefaultCredentials = false;
                    var emailConfig =
                     Sitecore.Configuration.Factory.GetDatabase("master").GetItem("Your-Item-GUID-Here"); ;
                        UserName = emailConfig?.Fields["UserName"]?.Value,
                        Password = emailConfig?.Fields["Password"]?.Value,
                        Port = emailConfig?.Fields["Port"]?.Value,
                    smtp.Credentials = new System.Net.NetworkCredential(emailSettings.UserName, emailSettings.Password);
                    smtp.Port = Port; 
                    smtp.EnableSsl = true;
                    message.Body = message?.Body?.Replace("$host$", host); 
                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error(ex.Message, this);
                }
            }
        }
    }
}