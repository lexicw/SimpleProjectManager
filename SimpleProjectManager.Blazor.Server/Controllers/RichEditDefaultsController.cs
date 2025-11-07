using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Office.Blazor.Editors;
using DevExpress.ExpressApp.Office.Blazor.Components.Models; // DxRichEditModel
using DevExpress.Blazor.RichEdit;                             // ViewType
using Microsoft.AspNetCore.Components;                        // EventCallback (optional)

namespace SimpleProjectManager.Module.Blazor.Controllers
{
    public class RichEditDefaultsController : ViewController<DetailView>
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            View.CustomizeViewItemControl<RichTextPropertyEditor>(this, editor => {
                var richEditModel = (DxRichEditModel)editor.ComponentModel;

                // Default to Simple View (instead of Print Layout)
                richEditModel.ViewType = DevExpress.Blazor.RichEdit.ViewType.Simple;

                // Hide the rulers
                richEditModel.HorizontalRulerVisible = false;

                // (Optional) enforce again after the document loads
                var prevLoaded = richEditModel.DocumentLoaded;
                richEditModel.DocumentLoaded = EventCallback.Factory.Create<Document>(this, async doc => {
                    await prevLoaded.InvokeAsync(doc);
                    // Make sure the view sticks even if the user toggled it before
                    richEditModel.ViewType = DevExpress.Blazor.RichEdit.ViewType.Simple;
                    richEditModel.HorizontalRulerVisible = false;
                });
            });
        }
    }
}
