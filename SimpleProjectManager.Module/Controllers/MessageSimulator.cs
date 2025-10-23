using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            "jane.doe@vehementcapital.com","malcolm.reynolds@bluesun.com","austin.powers@virtucon.com",
            "harry.osborn@oscorp.com","lex.luthor@lexcorp.com","desmond.miles@abstergo.com",
            "gordon.freeman@blackmesa.com","wile.e.coyote@acme.com"
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
                        // Use the overload your XAF version requires (Type-based):
                        using var os = osFactory.CreateObjectSpace(typeof(Message));

                        var msg = os.CreateObject<Message>();
                        msg.Subject = (rand.Next(0, 3) == 0 ? "Re: " : "") + Subjects[rand.Next(Subjects.Length)];
                        msg.Company = Companies[rand.Next(Companies.Length)];
                        msg.Email = Emails[rand.Next(Emails.Length)];
                        msg.Status = Message.MessageStatuses.New;

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
    }
}
