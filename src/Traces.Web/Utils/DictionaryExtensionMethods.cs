using System;
using System.Collections.Generic;
using System.Linq;
using Traces.Web.Models;

namespace Traces.Web.Utils
{
    public static class DictionaryExtensionMethods
    {
        public static void AddTrace(this SortedDictionary<DateTime, List<TraceItemModel>> dictionary, TraceItemModel trace)
        {
            if (dictionary.ContainsKey(trace.DueDate))
            {
                var existentTraces = dictionary[trace.DueDate];
                if (existentTraces.Exists(item => item.Id == trace.Id))
                {
                    return;
                }

                existentTraces.Add(trace);
            }
            else
            {
                dictionary.Add(
                    trace.DueDate,
                    new List<TraceItemModel>
                    {
                        trace
                    });
            }
        }

        public static void LoadTraces(this SortedDictionary<DateTime, List<TraceItemModel>> dictionary, IReadOnlyList<TraceItemModel> traces)
        {
            dictionary.Clear();

            var groupedTraces = traces.GroupBy(x => x.DueDate).ToList();

            foreach (var group in groupedTraces)
            {
                dictionary.Add(group.Key, group.ToList());
            }
        }

        public static void RemoveTrace(this SortedDictionary<DateTime, List<TraceItemModel>> dictionary, TraceItemModel trace)
        {
            if (dictionary.ContainsKey(trace.DueDate))
            {
                dictionary[trace.DueDate].Remove(trace);

                if (dictionary[trace.DueDate].Count == 0)
                {
                    dictionary.Remove(trace.DueDate);
                }
            }
        }
    }
}