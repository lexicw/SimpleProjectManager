using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Blazor.Editors;
using Microsoft.AspNetCore.Components;

namespace SimpleProjectManager.Blazor.Server.Editors
{
    [PropertyEditor(typeof(string), "HtmlStringPropertyEditor", false)]
    public class HtmlStringPropertyEditor : BlazorPropertyEditorBase
    {
        public HtmlStringPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected override RenderFragment CreateViewComponentCore(object dataContext)
        {
            string htmlString = string.Empty;

            if (MemberInfo != null && dataContext != null)
            {
                htmlString = MemberInfo.GetValue(dataContext) as string ?? string.Empty;
            }

            return builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddContent(1, (MarkupString)htmlString); // render as HTML
                builder.CloseElement();
            };
        }

        protected override object GetControlValueCore() => PropertyValue;
    }
}
