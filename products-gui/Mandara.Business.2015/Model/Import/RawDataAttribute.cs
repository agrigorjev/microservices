using System;

namespace Mandara.Import
{
    public class RawDataAttribute : Attribute
    {
        private string _fieldName;

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }
    }
}