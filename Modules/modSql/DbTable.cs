using System;

namespace COTES.ISTOK.Modules.Registrator
{
    class DbTable
    {
        private string name;
        private string fieldID;
        private string fieldName;

        private string separator;
        private string point;

        internal DbTable()
        {
            name = "";
            fieldID = fieldName = "";
            separator = "\\";
            point = ".";
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string FieldID
        {
            get { return fieldID; }
            set { fieldID = value; }
        }
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }
        public string Field
        {
            get
            {
                string tag;

                //if (!string.IsNullOrEmpty(fieldID)) tag = fieldID;
                //else
                    if (!string.IsNullOrEmpty(fieldName)) tag = fieldName;
                    else
                        return "";

                return EnumFields(tag);
            }
        }
        public string TableSeparator
        {
            get { return separator; }
            set { separator = value; }
        }
        public string TablePoint
        {
            get { return point; }
            set { point = value; }
        }
        protected string EnumFields(string field)
        {
            string tag;
            string[] arrItems = null;
            string[] arrSeparator = null;
            bool useCast = false;
            int i;

            if (field == null) throw new ArgumentNullException("null:field");

            tag = field;

            if (!string.IsNullOrEmpty(TableSeparator))
            {
                arrSeparator = new string[] { TableSeparator };
                arrItems = tag.Split(arrSeparator, StringSplitOptions.None);
            }
            else
            {
                if (tag.StartsWith(Name + TablePoint)) return tag;
                else return Name + TablePoint + tag;
            }

            if (arrItems.Length > 1) useCast = true;
            tag = "";

            for (i = 0; i < arrItems.Length; i++)
            {
                if (!useCast)
                {
                    if (i > 0) tag += ",";
                    if (arrItems[i].StartsWith(Name + TablePoint)) tag += arrItems[i];
                    else tag += Name + TablePoint + arrItems[i];
                }
                else
                {
                    if (i > 0) tag += "+'" + TableSeparator + "'+";
                    tag += "cast(";
                    if (arrItems[i].StartsWith(Name + TablePoint)) tag += arrItems[i];
                    else tag += Name + TablePoint + arrItems[i];
                    tag += " as varchar)";
                }
            }

            return tag;
        }
    }
}
