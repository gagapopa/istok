namespace COTES.ISTOK.Modules.Registrator
{
    class DbValuesTable : DbTable
    {
        private string fieldTimeName;
        private string fieldTagID;
        private string fieldTagName;
        private string fieldQualityID;
        private string fieldQualityName;

        internal DbValuesTable()
        {
            fieldTagName = fieldTagID = "";
            fieldQualityID = fieldQualityName = "";
        }

        public string FieldTimeName
        {
            get { return fieldTimeName; }
            set { fieldTimeName = value; }
        }
        public string FieldTime
        {
            get
            {
                string tag;

                if (!string.IsNullOrEmpty(fieldTimeName)) tag = fieldTimeName;
                else
                    return "";

                return EnumFields(tag);
            }
        }
        public string FieldTagID
        {
            get { return fieldTagID; }
            set { fieldTagID = value; }
        }
        public string FieldTagName
        {
            get { return fieldTagName; }
            set { fieldTagName = value; }
        }
        public string FieldTag
        {
            get
            {
                string tag;

                if (!string.IsNullOrEmpty(fieldTagID)) tag = fieldTagID;
                else
                    if (!string.IsNullOrEmpty(fieldTagName)) tag = fieldTagName;
                    else
                        return "";

                return EnumFields(tag);
            }
        }
        public string FieldQualityID
        {
            get { return fieldQualityID; }
            set { fieldQualityID = value; }
        }
        public string FieldQualityName
        {
            get { return fieldQualityName; }
            set { fieldQualityName = value; }
        }
        public string FieldQuality
        {
            get
            {
                if (!string.IsNullOrEmpty(fieldQualityID)) return EnumFields(fieldQualityID);
                else
                    if (!string.IsNullOrEmpty(fieldQualityName)) return EnumFields(fieldQualityName);
                    else
                        return "";
            }
        }
    }
}
