using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Mandara.ProductGUI.Calendars
{
    public partial class SelectControlBase : DevExpress.XtraEditors.XtraUserControl
    {
        #region Header

        public event CancelEventHandler ValueChanging;
        public event EventHandler ValueChanged;
        private List<object> items;
        private object currentValue;
        private int currentIndex;

        #endregion

        #region Constructors
        public SelectControlBase()
        {
            InitializeComponent();

            items = new List<object>();

            SetItems(new List<object>());
        }
        #endregion

        #region Static Members
        #endregion

        #region Public Methods
        public virtual void SetPreviousValue()
        {
            if (CurrentIndex == 0)
            {
                if (Cycle)
                {
                    CurrentIndex = Items.Count - 1;
                }
            }
            else
            {
                CurrentIndex = CurrentIndex - 1;
            }
        }


        public virtual void SetNextValue()
        {
            if (CurrentIndex == Items.Count - 1)
            {
                if (Cycle)
                {
                    CurrentIndex = 0;
                }
            }
            else
            {
                CurrentIndex = CurrentIndex + 1;
            }
        }
        #endregion

        #region Public Properties
        public object CurrentValue
        {
            get
            {
                return (currentIndex >= 0) ? Items[currentIndex] : null;
            }
            set { SetCurrenIndex(Items.IndexOf(value)); }
        }

        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                SetCurrenIndex(value);
            }
        }

        public bool Cycle { get; set; }
        #endregion

        #region Private Methods
        private void SetTextBoxValue()
        {
            valueTextEdit.Text = (CurrentValue == null) ? string.Empty : CurrentValue.ToString();
        }

        private void SetCurrenIndex(int newIndex)
        {
            if (currentIndex != newIndex)
            {
                var cancelEventArgs = new CancelEventArgs(false);
                OnValueChanging(cancelEventArgs);

                if (cancelEventArgs.Cancel == false)
                {
                    if ((newIndex < -1) || (newIndex == -1 && items.Count > 0) || (newIndex >= items.Count))
                    {
                        string message = string.Format("Current index={0} not valid for the Items.Count = {1}", newIndex, Items.Count);
                        throw new ArgumentException(message);
                    }

                    currentIndex = newIndex;
                    SetTextBoxValue();

                    OnValueChanged(new EventArgs());
                }
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Event Handlers
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            SetPreviousValue();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            SetNextValue();
        }
        #endregion

        #region Protected Members

        protected virtual void OnValueChanging(CancelEventArgs e)
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }
        protected void SetItems(IEnumerable items)
        {
            this.items.Clear();
            foreach (var each in items)
            {
                this.items.Add(each);
            }
            SetCurrenIndex((this.items.Count == 0) ? -1 : 0);
            SetTextBoxValue();
        }
        protected List<object> Items
        {
            get { return items; }
        }
        #endregion


















    }
}
