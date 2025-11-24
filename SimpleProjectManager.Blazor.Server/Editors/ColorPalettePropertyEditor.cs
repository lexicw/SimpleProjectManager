using System;
using System.Drawing;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;

namespace SimpleProjectManager.Blazor.Server.Editors
{
    [PropertyEditor(typeof(Color), false)]
    public class ColorPalettePropertyEditor : BlazorPropertyEditorBase
    {
        public ColorPalettePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) { }

        public new ColorPaletteDropDownModel ComponentModel =>
            (ColorPaletteDropDownModel)base.ComponentModel;

        protected override IComponentModel CreateComponentModel()
        {
            var model = new ColorPaletteDropDownModel();

            model.ValueChanged = EventCallback.Factory.Create<string?>(this, value => {
                model.Value = value;
                OnControlValueChanged();
                WriteValue();
            });

            return model;
        }

        protected override void ReadValueCore()
        {
            base.ReadValueCore();

            Color? color = PropertyValue as Color?;
            if (PropertyValue is Color c)
                color = c;

            ComponentModel.Value = color.HasValue
                ? ColorTranslator.ToHtml(color.Value)
                : null;
        }

        protected override object GetControlValueCore()
        {
            var hex = ComponentModel.Value;
            if (string.IsNullOrEmpty(hex))
                return null!; // Color? null

            return ColorTranslator.FromHtml(hex);
        }

        protected override void ApplyReadOnly()
        {
            base.ApplyReadOnly();
        }
    }
}
