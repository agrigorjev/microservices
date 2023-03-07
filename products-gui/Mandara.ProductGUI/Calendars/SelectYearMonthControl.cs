using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Mandara.ProductGUI.Calendars
{
    public partial class SelectYearMonthControl : DevExpress.XtraEditors.XtraUserControl
    {

        #region Header

        private int currentMonthIndex;
        public event CancelEventHandler ValueChanging;
        public event EventHandler ValueChanged;

        #endregion

        #region Constructors
        public SelectYearMonthControl()
        {
            InitializeComponent();

            selectMonthControl.BorderStyle = BorderStyle.None;
            selectYearControl.BorderStyle = BorderStyle.None;

            currentMonthIndex = selectMonthControl.SelectedMonthIndex;

            selectMonthControl.ValueChanging += new CancelEventHandler(selectMonthControl_ValueChanging);
            selectMonthControl.ValueChanged += new EventHandler(selectMonthControl_ValueChanged);

            selectYearControl.ValueChanging += new CancelEventHandler(selectYearControl_ValueChanging);
            selectYearControl.ValueChanged += new EventHandler(selectYearControl_ValueChanged);
        }
        #endregion

        #region Static Members
        #endregion

        #region Public Methods
        #endregion

        #region Public Properties

        public int SelectedMonthIndex
        {
            get { return selectMonthControl.SelectedMonthIndex; }
        }

        public int SelectedYear
        {
            get { return selectYearControl.SelectedYear; }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Event Handlers
        void selectYearControl_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        void selectYearControl_ValueChanging(object sender, CancelEventArgs e)
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }

        void selectMonthControl_ValueChanged(object sender, EventArgs e)
        {
            int prevCurrentIndex = currentMonthIndex;
            currentMonthIndex = selectMonthControl.SelectedMonthIndex;

            if (currentMonthIndex == 12 && prevCurrentIndex == 1)
            {
                selectYearControl.SetPreviousValue();
            }
            else if (currentMonthIndex == 1 && prevCurrentIndex == 12)
            {
                selectYearControl.SetNextValue();
            }
            else
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, e);
                }
            }
        }

        void selectMonthControl_ValueChanging(object sender, CancelEventArgs e)
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }

        #endregion

        #region Protected Members
        #endregion





    }
}
