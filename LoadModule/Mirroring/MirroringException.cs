using System;

namespace COTES.ISTOK.Block.MirroringManager
{
    /// <summary>
    ///     Класс иключения. Не несет ни какой дополнительной
    ///     информации. Предназначем исключетельно для разграничения
    ///     ошибок возникших в процессе работы менеждера зеркалирования
    ///     и прочих ошибок программы.
    /// </summary>
    public class MirroringException : Exception
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="inner">
        ///     Внутренние исключение возникшее при работе с зеркалированием.
        /// </param>
        public MirroringException(Exception inner)
            :base(inner.Message, inner)
        { }

        public MirroringException(string message)
            : base(message)
        { }
    }
}
