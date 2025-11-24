using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;


namespace SimpleProjectManager.Module.BusinessObjects
{
    [DefaultProperty(nameof(FullName))]
    [DefaultClassOptions]
    public class Employee : IXafEntityObject, IObjectSpaceLink
    {
        protected IObjectSpace ObjectSpace;

        [System.ComponentModel.DataAnnotations.Key]
        [Browsable(false)]
        public virtual int id { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

        [VisibleInDetailView(true)]
        [VisibleInListView(false)]
        public virtual Color? Color { get; set; }

        [NotMapped]
        [EditorAlias("HtmlStringPropertyEditor")]
        [VisibleInDetailView(false)]
        [VisibleInListView(true)]
        [VisibleInLookupListView(false)]
        public string ColorHex
        {
            get
            {
                if (Color == null)
                    return "<span style='color: gray;'>#000000</span>";

                var c = Color.Value;
                var hex = $"#{c.R:X2}{c.G:X2}{c.B:X2}";

                return $@"
            <div style='display:flex;align-items:center;gap:6px;'>
                <div style='width:16px;height:16px;border-radius:3px;border:1px solid #ccc;background-color:{hex};'></div>
                <span>{hex}</span>
            </div>";
            }
        }


        public string FullName
        {
            get
            {
                return ObjectFormatter.Format("{FirstName} {LastName}",
                    this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);
            }
        }

        public virtual IList<Message> AssignedMessages { get; set; } = new ObservableCollection<Message>();

        public void OnSaving() { }
        public void OnCreated() { }
        public void OnLoaded() { }

        IObjectSpace IObjectSpaceLink.ObjectSpace
        {
            get => ObjectSpace;
            set => ObjectSpace = value;
        }
    }
}