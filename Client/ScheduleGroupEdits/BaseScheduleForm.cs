using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using COTES.ISTOK.ClientCore;

namespace COTES.ISTOK.Client
{
    partial class BaseScheduleForm : BaseAsyncWorkForm
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        //protected RemoteDataService current_data_service = RemoteDataService.Instance;

        public BaseScheduleForm(StructureProvider strucProvider)
            : base(strucProvider)
        {
            InitializeComponent();
            ImageList il = new ImageList();
            //il.Images.Add(Properties.Resources.schedule_file);
            listViewSchedules.SmallImageList = il;
            listViewSchedules.LargeImageList = il;
        }

        protected async void LoadSchedule()
        {
            try
            {
                //var watcher = 
                var sh = await Task.Factory.StartNew(() =>
                    strucProvider.Session.GetSchedules());

                Action<Schedule> dlg =
                    new Action<Schedule>(AddScheduleToListView);

                //watcher.AddValueRecivedHandler((object x) =>
                //{
                //    try
                //    {
                //        Schedule[] schedules = x as Schedule[];

                //        foreach (var it in schedules)
                //            this.Invoke(dlg, it);
                //    }
                //    catch (Exception exp)
                //    {
                //        this.ShowError(exp);
                //    }
                //});

                //RunWatcher(watcher);

                foreach (var it in sh)
                    this.Invoke(dlg, it);
            }
            catch (Exception exp)
            {
                log.WarnException("Ошибка при просмотре элементов расписания.", exp);

                ShowError(exp);
            }
        }

        protected void AddScheduleToListView(Schedule schedule)
        {
            listViewSchedules.BeginUpdate();
            listViewSchedules.Items.Add(new ListViewItem()
            {
                Name = schedule.Name,
                Text = schedule.Name,
                Tag = schedule.Id,
                ImageIndex = 0
            });
            listViewSchedules.EndUpdate();
        }
    }
}
