using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using DevExpress.ExpressApp.EFCore;
using SimpleProjectManager.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleProjectManager.Blazor.Server.Controllers
{
    /// <summary>
    /// Continuously inserts random Message rows via XAF ObjectSpace until canceled.
    /// It creates a fresh DI scope and ObjectSpace on each iteration.
    /// </summary>
    public static class MessageSimulator
    {
        private static readonly string[] Subjects = {
            "Meeting Follow-up","Budget Review","Proposal Update","Client Feedback",
            "New Requirements","Design Review","Timeline Discussion","Next Steps",
            "Internal Memo","Project Status","Action Items","Weekly Report","Today's Meeting",
            "Launch Plan","Feature Discussion","Performance Review","Testing Feedback",
            "Server Migration","Contract Renewal","Team Alignment","Marketing Strategy",
            "Design Mockups","Client Call Notes","Budget Revision","Q4 Planning",
            "UX Review","Development Roadmap","QA Summary","Release Candidate","Sprint Retrospective"
        };

        private static readonly string[] Companies = {
            "Globex Inc","Stark Industries","Wayne Enterprises","Wonka Labs",
            "Umbrella Corp","Hooli","Initech","Aperture Science","Soylent Systems",
            "Cyberdyne Systems","Massive Dynamic","Tyrell Corporation","Vehement Capital Partners",
            "Blue Sun Corp","Virtucon","Oscorp Industries","LexCorp","Abstergo Industries",
            "Black Mesa Research","ACME Corporation"
        };

        private static readonly string[] Emails = {
            "daniel.morris@globex.com","victoria.green@starkindustries.com",
            "bruce.wayne@wayne.com","charlie@wonka.com","claire.redfield@umbrella.com",
            "gavin@hooli.com","samir@initech.com","chell@aperture.com","frank@soylent.com",
            "miles.dyson@cyberdyne.com","olivia.dunham@massivedynamic.com","rachel.tyrell@tyrell.com",
            "jane.doe@vehementcapital.com","malcolm.reynolds@bluesun.com","austin.powell@virtucon.com",
            "harry.osborn@oscorp.com","lex.luthor@lexcorp.com","desmond.miles@abstergo.com",
            "gordon.freeman@blackmesa.com","wile.e.coyote@acme.com"
        };

        // Lightweight Html Email templates for testing purposes
        private static readonly string[] HtmlTemplates =
        {
            @"<!DOCTYPE html>
            <html>
              <head>
                <meta charset=""utf-8"">
                <meta name=""x-apple-disable-message-reformatting"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                <title>{0}</title>
              </head>
              <body style=""margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;line-height:1.5;"">
                <div style=""max-width:600px;margin:0 auto;padding:16px;"">
                  <h2 style=""margin:0 0 12px 0;font-size:20px;"">{0}</h2>
                  <p style=""margin:0 0 12px 0;"">Hi team,</p>
                  <p style=""margin:0 0 12px 0;"">Following up on our recent discussion about <strong>{1}</strong>. Please review the points below before our next sync:</p>
                  <ul style=""margin:0 0 12px 20px;padding:0;"">
                    <li>Confirm revised scope and timeline.</li>
                    <li>Validate budget adjustments.</li>
                    <li>Share feedback on the latest mockups.</li>
                  </ul>
                  <p style=""margin:0 0 12px 0;"">If everything looks good, I'll prepare the action items and circulate them.</p>
                  <p style=""margin:0 0 12px 0;"">Thanks,<br>{2}<br><a href=""mailto:{3}"">{3}</a></p>
                  <hr style=""border:none;border-top:1px solid #ddd;margin:16px 0;"">
                  <p style=""font-size:12px;color:#777;margin:0;"">Sent on {4}</p>
                </div>
              </body>
            </html>",

            @"<!DOCTYPE html>
            <html>
              <head>
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                <title>{0}</title>
              </head>
              <body style=""margin:0;padding:0;background:#f7f7f7;font-family:Arial,Helvetica,sans-serif;"">
                <div style=""max-width:640px;margin:0 auto;background:#ffffff;padding:20px;"">
                  <h1 style=""font-size:18px;margin:0 0 10px 0;"">{0}</h1>
                  <p style=""margin:0 0 12px 0;"">Hello,</p>
                  <p style=""margin:0 0 12px 0;"">Quick update on the <strong>{1}</strong> workstream. We’re on track, but we need sign-off on the revised proposal.</p>
                  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse:collapse;margin:12px 0;"">
                    <tr>
                      <td style=""padding:8px;border:1px solid #eee;"">Deliverable</td>
                      <td style=""padding:8px;border:1px solid #eee;"">ETA</td>
                      <td style=""padding:8px;border:1px solid #eee;"">Owner</td>
                    </tr>
                    <tr>
                      <td style=""padding:8px;border:1px solid #eee;"">Revised Proposal</td>
                      <td style=""padding:8px;border:1px solid #eee;"">EOW</td>
                      <td style=""padding:8px;border:1px solid #eee;"">{2}</td>
                    </tr>
                  </table>
                  <p style=""margin:0 0 12px 0;"">
                    Please <a href=""mailto:{3}"">reply</a> with any blockers.
                  </p>
                  <p style=""margin:0;"">Best,<br>{2}</p>
                  <p style=""font-size:12px;color:#777;margin:16px 0 0 0;"">Sent on {4}</p>
                </div>
              </body>
            </html>",

            @"<!DOCTYPE html>
            <html>
              <head>
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                <title>{0}</title>
              </head>
              <body style=""margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;"">
                <div style=""max-width:620px;margin:0 auto;padding:18px;"">
                  <p style=""margin:0 0 12px 0;"">Subject: <strong>{0}</strong></p>
                  <p style=""margin:0 0 12px 0;"">Team, the latest feedback from <strong>{1}</strong> has been incorporated.</p>
                  <ol style=""margin:0 0 12px 20px;padding:0;"">
                    <li>Updated the timeline to reflect testing.</li>
                    <li>Captured client notes and action items.</li>
                    <li>Prepared materials for the next review.</li>
                  </ol>
                  <p style=""margin:0 0 12px 0;"">Regards,<br>{2} &lt;<a href=""mailto:{3}"">{3}</a>&gt;</p>
                  <hr style=""border:none;border-top:1px solid #e5e5e5;margin:16px 0;"">
                  <p style=""font-size:12px;color:#777;margin:0;"">{4}</p>
                </div>
              </body>
            </html>"
        };

        public static async Task StartAsync(IServiceScopeFactory scopeFactory, CancellationToken token)
        {
            var rand = new Random();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var osFactory = scope.ServiceProvider.GetRequiredService<IObjectSpaceFactory>();
                        using var os = osFactory.CreateObjectSpace(typeof(Message));

                        var msg = os.CreateObject<Message>();
                        msg.Subject = (rand.Next(0, 3) == 0 ? "Re: " : "") + Subjects[rand.Next(Subjects.Length)];
                        msg.Company = Companies[rand.Next(Companies.Length)];
                        msg.Email = Emails[rand.Next(Emails.Length)];
                        msg.Status = Message.MessageStatuses.New;
                        msg.CreatedOn = DateTime.Now;

                        // NEW: Generate HTML MessageBody
                        var displayName = ToDisplayName(msg.Email);
                        var when = DateTime.Now.ToString("f"); // e.g., Monday, November 3, 2025 1:23 PM
                        var template = HtmlTemplates[rand.Next(HtmlTemplates.Length)];
                        msg.MessageBody = string.Format(template, msg.Subject, msg.Company, displayName, msg.Email, when);

                        os.CommitChanges(); // Persist the insert
                    }

                    await Task.Delay(rand.Next(5000, 10000), token);
                }
                catch (TaskCanceledException) { break; }
                catch
                {
                    // short backoff on any transient error, then continue
                    try { await Task.Delay(1000, token); } catch { break; }
                }
            }
        }

        private static string ToDisplayName(string email)
        {
            try
            {
                var local = email?.Split('@').FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(local)) return email ?? "";
                // Replace separators with spaces, title-case each token
                var friendly = local.Replace('.', ' ').Replace('_', ' ').Replace('-', ' ');
                var ti = CultureInfo.CurrentCulture.TextInfo;
                friendly = string.Join(" ",
                    friendly.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => ti.ToTitleCase(w)));
                return friendly;
            }
            catch { return email ?? ""; }
        }
    }
}
