using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using System.Collections.ObjectModel;

namespace SimpleProjectManager.Module.BusinessObjects
{
    // Use this attribute to place a navigation item that corresponds to the entity class in the specified navigation group.
    [NavigationItem("Marketing")]

    // Inherit your entity classes from the BaseObject class to support CRUD operations for the declared objects automatically.
    public class Customer : BaseObject
    {
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

    }
}