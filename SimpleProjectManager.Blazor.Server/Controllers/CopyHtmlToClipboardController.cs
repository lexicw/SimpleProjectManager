using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using SimpleProjectManager.Module.BusinessObjects;

namespace SimpleProjectManager.Module.Blazor.Controllers
{
    //public class CopyHtmlToClipboardController
    //    : ObjectViewController<DetailView, Message>
    //{
    //    private readonly SimpleAction copyHtmlAction;
    //    private IJSRuntime js;

    //    public CopyHtmlToClipboardController()
    //    {
    //        // Scope: DetailView of Message objects
    //        TargetViewNesting = Nesting.Root; // remove if you also want it on nested DetailViews

    //        copyHtmlAction = new SimpleAction(this, "CopyHtmlToClipboard", PredefinedCategory.Edit)
    //        {
    //            Caption = "Copy HTML",
    //            ImageName = "Actions_Copy",
    //            PaintStyle = ActionItemPaintStyle.CaptionAndImage
    //        };
    //        copyHtmlAction.Execute += CopyHtmlAction_Execute;
    //    }

    //    protected override void OnActivated()
    //    {
    //        base.OnActivated();
    //        js = Application.ServiceProvider.GetRequiredService<IJSRuntime>();

    //        // If you want to hide the action when there's no body:
    //        var msg = View.CurrentObject as Message;
    //        copyHtmlAction.Active["HasBody"] = !string.IsNullOrWhiteSpace(msg?.MessageBody);
    //    }


    //    private async void CopyHtmlAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    //    {
    //        var msg = View.CurrentObject as Message;
    //        var html = msg?.MessageBody ?? string.Empty;

    //        if (string.IsNullOrWhiteSpace(html))
    //        {
    //            await js.InvokeVoidAsync("eval", "alert('Nothing to copy: MessageBody is empty.');");
    //            return;
    //        }

    //        await js.InvokeVoidAsync("navigator.clipboard.writeText", html);
    //        await js.InvokeVoidAsync("eval", "setTimeout(()=>alert('HTML copied to clipboard.'),0);");
    //    }

    //    protected override void OnDeactivated()
    //    {
    //        copyHtmlAction.Execute -= CopyHtmlAction_Execute;
    //        base.OnDeactivated();
    //    }
    //}
}
