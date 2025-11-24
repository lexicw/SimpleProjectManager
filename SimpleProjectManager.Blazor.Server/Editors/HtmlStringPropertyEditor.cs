using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Blazor.Editors;
using Microsoft.AspNetCore.Components;
using DevExpress.ExpressApp.Blazor.Components.Models;

namespace SimpleProjectManager.Blazor.Server.Editors
{
    [PropertyEditor(typeof(string), "HtmlStringPropertyEditor", false)]
    public class HtmlStringPropertyEditor : BlazorPropertyEditorBase
    {
        public HtmlStringPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        public override StaticTextComponentModel ComponentModel
            => (StaticTextComponentModel)base.ComponentModel;

        protected override IComponentModel CreateComponentModel()
        {
            return new StaticTextComponentModel
            {
                UseMarkupString = true
            };
        }

        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            ComponentModel.Text = (string)PropertyValue ?? string.Empty;
        }

        protected override object GetControlValueCore() => PropertyValue;

        protected override void ApplyReadOnly()
        {
            base.ApplyReadOnly();
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
                // render as HTML
                builder.AddContent(1, (MarkupString)htmlString);
                builder.CloseElement();
            };
        }
    }
}
