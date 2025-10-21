using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.EFCore;
using Microsoft.EntityFrameworkCore;

namespace SimpleProjectManager.Module.Controllers
{
    public class AutoRefreshCustomerListViewController : ViewController<ListView>
    {
        private System.Timers.Timer _timer;
        private SynchronizationContext _uiContext;

        private long _lastCount = -1;
        private byte[] _lastMaxRv = null;
        private int _isRunning = 0;

        public AutoRefreshCustomerListViewController()
        {
            TargetObjectType = typeof(BusinessObjects.Customer);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            _uiContext = SynchronizationContext.Current;

            _timer = new System.Timers.Timer(5000) { AutoReset = true, Enabled = true };
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // prevent overlapping probes
            if (Interlocked.Exchange(ref _isRunning, 1) == 1) return;

            Task.Run(() => {
                try
                {
                    // Use a short-lived ObjectSpace so we don't touch the UI-bound context
                    using var os = Application.CreateObjectSpace(typeof(BusinessObjects.Customer));
                    if (os is EFCoreObjectSpace efos)
                    {
                        var db = efos.DbContext;

                        // Cheap aggregates (no tracking)
                        var count = db.Set<BusinessObjects.Customer>().AsNoTracking().LongCount();

                        // Get MAX(RowVersion) by ordering; FirstOrDefault can be null if table empty
                        var maxRv = db.Set<BusinessObjects.Customer>()
                            .AsNoTracking()
                            .OrderByDescending(c => EF.Property<byte[]>(c, "RowVersion"))
                            .Select(c => EF.Property<byte[]>(c, "RowVersion"))
                            .FirstOrDefault();

                        bool changed =
                            count != _lastCount ||
                            !RowVersionEquals(maxRv, _lastMaxRv);

                        if (changed)
                        {
                            _lastCount = count;
                            _lastMaxRv = maxRv;

                            void Reload()
                            {
                                if (View is ListView lv)
                                {
                                    try
                                    {
                                        lv.CollectionSource.Reload();
                                        View.Refresh();
                                    }
                                    catch { }
                                }
                            }
                            if (_uiContext != null) _uiContext.Post(_ => Reload(), null);
                            else Reload();
                        }
                    }
                }
                catch
                {
                    // ignore transient errors during navigation/shutdown
                }
                finally
                {
                    Interlocked.Exchange(ref _isRunning, 0);
                }
            });
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
