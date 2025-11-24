// MySolution.Blazor.Server/Editors/HtmlEditorPropertyEditor.cs
using System;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;
using DevExpress.Blazor;

namespace SimpleProjectManager.Blazor.Server.Editors
{
    public static class EditorAliases
    {
        public const string HtmlEmailEditor = "HtmlEmailEditor";
    }

    [PropertyEditor(typeof(string), false)]
    public class HtmlEditorPropertyEditor : BlazorPropertyEditorBase
    {
        public HtmlEditorPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) { }

        public override HtmlEditorModel ComponentModel => (HtmlEditorModel)base.ComponentModel;

        protected override IComponentModel CreateComponentModel()
        {
            var model = new HtmlEditorModel
            {
                BindMarkupMode = HtmlEditorBindMarkupMode.OnDelayedInput
            };
            // Optional: make "delayed" feel snappy
            model.SetAttribute(nameof(DxHtmlEditor.InputDelay), 150);

            model.MarkupChanged = EventCallback.Factory.Create<string>(this, value =>
            {
                model.Markup = value;
                OnControlValueChanged();
                WriteValue();
            });
            return model;
        }

        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            ComponentModel.Markup = (string)PropertyValue;
        }

        protected override object GetControlValueCore() => ComponentModel.Markup;

        protected override void ApplyReadOnly()
        {
            base.ApplyReadOnly();
            ComponentModel?.SetAttribute("readonly", !AllowEdit);
        }
    }
}
