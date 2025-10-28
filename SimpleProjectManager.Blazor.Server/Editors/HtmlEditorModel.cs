// MySolution.Blazor.Server/Editors/HtmlEditorModel.cs
using DevExpress.ExpressApp.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;
using DevExpress.Blazor;

namespace SimpleProjectManager.Blazor.Server.Editors
{
    public class HtmlEditorModel : ComponentModelBase
    {
        public string Markup
        {
            get => GetPropertyValue<string>();
            set => SetPropertyValue(value);
        }

        public EventCallback<string> MarkupChanged
        {
            get => GetPropertyValue<EventCallback<string>>();
            set => SetPropertyValue(value);
        }

        // Correct enum type:
        public HtmlEditorBindMarkupMode BindMarkupMode
        {
            get => GetPropertyValue<HtmlEditorBindMarkupMode>();
            set => SetPropertyValue(value);
        }

        public override Type ComponentType => typeof(DxHtmlEditor);
    }
}


