using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualBasic;

namespace Traces.Web.Models
{
    public class TracesGroupModel
    {
        private readonly IList<DateTime> _dates;

        private readonly List<TraceItemModel> _traces;

        private readonly List<TraceItemModel> _overdueItems;

        private readonly Dictionary<DateTime, List<TraceItemModel>> _dayItems;

        private readonly Dictionary<DateTime, List<TraceItemModel>> _monthItems;

        public TracesGroupModel()
        {
            _dates = Enumerable.Range(0, 7)
                .Select(offset => DateTime.Today.AddDays(offset).Date).ToArray();

            _traces = new List<TraceItemModel>();
            _overdueItems = new List<TraceItemModel>();
            _dayItems = new Dictionary<DateTime, List<TraceItemModel>>();
            _monthItems = new Dictionary<DateTime, List<TraceItemModel>>();
        }

        public ReadOnlyCollection<TraceItemModel> Traces => _traces.AsReadOnly();

        public ReadOnlyCollection<TraceItemModel> OverdueItems => _overdueItems.AsReadOnly();

        public IReadOnlyDictionary<DateTime, ReadOnlyCollection<TraceItemModel>> DayItems =>
            _dayItems.ToDictionary(k => k.Key, v => v.Value.AsReadOnly());

        public IReadOnlyDictionary<DateTime, ReadOnlyCollection<TraceItemModel>> MonthItems =>
            _monthItems.ToDictionary(k => k.Key, v => v.Value.AsReadOnly());

        public void Add(TraceItemModel trace)
        {
            _traces.Add(trace);

            GetCorrectGroupList(trace.DueDate).Add(trace);
        }

        public void Remove(TraceItemModel trace)
        {
            _traces.Remove(trace);

            GetCorrectGroupList(trace.DueDate).Remove(trace);
        }

        public void Replace(TraceItemModel trace, ReplaceTraceItemModel replaceTraceItemModel)
        {
            var oldDueDate = trace.DueDate;
            var newDueDate = replaceTraceItemModel.DueDate;

            trace.Title = replaceTraceItemModel.Title;
            trace.Description = replaceTraceItemModel.Description;
            trace.DueDate = replaceTraceItemModel.DueDate;

            if (oldDueDate != newDueDate)
            {
                GetCorrectGroupList(oldDueDate).Remove(trace);
                var list = GetCorrectGroupList(newDueDate);
                list.Add(trace);
                if (IsMonthGroup(newDueDate))
                {
                    SortList(list);
                }
            }
        }

        public void Sort()
        {
            SortList(_overdueItems);
            foreach (var monthItemsValue in _monthItems.Values)
            {
                SortList(monthItemsValue);
            }
        }

        public void Clear()
        {
            _traces.Clear();
            _overdueItems.Clear();
            _dayItems.Clear();
            _monthItems.Clear();

            foreach (var dateTime in _dates)
            {
                _dayItems.Add(dateTime, new List<TraceItemModel>());
            }
        }

        private void SortList(List<TraceItemModel> list)
        {
            list.Sort((a, b) => a.DueDate.CompareTo(b.DueDate));
        }

        private List<TraceItemModel> GetCorrectGroupList(DateTime dateTime)
        {
            var date = dateTime;
            if (date < DateTime.Today.Date)
            {
                return _overdueItems;
            }

            if (_dayItems.ContainsKey(date))
            {
                return _dayItems[date];
            }

            date = GetFirstOfMonth(date);
            if (!_monthItems.ContainsKey(date))
            {
                _monthItems.Add(date, new List<TraceItemModel>());
            }

            return _monthItems[date];
        }

        private bool IsMonthGroup(DateTime dateTime)
        {
            var date = dateTime.Date;
            return !_dayItems.ContainsKey(date) && _monthItems.ContainsKey(GetFirstOfMonth(date));
        }

        private DateTime GetFirstOfMonth(DateTime dateTime)
        {
            var date = dateTime.Date;
            return date.AddDays(-(date.Day - 1));
        }
    }
}