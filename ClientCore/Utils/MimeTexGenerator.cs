using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using COTES.ISTOK.ASC;

namespace COTES.ISTOK.ClientCore.Utils
{
    /// <summary>
    /// Генератор изображений кодов параметров, работающий в отдельном потоке
    /// </summary>
    public class MimeTexGenerator
    {
        private static MimeTexGenerator singlton = new MimeTexGenerator();

        /// <summary>
        /// Синглтон генератора
        /// </summary>
        public static MimeTexGenerator Singlton { get { return singlton; } }

        private MimeTexWrapper mimeTex = new MimeTexWrapper();

        /// <summary>
        /// Запросить изображение кода параметра
        /// </summary>
        /// <param name="parameter">Параметр</param>
        /// <param name="callBack">Калбэк через который будет передано изображение парамтера</param>
        public void GetImage(ParameterNode parameter, Action<ParameterNode, Image> callBack)
        {
                updateMimeTexQueue.Enqueue(new QueueItem(parameter, callBack));
                StartMimeTexGenerator();
        }

        class QueueItem
        {
            public ParameterNode Parameter { get; set; }
            public Action<ParameterNode, Image> NotifyAction { get; set; }
            public Image CodeImage { get; set; }

            public QueueItem(ParameterNode parameter,Action<ParameterNode, Image> notifyAction)
            {
                this.Parameter = parameter;
                this.NotifyAction = notifyAction;
            }
        }

        /// <summary>
        /// Параметры для которых требуется сгенерировать картинки
        /// </summary>
        Queue<QueueItem> updateMimeTexQueue = new Queue<QueueItem>();

        bool mimeTexGeneratorStarted;

        /// <summary>
        /// Запустить генерацию картинок кодов функции
        /// </summary>
        void StartMimeTexGenerator()
        {
            if (!mimeTexGeneratorStarted)
            {
                mimeTexGeneratorStarted = true;
                ThreadPool.QueueUserWorkItem(MimeTexGenerate);
            }
        }

        /// <summary>
        /// Синхронизация нескольких потоков генерации картинок
        /// </summary>
        static Object mimeSync = new Object();

        /// <summary>
        /// Генерация картинок в отдельном потоке - 1 на все открытые юнитпровайдеры
        /// </summary>
        /// <param name="state"></param>
        void MimeTexGenerate(Object state)
        {
            System.Drawing.Image im;
            QueueItem queueItem;
            ParameterNode paramNode;

            lock (mimeSync)
            {
                try
                {
                    while (updateMimeTexQueue.Count > 0)
                    {
                        try
                        {
                            queueItem = updateMimeTexQueue.Dequeue();
                            paramNode = queueItem.Parameter;
                            im = mimeTex.EquationToImage(paramNode.Code);
                            //if (im == null) im = Program.MainForm.Icons.Images[((int)paramNode.Typ).ToString()];
                            queueItem.CodeImage = im;
                            ThreadPool.QueueUserWorkItem(Notify, queueItem);
                        }
                        catch { }
                    }
                }
                finally
                {
                    mimeTexGeneratorStarted = false;
                }
            }
        }

        /// <summary>
        /// Вызвать калбэк для передачи сгенерированного изображения
        /// </summary>
        /// <param name="state"></param>
        void Notify(Object state)
        {
            try
            {
                QueueItem queueItem;

                if ((queueItem = state as QueueItem)!=null)
                    queueItem.NotifyAction(queueItem.Parameter, queueItem.CodeImage); 
            }
            catch { }
        }
    }
}
