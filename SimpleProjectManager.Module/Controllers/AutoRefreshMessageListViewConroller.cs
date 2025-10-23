using DevExpress.ExpressApp;
using DevExpress.ExpressApp.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SimpleProjectManager.Module.Controllers
{
    public class AutoRefreshMessageListViewController : ViewController<ListView>
    {
        private System.Timers.Timer _timer;
        private SynchronizationContext _uiContext;

        private long _lastCount = -1;
        private byte[] _lastMaxRv = null;
        private int _isRunning = 0;

        private const int IntervalMs = 2000;

        public AutoRefreshMessageListViewController()
        {
            TargetObjectType = typeof(BusinessObjects.Message);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            _uiContext = SynchronizationContext.Current;

            _timer = new System.Timers.Timer(IntervalMs) { AutoReset = true, Enabled = true };
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref _isRunning, 1) == 1) return;

            Task.Run(() =>
            {
                bool didUpdates = false;

                try
                {
                    using var osProbe = Application.CreateObjectSpace(typeof(BusinessObjects.Message));
                    if (osProbe is EFCoreObjectSpace efProbe)
                    {
                        var db = efProbe.DbContext;

                        // 10 Minutes ago cutoff for "Attention" status
                        var cutoff = DateTime.Now.AddMinutes(-2);
                        var attentionStatus = db.Set<BusinessObjects.Message>()
                                       .Where(m => m.Status != BusinessObjects.Message.MessageStatuses.Attention
                                                && m.CreatedOn <= cutoff)
                                       .ToList();

                        if (attentionStatus.Count > 0)
                        {
                            foreach (var m in attentionStatus)
                                m.Status = BusinessObjects.Message.MessageStatuses.Attention;

                            db.SaveChanges();
                            didUpdates = true;
                        }

                        // Probe AFTER possible updates
                        var countNow = db.Set<BusinessObjects.Message>().AsNoTracking().LongCount();
                        var maxRvNow = db.Set<BusinessObjects.Message>()
                                         .AsNoTracking()
                                         .OrderByDescending(c => EF.Property<byte[]>(c, "RowVersion"))
                                         .Select(c => EF.Property<byte[]>(c, "RowVersion"))
                                         .FirstOrDefault();

                        bool countChanged = countNow != _lastCount;                          // insert/delete
                        bool rvChanged = !RowVersionEquals(maxRvNow, _lastMaxRv) || didUpdates; // updates or we changed rows

                        // Always baseline (prevents stale baselines blocking refresh later)
                        _lastCount = countNow;
                        _lastMaxRv = maxRvNow;

                        if (countChanged || rvChanged)
                        {
                            if (_uiContext != null)
                                _uiContext.Post(_ => ApplyRefresh(countChanged, rvChanged), null);
                            else
                                ApplyRefresh(countChanged, rvChanged);
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    Interlocked.Exchange(ref _isRunning, 0);
                }
            });
        }

        private void ApplyRefresh(bool countChanged, bool rvChanged)
        {
            if (View is not ListView lv || View.ObjectSpace == null) return;

            try
            {
                var os = View.ObjectSpace;

                if (!countChanged && rvChanged)
                {
                    if (os is EFCoreObjectSpace efosUpd)
                        efosUpd.DbContext.ChangeTracker.Clear();

                    os.Refresh();
                    View.Refresh();
                    return;
                }

                var selectedKeys = View.SelectedObjects.Cast<object>()
                    .Select(o => os.GetKeyValue(o))
                    .ToList();

                object focusedKey = View.CurrentObject != null ? os.GetKeyValue(View.CurrentObject) : null;

                if (os is EFCoreObjectSpace efos)
                    efos.DbContext.ChangeTracker.Clear();

                lv.CollectionSource.Reload();
                View.Refresh();
            }
            catch
            {
            }
        }

        private static bool RowVersionEquals(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            return true;
        }

        protected override void OnDeactivated()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= Timer_Elapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            _uiContext = null;
            base.OnDeactivated();
        }
    }
}
