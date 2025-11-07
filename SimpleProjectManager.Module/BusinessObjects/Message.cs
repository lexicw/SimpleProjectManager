using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.Xpo;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.ExpressApp.Editors;

namespace SimpleProjectManager.Module.BusinessObjects
{
    [Appearance("HighlightUrgentMessages", AppearanceItemType = "ViewItem", TargetItems = "*", Context = "ListView", Criteria = "Status = 3", FontColor = "Red", FontStyle = DevExpress.Drawing.DXFontStyle.Bold, BackColor = "255,240,240", Priority = 100)]
    [Appearance("HighLightNewMessages", AppearanceItemType = "ViewItem", TargetItems = "*",
        Context = "ListView", Criteria = "Status = 0", FontColor = "Dodgerblue", FontStyle = DevExpress.Drawing.DXFontStyle.Bold, BackColor = "240, 247, 255")]
    [DefaultClassOptions]
    // Inherit your entity classes from the BaseObject class to support CRUD operations for the declared objects automatically.
    public class Message : IXafEntityObject, IObjectSpaceLink
    {
        protected IObjectSpace ObjectSpace;

        [System.ComponentModel.DataAnnotations.Key]
        [Browsable(false)]
        public virtual int id { get; set; }
        public virtual String Subject { get; set; }

        [FieldSize(255)]
        public virtual String Email { get; set; }

        public virtual String Company { get; set; }

        [Timestamp]
        public virtual byte[] RowVersion { get; set; }

        public virtual MessageStatuses Status { get; set; }

        public virtual DateTime CreatedOn { get; set; }

        [EditorAlias("HtmlEmailEditor")]
        [FieldSize(FieldSizeAttribute.Unlimited)]
        public virtual string MessageBody { get; set; }

        public virtual IList<Employee> AssignedTo { get; set; } = new ObservableCollection<Employee>();

        public enum MessageStatuses
        {
            New = 0,
            Pending = 1,
            [XafDisplayName("Awaiting Response")]
            Waiting = 2,
            [XafDisplayName("Needs Attention")]
            Attention = 3
        }

        public void OnSaving()
        {

        }

        public void OnCreated()
        {
            CreatedOn = DateTime.Now;
        }

        public void OnLoaded() { }

        IObjectSpace IObjectSpaceLink.ObjectSpace
        {
            get => ObjectSpace;
            set => ObjectSpace = value;
        }

    }
}
