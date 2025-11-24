using System.Linq; // for OfType / FirstOrDefault
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using SimpleProjectManager.Module.BusinessObjects;

// MailKit/MimeKit aliases
using MailKitSmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKitSecure = MailKit.Security.SecureSocketOptions;
using MimeMessage = MimeKit.MimeMessage;
using MailboxAddress = MimeKit.MailboxAddress;
using MimeMultipart = MimeKit.Multipart;
using MimeTextPart = MimeKit.TextPart;
using DevExpress.ExpressApp.Templates;

namespace SimpleProjectManager.Module.Blazor.Controllers
{
    public class SendMessageEmailController : ViewController
    {
        private const string SendEmailActionId = "SendMessageViaSmtp";
        private bool _subscribed;

        public SendMessageEmailController()
        {
            TargetViewType = ViewType.DetailView;
            TargetObjectType = typeof(Message);
            TargetViewId = "Message_DetailView";

            // Create the action (local/scoped is fine)
            var sendEmail = new SimpleAction(this, SendEmailActionId, PredefinedCategory.View)
            {
                Caption = "Send Email (SMTP)",
                ImageName = "Actions_Send",
                PaintStyle = ActionItemPaintStyle.CaptionAndImage,
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject
            };
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Subscribe when view/frame is active
            var action = Actions.OfType<SimpleAction>().FirstOrDefault(a => a.Id == SendEmailActionId);
            if (action != null && !_subscribed)
            {
                action.Execute += SendEmail_Execute;
                _subscribed = true;
            }
        }

        protected override void OnDeactivated()
        {
            // Cleanly unsubscribe when the view/frame deactivates
            var action = Actions.OfType<SimpleAction>().FirstOrDefault(a => a.Id == SendEmailActionId);
            if (action != null && _subscribed)
            {
                action.Execute -= SendEmail_Execute;
                _subscribed = false;
            }
            base.OnDeactivated();
        }

        private async void SendEmail_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Application.ShowViewStrategy.ShowMessage("Sending Email...", InformationType.Info, 2500, InformationPosition.Bottom);

            if (ObjectSpace?.IsModified == true)
                ObjectSpace.CommitChanges();

            var current = View.CurrentObject as Message;
            if (current == null)
            {
                Application.ShowViewStrategy.ShowMessage("No current Message.", InformationType.Warning, 2000, InformationPosition.Bottom);
                return;
            }

            var to = current.Email ?? "";
            var subject = current.Subject ?? "";
            var htmlBody = current.MessageBody ?? "";

            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(htmlBody))
            {
                Application.ShowViewStrategy.ShowMessage("Fill Email, Subject, and MessageBody first.", InformationType.Warning, 3000, InformationPosition.Bottom);
                return;
            }

            const string host = "localhost";
            const int port = 25;
            const string fromAddr = "lwheeler@theamegroup.com";
            const string fromName = "Lexi Wheeler";

            try
            {
                using var client = new MailKitSmtpClient();
                await client.ConnectAsync(host, port, MailKitSecure.None);

                var mime = new MimeMessage();
                mime.From.Add(new MailboxAddress(fromName, fromAddr));
                mime.To.Add(MailboxAddress.Parse(to));
                mime.Subject = subject;

                var plain = new MimeTextPart("plain") { Text = $"Subject: {subject}\n(HTML content available)" };
                var html = new MimeTextPart("html") { Text = htmlBody };
                mime.Body = new MimeMultipart("alternative") { plain, html };

                await client.SendAsync(mime);
                await client.DisconnectAsync(true);

                current.Status = Message.MessageStatuses.Waiting;
                if (ObjectSpace?.IsModified == true)
                    ObjectSpace.CommitChanges();

                Application.ShowViewStrategy.ShowMessage("Sent to Papercut.", InformationType.Success, 2500, InformationPosition.Bottom);
            }
            catch (System.Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"SMTP error: {ex.Message}", InformationType.Error, 5000, InformationPosition.Bottom);
            }
        }
    }
}
