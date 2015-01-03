using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace COTES.ISTOK.Assignment
{
    /// <summary>
    /// Управление ревизиями
    /// </summary>
    class RevisionManager
    {
        MyDBdata dbData;

        RevisionInfo[] revisions;

        public RevisionManager(MyDBdata dbData)
        {
            this.dbData = dbData;
        }

        /// <summary>
        /// Получить все ревизии
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RevisionInfo> GetRevisions()
        {
            const String selectQuery = "SELECT [id], [time_from], [brief], [comment] FROM [revision] ORDER BY [time_from]";

            if (revisions == null)
            {
                List<RevisionInfo> revisionInfo = new List<RevisionInfo>();

                revisionInfo.Add(RevisionInfo.Default);

                using (DataTable dt = dbData.ExecSQL_toTable(selectQuery, null))
                {
                    foreach (DataRow dataRow in dt.Rows)
                    {
                        revisionInfo.Add(new RevisionInfo
                        {
                            ID = Convert.ToInt32(dataRow["id"]),
                            Time = Convert.ToDateTime(dataRow["time_from"]),
                            Brief = Convert.ToString(dataRow["brief"]),
                            Comment = Convert.ToString(dataRow["comment"])
                        });
                    }
                }
                revisions = revisionInfo.ToArray();
            }
            return revisions;
        }

        public IEnumerable<RevisionInfo> GetRevisions(DateTime startTime, DateTime endTime)
        {
            const String selectQuery = "SELECT [id], [time_from], [brief], [comment] FROM [revision] WHERE [time_from] > @start_time AND [time_from] < @end_time";
            const String selectFirstQuery = "SELECT TOP 1 [id], [time_from], [brief], [comment] FROM [revision] WHERE [time_from] < @start_time ORDER BY [time_from] DESC";

            List<RevisionInfo> revisionInfo = new List<RevisionInfo>();

            DB_Parameters a = new DB_Parameters();
            a.Add("@start_time", DbType.DateTime, startTime);
            a.Add("@end_time", DbType.DateTime, endTime);

            using (DataTable dt = dbData.ExecSQL_toTable(selectFirstQuery, a))
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    revisionInfo.Add(new RevisionInfo
                    {
                        ID = Convert.ToInt32(dataRow["id"]),
                        Time = Convert.ToDateTime(dataRow["time_from"]),
                        Brief = Convert.ToString(dataRow["brief"]),
                        Comment = Convert.ToString(dataRow["comment"])
                    });
                }
                if (revisionInfo.Count == 0)
                    revisionInfo.Add(RevisionInfo.Default);
            }

            using (DataTable dt = dbData.ExecSQL_toTable(selectQuery, a))
            {
                foreach (DataRow dataRow in dt.Rows)
                {
                    revisionInfo.Add(new RevisionInfo
                    {
                        ID = Convert.ToInt32(dataRow["id"]),
                        Time = Convert.ToDateTime(dataRow["time_from"]),
                        Brief = Convert.ToString(dataRow["brief"]),
                        Comment = Convert.ToString(dataRow["comment"])
                    });
                }
            }

            return revisionInfo.ToArray();
        }

        /// <summary>
        /// Изменить или добавить ревизию
        /// </summary>
        /// <param name="revision"></param>
        public void Update(RevisionInfo revision)
        {
            const String updateQuery = "UPDATE [revision] SET [time_from] = @time_from, [brief] = @brief, [comment] = @comment WHERE id = @id";
            const String insertQuery = "INSERT INTO [revision] ([time_from], [brief], [comment]) VALUES (@time_from, @brief, @comment)";

            DB_Parameters dbp = new DB_Parameters();
            dbp.Add("time_from", DbType.DateTime, revision.Time);
            dbp.Add("brief", DbType.String, revision.Brief);
            dbp.Add("comment", DbType.String, revision.Comment);

            if (revision.ID != 0)
            {
                dbp.Add("id", DbType.Int32, revision.ID);
                dbData.ExecSQL(updateQuery, dbp);
            }
            else
                dbData.ExecSQL(insertQuery, dbp);

            revisions = null;
        }

        /// <summary>
        /// Удалить ревизию
        /// </summary>
        /// <param name="revision"></param>
        public void Delete(RevisionInfo revision)
        {
            const String deleteQuery = "DELETE FROM [revision] WHERE id = @id";

            DB_Parameters dbp = new DB_Parameters();

            dbp.Add("id", DbType.Int32, revision.ID);
            dbData.ExecSQL(deleteQuery, dbp);

            revisions = null;
        }

        /// <summary>
        /// Получить ревизию по ид
        /// </summary>
        /// <param name="revisionID">ИД ревизии</param>
        /// <returns>Ревизия</returns>
        public RevisionInfo GetRevision(int revisionID)
        {
            return GetRevisions().FirstOrDefault(r => r.ID == revisionID);
        }

        public RevisionInfo GetRevision(DateTime time)
        {
            return (from r in GetRevisions() 
                    where r.Time <= time 
                    orderby r.Time descending 
                    select r).FirstOrDefault();
        }
    }
}
