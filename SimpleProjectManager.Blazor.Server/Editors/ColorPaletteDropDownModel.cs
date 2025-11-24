using System;
using DevExpress.ExpressApp.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;

namespace SimpleProjectManager.Blazor.Server.Editors
{
    public class ColorPaletteDropDownModel : ComponentModelBase
    {
        public string? Value
        {
            get => GetPropertyValue<string?>();
            set => SetPropertyValue(value);
        }

        public EventCallback<string?> ValueChanged
        {
            get => GetPropertyValue<EventCallback<string?>>();
            set => SetPropertyValue(value);
        }

        public override Type ComponentType => typeof(Components.ColorPaletteDropDown);
    }
}
