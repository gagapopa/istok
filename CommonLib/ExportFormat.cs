
namespace COTES.ISTOK
{
    /// <summary>
    /// Формат файла используемый для экспорта и импорта
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        /// Использовать XML
        /// </summary>
        XML,
        /// <summary>
        /// Использовать сжатый XML, расширение файла - xml.gz
        /// </summary>
        ZippedXML,
        /// <summary>
        /// Excel Format
        /// </summary>
        Excel,
        /// <summary>
        /// Word Xml Format
        /// </summary>
        WordX
    }
}
