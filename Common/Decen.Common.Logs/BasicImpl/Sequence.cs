using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decen.Common.Logs.BasicImpl
{
    public static class Sequence
    {
        public static T[] AsSingle<T>(this T item) => item == null ? Array.Empty<T>() : new[] { item };

        public static T? FirstOrNull<T>(this IEnumerable<T> items, Func<T, bool> p) where T : struct
        {
            foreach (var item in items)
            {
                if (p(item))
                    return item;
            }
            return null;
        }


        public static List<T> DistinctLast<T>(this IEnumerable<T> items)
        {
            Dictionary<T, int> d = new Dictionary<T, int>();
            int i = 0;
            foreach (var item in items)
            {
                d[item] = i;
                i++;
            }
            return d.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();
        }

        internal static int ProcessPattern<T1>(IEnumerator objs, Action<T1> f1)
        {
            while (objs.MoveNext())
            {
                switch (objs.Current)
                {
                    case T1 t:
                        f1(t);
                        return 1;
                }
            }
            return 0;
        }

        internal static int ProcessPattern<T1, T2>(IEnumerator objs, Action<T1> f1, Action<T2> f2)
        {
            while (objs.MoveNext())
            {
                switch (objs.Current)
                {
                    case T1 t:
                        f1(t);
                        return 1 + ProcessPattern(objs, f2);
                    case T2 t:
                        f2(t);
                        return 1 + ProcessPattern(objs, f1);
                }
            }
            return 0;
        }

        internal static int ProcessPattern<T1, T2, T3>(IEnumerator objs, Action<T1> f1, Action<T2> f2, Action<T3> f3)
        {
            while (objs.MoveNext())
            {
                switch (objs.Current)
                {
                    case T1 t:
                        f1(t);
                        return 1 + ProcessPattern(objs, f2, f3);
                    case T2 t:
                        f2(t);
                        return 1 + ProcessPattern(objs, f1, f3);
                    case T3 t:
                        f3(t);
                        return 1 + ProcessPattern(objs, f1, f2);
                }
            }
            return 0;
        }

        /// <summary> Adds elements that arent null to the list. </summary>
        internal static void AddExceptNull<T>(this ICollection<T> list, T x)
        {
            if (x != null)
                list.Add(x);
        }

        internal static int ProcessPattern<T1, T2, T3, T4>(IEnumerator objs, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4)
        {
            while (objs.MoveNext())
            {
                switch (objs.Current)
                {
                    case T1 t:
                        f1(t);
                        return 1 + ProcessPattern(objs, f2, f3, f4);
                    case T2 t:
                        f2(t);
                        return 1 + ProcessPattern(objs, f1, f3, f4);
                    case T3 t:
                        f3(t);
                        return 1 + ProcessPattern(objs, f1, f2, f4);
                    case T4 t:
                        f4(t);
                        return 1 + ProcessPattern(objs, f1, f2, f3);
                }
            }
            return 0;
        }

        public static int ProcessPattern<T1, T2, T3, T4, T5>(IEnumerator objs, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5)
        {
            while (objs.MoveNext())
            {
                switch (objs.Current)
                {
                    case T1 t:
                        f1(t);
                        return 1 + ProcessPattern(objs, f2, f3, f4, f5);
                    case T2 t:
                        f2(t);
                        return 1 + ProcessPattern(objs, f1, f3, f4, f5);
                    case T3 t:
                        f3(t);
                        return 1 + ProcessPattern(objs, f1, f2, f4, f5);
                    case T4 t:
                        f4(t);
                        return 1 + ProcessPattern(objs, f1, f2, f3, f5);
                    case T5 t:
                        f5(t);
                        return 1 + ProcessPattern(objs, f1, f2, f3, f4);
                }
            }
            return 0;
        }

        public static int ProcessPattern<T1, T2, T3, T4, T5, T6>(IEnumerator objs, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6)
        {
            while (objs.MoveNext())
            {
                switch (objs.Current)
                {
                    case T1 t:
                        f1(t);
                        return 1 + ProcessPattern(objs, f2, f3, f4, f5, f6);
                    case T2 t:
                        f2(t);
                        return 1 + ProcessPattern(objs, f1, f3, f4, f5, f6);
                    case T3 t:
                        f3(t);
                        return 1 + ProcessPattern(objs, f1, f2, f4, f5, f6);
                    case T4 t:
                        f4(t);
                        return 1 + ProcessPattern(objs, f1, f2, f3, f5, f6);
                    case T5 t:
                        f5(t);
                        return 1 + ProcessPattern(objs, f1, f2, f3, f4, f6);
                    case T6 t:
                        f6(t);
                        return 1 + ProcessPattern(objs, f1, f2, f3, f4, f5);
                }
            }
            return 0;
        }

        public static int ProcessPattern<T1, T2>(IEnumerable<object> objs, Action<T1> f1, Action<T2> f2)
        {
            using (var e = objs.GetEnumerator())
                return ProcessPattern(e, f1, f2);
        }

        public static int ProcessPattern<T1, T2, T3>(IEnumerable<object> objs, Action<T1> f1, Action<T2> f2, Action<T3> f3)
        {
            using (var e = objs.GetEnumerator())
                return ProcessPattern(e, f1, f2, f3);
        }
        public static int ProcessPattern<T1, T2, T3, T4>(IEnumerable<object> objs, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4)
        {
            using (var e = objs.GetEnumerator())
                return ProcessPattern(e, f1, f2, f3, f4);
        }
        public static int ProcessPattern<T1, T2, T3, T4, T5>(IEnumerable<object> objs, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5)
        {
            using (var e = objs.GetEnumerator())
                return ProcessPattern(e, f1, f2, f3, f4, f5);
        }
        public static int ProcessPattern<T1, T2, T3, T4, T5, T6>(IEnumerable<object> objs, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4, Action<T5> f5, Action<T6> f6)
        {
            using (var e = objs.GetEnumerator())
                return ProcessPattern(e, f1, f2, f3, f4, f5, f6);
        }

        /// <summary>
        /// Count the number of elements in an enumerable.
        /// </summary>
        public static int Count(this IEnumerable enumerable)
        {
            if (enumerable is ICollection col)
                return col.Count;
            int c = 0;
            foreach (var _ in enumerable)
                c++;
            return c;
        }

        /// <summary>
        /// iterates lists and generates pairs of each list. Once the end is reached for one of the lists, execution stops. 
        /// </summary>
        public static IEnumerable<(T1, T2)> Pairwise<T1, T2>(this IEnumerable<T1> a, IEnumerable<T2> b)
        {
            using (var ia = a.GetEnumerator())
            using (var ib = b.GetEnumerator())
            {
                while (ia.MoveNext() && ib.MoveNext())
                {
                    yield return (ia.Current, ib.Current);
                }
            }
        }

        public static void Append<T>(ref T[] array, params T[] appendage)
        {
            int preLen = array.Length;
            Array.Resize(ref array, array.Length + appendage.Length);
            Array.Copy(appendage, 0, array, preLen, appendage.Length);
        }

        public static IEnumerable<T2> TrySelect<T, T2>(this IEnumerable<T> src, Func<T, T2> f,
            Action<Exception, T> handler) => src.TrySelect<T, T2, Exception>(f, handler);
        public static IEnumerable<T2> TrySelect<T, T2>(this IEnumerable<T> src, Func<T, T2> f,
            Action<Exception> handler) => src.TrySelect<T, T2, Exception>(f, (e, v) => handler(e));

        public static IEnumerable<T2> TrySelect<T, T2, T3>(this IEnumerable<T> src, Func<T, T2> f, Action<T3, T> handler) where T3 : Exception
        {
            foreach (var x in src)
            {
                T2 y;
                try
                {
                    y = f(x);
                }
                catch (T3 e)
                {
                    handler(e, x);
                    continue;
                }
                yield return y;
            }
        }
    }
}
