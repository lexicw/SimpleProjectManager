using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Blazor.Editors.Models;

namespace SimpleProjectManager.Module.Controllers
{
    // Scope to a specific BO if you want; remove TargetObjectType to make it global
    public class DisableSelectAllController : ObjectViewController<ListView, BusinessObjects.Message>
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            if (View?.Editor is DxGridListEditor editor)
            {
                // Hide the “Select All” checkbox in the selection column header
                editor.GridSelectionColumnModel.AllowSelectAll = false;
            }
        }
    }
}
