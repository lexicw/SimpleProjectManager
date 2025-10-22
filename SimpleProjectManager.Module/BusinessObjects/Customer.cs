using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace SimpleProjectManager.Module.BusinessObjects
{
    // Use this attribute to place a navigation item that corresponds to the entity class in the specified navigation group.
    [DefaultClassOptions]
    [NavigationItem("Marketing")]

    // Inherit your entity classes from the BaseObject class to support CRUD operations for the declared objects automatically.
    public class Customer : IXafEntityObject, IObjectSpaceLink
    {
        protected IObjectSpace ObjectSpace;

        [System.ComponentModel.DataAnnotations.Key]
        [Browsable(false)]
        public virtual int id { get; set; }
        public virtual String FirstName { get; set; }

        public virtual String LastName { get; set; }

        // Use this attribute to specify the maximum number of characters that the property's editor can contain.
        [FieldSize(255)]
        public virtual String Email { get; set; }

        public virtual String Company { get; set; }

        public virtual String Occupation { get; set; }

        public virtual IList<Testimonial> Testimonials { get; set; } =
        new ObservableCollection<Testimonial>();

        public String FullName
        {
            get
            {
                return ObjectFormatter.Format("{FirstName} {LastName} ({Company})",
            this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);
            }
        }

        // Use this attribute to show or hide a column with the property's values in a List View.
        [VisibleInListView(false)]

        // Use this attribute to specify dimensions of an image property editor.
        [ImageEditor(ListViewImageEditorCustomHeight = 75, DetailViewImageEditorFixedHeight = 150)]

        public virtual MediaDataObject Photo { get; set; }

        [Timestamp]
        public virtual byte[] RowVersion { get; set; }

        public virtual MessageStatuses Status { get; set; }

        public enum MessageStatuses
        {
            New = 0,
            Pending = 1,
            [XafDisplayName("Awaiting Response")]
            Waiting = 2
        }

        public void OnSaving()
        {

        }

        public void OnCreated() 
        {
        }

        public void OnLoaded() { }

        IObjectSpace IObjectSpaceLink.ObjectSpace
        {
            get => ObjectSpace;
            set => ObjectSpace = value;
        }

    }
}