// File: DataFrameExtensions.cs
// Author: Jan Lorenc
// Solution: RQA System Anomaly Detection
// Project: AnomalyDetection.Application

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Analysis;

namespace YSoft.Rqa.AnomalyDetection.Application
{
    public static class DataFrameExtensions
    {
        /// <summary>Gets a column data from the DataFrame.</summary>
        /// <typeparam name="T">Type of the DataFrame column.</typeparam>
        /// <param name="df"></param>
        /// <param name="col">Name of the DataFrame column.</param>
        /// <returns>DataFrame column with specified name.</returns>
        public static PrimitiveDataFrameColumn<T> Get<T>(this DataFrame df, string col) where T : unmanaged => df.Columns.GetPrimitiveColumn<T>(col);

        /// <summary>Gets a column data from the DataFrame transformed to a list with non-nullable type.</summary>
        /// <typeparam name="T">Type of the DataFrame column.</typeparam>
        /// <param name="df"></param>
        /// <param name="col">Name of the DataFrame column.</param>
        /// <returns>DataFrame column with specified name as a list.</returns>
        public static List<T> GetAsList<T>(this DataFrame df, string col) where T : unmanaged => df.Columns.GetPrimitiveColumn<T>(col).Select(x => x.Value).ToList();

        /// <summary>Sets a constant value a whole column.</summary>
        /// <typeparam name="T">Type of the DataFrame column.</typeparam>
        /// <param name="df"></param>
        /// <param name="col">Name of the DataFrame column.</param>
        /// <param name="value">Value to set.</param>
        public static void Set<T>(this DataFrame df, string col, T? value) where T : unmanaged {
            var newCol = new PrimitiveDataFrameColumn<T>(col, Enumerable.Repeat(value, df.Length()));
            df.Columns.Remove(col);
            df.Columns.Add(newCol);
        }

        /// <summary>Adds or replaces a column with a new one of that name.</summary>
        /// <typeparam name="T">Type of the DataFrame column.</typeparam>
        /// <param name="df"></param>
        /// <param name="col">Column values to set.</param>
        public static void Set<T>(this DataFrame df, PrimitiveDataFrameColumn<T> col) where T : unmanaged {
            df.Columns.Remove(col.Name);
            df.Columns.Add(col);
        }

        /// <summary>Removes duplicate rows from a dataset.</summary>
        /// <typeparam name="T">Type of the DataFrame column by which the duplicity is measured.</typeparam>
        /// <param name="df"></param>
        /// <param name="by">Name of the DataFrame column by which the duplicity is measured.</param>
        /// <param name="keepFirst">If set, first of the duplicates is kept, otherwise all are removed.</param>
        /// <returns></returns>
        public static DataFrame DropDuplicates<T>(this DataFrame df, string by, bool keepFirst = true) where T : unmanaged {
            var col = df.GetAsList<T>(by);
            var isDuplicateList = keepFirst
                ? col.Select((x, i) => col.Take(i).Any(y => y.Equals(x))).ToList()
                : col.Select(x => col.Count(y => y.Equals(x)) > 1).ToList();

            var filter = new PrimitiveDataFrameColumn<bool>("filter", isDuplicateList).Xor(true);
            return df[filter];
        }

        /// <summary>Gets the DataFrame length as number of rows.</summary>
        /// <param name="df"></param>
        /// <returns>Number of rows.</returns>
        public static int Length(this DataFrame df) => (int) df.Rows.Count;
    }
}