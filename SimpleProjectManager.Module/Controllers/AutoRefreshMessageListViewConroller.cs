using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

            _timer = new System.Timers.Timer(IntervalMs)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref _isRunning, 1) == 1) return;

            Task.Run(() =>
            {
                bool didUpdates = false;
                List<object> changedKeys = null;

                try
                {
                    using var osProbe = Application.CreateObjectSpace(typeof(BusinessObjects.Message));
                    if (osProbe is EFCoreObjectSpace efProbe)
                    {
                        var db = efProbe.DbContext;

                        // 2-minute cutoff in this sample
                        var cutoff = DateTime.Now.AddMinutes(-2);

                        // Track entities we need to update
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

                            // Capture primary keys of changed rows
                            changedKeys = attentionStatus
                                .Select(m => (object)osProbe.GetKeyValue(m))
                                .Where(k => k != null)
                                .Distinct()
                                .ToList();
                        }

                        // Probe AFTER possible updates
                        var countNow = db.Set<BusinessObjects.Message>().AsNoTracking().LongCount();
                        var maxRvNow = db.Set<BusinessObjects.Message>()
                                         .AsNoTracking()
                                         .OrderByDescending(c => EF.Property<byte[]>(c, "RowVersion"))
                                         .Select(c => EF.Property<byte[]>(c, "RowVersion"))
                                         .FirstOrDefault();

                        bool countChanged = countNow != _lastCount;                            // insert/delete
                        bool rvChanged = !RowVersionEquals(maxRvNow, _lastMaxRv) || didUpdates; // updates or we changed rows

                        // Baseline
                        _lastCount = countNow;
                        _lastMaxRv = maxRvNow;

                        if (countChanged || rvChanged)
                        {
                            if (_uiContext != null)
                                _uiContext.Post(_ => ApplyRefresh(countChanged, rvChanged, changedKeys), null);
                            else
                                ApplyRefresh(countChanged, rvChanged, changedKeys);
                        }
                    }
                }
                catch
                {
                    // log if needed
                }
                finally
                {
                    Interlocked.Exchange(ref _isRunning, 0);
                }
            });
        }

        private void ApplyRefresh(bool countChanged, bool rvChanged, IList<object> changedKeys = null)
        {
            if (View is not ListView lv || View.ObjectSpace == null) return;

            try
            {
                var os = View.ObjectSpace;

                // Lightweight path: values changed but no add/remove -> keep selection
                if (!countChanged && rvChanged)
                {
                    // Clear EF tracking before reloading objects
                    if (os is EFCoreObjectSpace efosUpd)
                        efosUpd.DbContext.ChangeTracker.Clear();

                    if (changedKeys != null && changedKeys.Count > 0)
                    {
                        // Map keys to current instances in THIS ObjectSpace, then reload those objects in place
                        var toReload = changedKeys
                            .Select(k => os.GetObjectByKey(lv.ObjectTypeInfo.Type, k))
                            .Where(o => o != null)
                            .ToList();

                        foreach (var obj in toReload)
                            os.ReloadObject(obj); // keeps instances -> preserves selection
                    }
                    else
                    {
                        // Fallback if we don't know which changed; still avoid reloading the CollectionSource
                        os.Refresh();
                    }

                    View.Refresh(); // repaint; instances unchanged so selection & focus stay
                    return;
                }

                // === Full reload path (add/remove occurred) ===

                // 1) Capture selection & focused object keys (before reload)
                var selectedKeys = View.SelectedObjects.Cast<object>()
                    .Select(o => os.GetKeyValue(o))
                    .Where(k => k != null)
                    .ToList();

                var focusedKey = View.CurrentObject != null ? os.GetKeyValue(View.CurrentObject) : null;

                // 2) Clear EF tracking and reload the collection
                if (os is EFCoreObjectSpace efos)
                    efos.DbContext.ChangeTracker.Clear();

                if (lv.Editor is DxGridListEditor ge)
                {
                    ge.RestoreFocusedObject = true;
                    // If your version has it, you can also enable:
                    // ge.RestoreSelectedObjects = true;
                }

                lv.CollectionSource.Reload();
                View.Refresh();

                // 3) Restore selection & focus by keys
                if (lv.Editor is DxGridListEditor gridEditor)
                {
                    var selectedObjs = selectedKeys
                        .Select(k => os.GetObjectByKey(lv.ObjectTypeInfo.Type, k))
                        .Where(o => o != null)
                        .ToList();

                    //gridEditor.UnselectAll();

                    // Portable re-select
                    foreach (var o in selectedObjs)
                        gridEditor.SelectObject(o);

                    if (focusedKey != null)
                    {
                        var focusedObj = os.GetObjectByKey(lv.ObjectTypeInfo.Type, focusedKey);
                        if (focusedObj != null)
                            View.CurrentObject = focusedObj;
                    }
                }
            }
            catch
            {
                // log if needed
            }
        }

        private static bool RowVersionEquals(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a == null || b == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
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
