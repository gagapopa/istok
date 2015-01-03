using System;
using System.Collections.Generic;
using System.Threading;
using NLog;

namespace COTES.ISTOK.Block
{
    /// <summary>
    /// Класс, отвечающий за загрузку/выгрузку и работоспособность каналов
    /// </summary>
    class DataBaseMaintance
    {
        private ChannelManager channelManager;
        private ValueBuffer valBuffer = null;
        private DALManager dalManager = null;
        Logger log = LogManager.GetCurrentClassLogger();

        Timer timer;

        public DataBaseMaintance(ChannelManager channelManager, DALManager dalManager, ValueBuffer valBuffer)
        {
            this.channelManager = channelManager;
            this.dalManager = dalManager;
            this.valBuffer = valBuffer;
        }

        #region Обслуживание базы данных
    
        /// <summary>
        /// Запустить обслуживание базы
        /// </summary>
        public void StartMaintenance()
        {
            TimeSpan timeout=TimeSpan.FromMinutes(NSI.DEFAULTSAVEMEMPARAM);
            if (timer==null)
            {
                timer = new Timer(dbMaintenanceMethod, null, timeout, timeout);
            }
            else
            {
                timer.Change(timeout, timeout);
            }
        }

        /// <summary>
        /// Остановить обслуживание базы
        /// </summary>
        public void StopMaintenance()
        {
            if (timer!=null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);

            }
        }

        volatile bool inProgress;

        DateTime lastCleanTime = DateTime.Now, lastFlushBufferTime = DateTime.MinValue;


        /// <summary>
        /// Метод производящий обслуживание базы
        /// </summary>
        private void dbMaintenanceMethod(Object state)
        {
            TimeSpan bufferFlushInterval = TimeSpan.FromMinutes(NSI.DEFAULTSAVEMEMPARAM);
            TimeSpan sleepInterval = TimeSpan.FromMinutes(1);
            DateTime nowTime;

            if (inProgress)
            {
                return;
            }
            try
            {
                inProgress = true;
                //Thread.Sleep(bufferFlushInterval);
                nowTime = DateTime.Now;
                if (!bufferFlushInterval.Equals(TimeSpan.Zero)
                    && (nowTime - lastFlushBufferTime) > bufferFlushInterval)
                {
                    valBuffer.FlushAll();
                    lastFlushBufferTime = nowTime;
                }

                ScheduleReg removerInterval = new ScheduleReg(BlockSettings.Instance.Maintenance.Schedule);

                if (BlockSettings.Instance.Maintenance.Enabled
                    && removerInterval != null
                    && removerInterval.CheckTimes(lastCleanTime, nowTime))
                {
                    log.Info("Запуск обслуживание базы");
                    int count = (int)BlockSettings.Instance.Maintenance.ValueDeleteCount;
                    int proccessed;
                    int storedays;
                    proccessed = dalManager.CleanExcessValues(count);
                    if (log.IsDebugEnabled && proccessed > 0)
                    {
                        log.Debug("Удалено {0} лишних строк", proccessed);
                    }

                    // get all parameters
                    List<ParameterItem> lstParameters = new List<ParameterItem>();
                    foreach (var channel in channelManager.GetLoadedChannels())
                    {
                        lstParameters.AddRange(channel.Parameters);
                    }

                    for (int i = 0; i < lstParameters.Count && (count -= proccessed) >= 0; i++)
                    {
                        try
                        {
                            // remove crossed packages
                            proccessed = dalManager.CleanBadPackages(count, lstParameters[i].Idnum);
                            if (log.IsDebugEnabled && proccessed > 0)
                            {
                                log.Debug("Удалено {0} пересекающихся пачек для параметра {1}", proccessed, lstParameters[i].Name);
                            }
                            count -= proccessed;

                            storedays = Math.Max(lstParameters[i].Store_days, (int)BlockSettings.Instance.Maintenance.KeepValuesDays);
                            proccessed = dalManager.CleanOldValues(count, lstParameters[i].Idnum, storedays);
                            if (log.IsDebugEnabled && proccessed > 0)
                            {
                                log.Debug("Удалено {0} устаревших строк для параметра {1} (настройка хранить дней: {2})", proccessed, lstParameters[i].Name, storedays);
                            }
                        }
                        catch (ThreadInterruptedException) { throw; }
                        catch (Exception exc)
                        {
                            log.ErrorException(String.Format("Ошибка при обслуживании базы. Параметр '{0}'", lstParameters[i].Name), exc);
                        }
                    }
                    lastCleanTime = nowTime;
                    log.Info("Обслуживание базы данных завершено. Затрачено времени: {0}", DateTime.Now - nowTime);
                }
                else
                {
#if DEBUG
                    log.Info("Обслуживание базы пропущено '{0}'", BlockSettings.Instance.Maintenance.Schedule); 
#endif
                }
            }
            catch (ThreadInterruptedException) { }
            catch (Exception exc)
            {
                log.ErrorException("Ошибка при обслуживании базы", exc);
            }
            finally
            {
                inProgress = false;
            }
        }
        #endregion
    }
}