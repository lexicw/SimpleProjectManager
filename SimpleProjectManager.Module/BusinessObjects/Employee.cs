using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public virtual String FirstName { get; set; }
        public virtual String LastName { get; set; }

        public virtual Color? Color { get; set; }
        public String FullName
        {
            get
            {
                return ObjectFormatter.Format("{FirstName} {LastName}",
            this, EmptyEntriesMode.RemoveDelimiterWhenEntryIsEmpty);
            }
        }

        public virtual IList<Message> AssignedMessages { get; set; } = new ObservableCollection<Message>();

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