using Decen.Common.Logs.Logging.LogImplement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Decen.Common.Logs.Utils
{
    public static class Util
    {
        static readonly char[] padding = { '=' };
        public static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd(padding).Replace('+', '-')
                .Replace('/', '_');
        }
        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> collection)
        {
            return collection.Select((ele, index) => (index, ele));
        }
        public static Action Bind<T>(this Action del, Action<T> f, T v)
        {
            del += () => f(v);
            return del;
        }


#if DEBUG
        public static readonly bool IsDebugBuild = true;
#else
        public static readonly bool IsDebugBuild = false;
#endif

        static public Action ActionDefault = () => { };

        public static void Swap<T>(ref T a, ref T b)
        {
            T buffer = a;
            a = b;
            b = buffer;
        }

        /// <summary>
        /// Clamps val to be between min and max, returning the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
        }

        /// <summary>
        /// Returns arg.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T Identity<T>(T id)
        {
            return id;
        }

        /// <summary> Do nothing. </summary>
        public static void Noop() { }


        /// <summary>
        /// Returns the element for which selector returns the max value.
        /// if IEnumerable is empty, it returns default(T) multiplier gives the direction to search.
        /// </summary>
        static T FindExtreme<T, C>(this IEnumerable<T> ienumerable, Func<T, C> selector, int multiplier) where C : IComparable
        {
            if (!ienumerable.Any())
            {
                return default(T);
            }
            T selected = ienumerable.FirstOrDefault();
            C max = selector(selected);


            foreach (T obj in ienumerable.Skip(1))
            {
                C comparable = selector(obj);
                if (comparable.CompareTo(max) * multiplier > 0)
                {
                    selected = obj;
                    max = comparable;
                }
            }

            return selected;
        }
        /// <summary>
        /// Returns the element for which selector returns the max value.
        /// if IEnumerable is empty, it returns default(T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="ienumerable"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T FindMax<T, C>(this IEnumerable<T> ienumerable, Func<T, C> selector) where C : IComparable
        {
            return FindExtreme(ienumerable, selector, 1);
        }

        /// <summary>
        /// Returns the element for which selector returns the minimum value.
        /// if IEnumerable is empty, it returns default(T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="ienumerable"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static T FindMin<T, C>(this IEnumerable<T> ienumerable, Func<T, C> selector) where C : IComparable
        {
            return FindExtreme(ienumerable, selector, -1);
        }

        /// <summary>
        /// Skips last N items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="n">n last items to skip.</param>
        /// <returns></returns>
        public static IEnumerable<T> SkipLastN<T>(this IEnumerable<T> source, int n)
        {
            var list = source.ToList();
            if ((list.Count - n) > 0)
                return list.Take(list.Count - n);
            else
                return Enumerable.Empty<T>();
        }


        /// <summary>
        /// Removes items of source matching a given predicate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pred"></param>
        public static void RemoveIf<T>(this IList<T> source, Predicate<T> pred)
        {
            if (source is List<T> lst)
            {
                lst.RemoveAll(pred);
                return;
            }
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (pred(source[i]))
                {
                    source.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes items of source matching a given predicate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pred"></param>
        public static void RemoveIf(this System.Collections.IList source, Predicate<object> pred)
        {

            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (pred(source[i]))
                {
                    source.RemoveAt(i);
                }
            }
        }

        static void flattenHeirarchy<T>(IEnumerable<T> lst, Func<T, IEnumerable<T>> lookup, IList<T> result)
        {
            flattenHeirarchy(lst, lookup, result, null);
        }

        private static void flattenHeirarchy<T>(IEnumerable<T> lst, Func<T, IEnumerable<T>> lookup, IList<T> result, HashSet<T> found)
        {
            foreach (var item in lst)
            {
                if (found != null)
                {
                    if (found.Contains(item))
                        continue;
                    found.Add(item);
                }
                result.Add(item);
                var sublist = lookup(item);
                if (sublist != null)
                    flattenHeirarchy(sublist, lookup, result, found);
            }
        }

        /// <summary>
        /// Flattens a recursive IEnumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="lookup">Returns a list of the next level of elements. The returned value is allowed to be null and will in this case be treated like an empty list.</param>
        /// <param name="distinct">True if only one of each element should be inserted in the list.</param>
        /// <param name="buffer">Buffer to use instead of creating a new list to store the values. This can be used to avoid allocation.</param>
        /// <returns></returns>
        public static List<T> FlattenHeirarchy<T>(IEnumerable<T> lst, Func<T, IEnumerable<T>> lookup, bool distinct = false, List<T> buffer = null)
        {
            if (buffer != null)
                buffer.Clear();
            else
                buffer = new List<T>();
            flattenHeirarchy(lst, lookup, buffer, distinct ? new HashSet<T>() : null);
            return buffer;
        }

        public static List<T> FlattenHeirarchy<T>(IEnumerable<T> lst, Func<T, T> lookup, bool distinct = false,
            List<T> buffer = null)
        {
            if (buffer != null)
                buffer.Clear();
            else
                buffer = new List<T>();
            flattenHeirarchy(lst, x => new[] { lookup(x) }, buffer, distinct ? new HashSet<T>() : null);
            return buffer;
        }


        public static void FlattenHeirarchyInto<T>(IEnumerable<T> lst, Func<T, IEnumerable<T>> lookup, ISet<T> set)
        {
            foreach (var item in lst)
            {
                if (set.Add(item))
                {
                    var sublist = lookup(item);
                    if (sublist != null)
                        FlattenHeirarchyInto(sublist, lookup, set);
                }
            }

        }

        public static IEnumerable<T> Recurse<T>(T item, Func<T, T> selector)
        {
            yield return item;
            while (true)
            {
                item = selector(item);
                yield return item;
            }
        }


        public static void ForEach<T>(this IEnumerable<T> source, Action<T> func)
        {
            foreach (var item in source) { func(item); }
        }
        /// <summary>
        /// Appends a range of elements to an IEnumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="newObjects"></param>
        /// <returns></returns>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] newObjects)
        {
            return source.Concat(newObjects);
        }

        /// <summary>
        /// First index where the result of predicate function is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pred"></param>
        /// <returns></returns>
        public static int IndexWhen<T>(this IEnumerable<T> source, Func<T, bool> pred)
        {
            int idx = 0;
            foreach (var item in source)
            {
                if (pred(item))
                {
                    return idx;
                }
                idx++;
            }
            return -1;
        }

        /// <summary>
        /// Returns true if the source is longer than count elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool IsLongerThan<T>(this IEnumerable<T> source, long count)
        {
            foreach (var _ in source)
                if (--count < 0)
                    return true;
            return false;
        }

        /// <summary>
        /// Adds a range of values to a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this IList<T> lst, IEnumerable<T> values)
        {
            foreach (var value in values)
                lst.Add(value);
        }

        [Obsolete("Cannot add to array", true)]
        public static void AddRange<T>(this T[] lst, IEnumerable<T> values)
        {
            // This function is intentionally added to avoid adding the arrays.
            // They also implement IList, so they normally hit the other overload.
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a HashSet from an IEnumerable.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
        /// <summary>
        /// Creates a HashSet from an IEnumerable, with a specialized comparer.
        /// </summary>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(source, comparer);
        }

        /// <summary>
        /// The opposite of Where.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, Func<T, bool> selector)
        {
            foreach (var x in source)
                if (selector(x) == false)
                    yield return x;
        }

        /// <summary> As 'Select' but skipping null values.
        /// Short hand for/more efficient version of 'Select(f).Where(x => x != null)' </summary>
        /// <param name="source"></param>
        /// <param name="f"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T2> SelectValues<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> f)
        {
            foreach (var x in source)
            {
                var value = f(x);
                if (value != null)
                    yield return value;
            }
        }

        /// <summary> As 'Select and FirstOrDefault' but skipping null values.
        /// Short hand for/more efficient version of 'Select(f).Where(x => x != null).FirstOrDefault()'
        /// </summary>
        public static T2 FirstNonDefault<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> f)
        {
            foreach (var x in source)
            {
                var value = f(x);
                if (Equals(value, default(T2)) == false)
                    return value;
            }

            return default(T2);
        }


        //We need to remember the timers or they risk getting garbage collected before elapsing.
        readonly static HashSet<System.Threading.Timer> delayTimers = new HashSet<System.Threading.Timer>();

        /// <summary>
        /// Calls function after a delay.
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="function"></param>
        public static void Delay(int ms, Action function)
        {
            lock (delayTimers)
            {
                System.Threading.Timer timer = null;
                timer = new System.Threading.Timer(obj =>
                {
                    lock (delayTimers) //happens in a new thread to no race.
                    {
                        delayTimers.Remove(timer);
                    }
                    function();
                }, null, ms, System.Threading.Timeout.Infinite);
                delayTimers.Add(timer); //see note for delayTimers.
            }
        }

        /// <summary>
        /// Merged a dictionary into another, overwriting colliding keys.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="srcDict"></param>
        /// <param name="dstDict"></param>
        public static void MergeInto<T1, T2>(this Dictionary<T1, T2> srcDict, Dictionary<T1, T2> dstDict)
        {
            foreach (var kv in srcDict)
            {
                dstDict[kv.Key] = kv.Value;
            }
        }

        /// <summary>
        /// Almost the same as string.Split, except it preserves split chars as 1 length strings. The process can always be reversed by String.Join("", result).
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitValues"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitPreserve(this string str, params char[] splitValues)
        {
            var splitHash = splitValues.ToHashSet();
            int offset = 0;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (splitHash.Contains(c))
                {
                    var newStr = str.Substring(offset, i - offset);
                    if (newStr.Length > 0) yield return newStr;

                    yield return new string(c, 1);
                    offset = i + 1;
                }
            }

            if (offset < str.Length)
            {
                yield return str.Substring(offset, str.Length - offset);
            }
        }

        struct OnceLogToken
        {
            public object Token;
            public TraceSource Log;
        }

        static HashSet<OnceLogToken> logOnceTokens = new HashSet<OnceLogToken>();

        /// <summary>
        /// Avoids spamming the log with errors that 
        /// should only be shown once by memorizing token and TraceSource. 
        /// </summary>
        /// <returns>True if an error was logged.</returns>
        public static bool ErrorOnce(this TraceSource log, object token, string message, params object[] args)
        {
            lock (logOnceTokens)
            {
                var logtoken = new OnceLogToken { Token = token, Log = log };
                if (!logOnceTokens.Contains(logtoken))
                {
                    log.Error(message, args);
                    logOnceTokens.Add(logtoken);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Lazily reads all the lines of a file. Should only be read once.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadFileLines(string filePath)
        {
            using (var str = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var read = new StreamReader(str))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }

        public static string ConvertToUnsecureString(this System.Security.SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static System.Security.SecureString ToSecureString(this string str)
        {
            System.Security.SecureString result = new System.Security.SecureString();
            foreach (var c in str)
                result.AppendChar(c);
            return result;
        }

        public static Type TypeOf(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Single: return typeof(float);
                case TypeCode.Double: return typeof(double);
                case TypeCode.SByte: return typeof(sbyte);
                case TypeCode.Int16: return typeof(short);
                case TypeCode.Int32: return typeof(int);
                case TypeCode.Int64: return typeof(long);
                case TypeCode.Byte: return typeof(byte);
                case TypeCode.UInt16: return typeof(ushort);
                case TypeCode.UInt32: return typeof(uint);
                case TypeCode.UInt64: return typeof(ulong);
                case TypeCode.String: return typeof(string);
                case TypeCode.Boolean: return typeof(bool);
                case TypeCode.DateTime: return typeof(DateTime);
                case TypeCode.Decimal: return typeof(decimal);
                case TypeCode.Char: return typeof(char);
            }
            return typeof(object);
        }

        public static bool IsNumeric(object obj)
        {
            switch (obj)
            {
                case float _: return true;
                case double _: return true;
                case decimal _: return true;
                case byte _: return true;
                case char _: return true;
                case sbyte _: return true;
                case short _: return true;
                case ushort _: return true;
                case int _: return true;
                case uint _: return true;
                case long _: return true;
                case ulong _: return true;
                default: return false;
            }

        }

        public static bool IsFinite(double value)
        {
            return false == (double.IsInfinity(value) || double.IsNaN(value));
        }

        public static bool Compatible(Version searched, Version referenced)
        {
            if (searched == null) return true;

            if (searched.Major != referenced.Major) return false;
            if (searched.Minor >= referenced.Minor) return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="flag"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static T SetFlag<T>(this T e, T flag, bool enabled) where T : struct
        {
            if (e is Enum == false)
                throw new InvalidOperationException("T must be an enum");
            int _e = (int)Convert.ChangeType(e, typeof(int));
            int _flag = (int)Convert.ChangeType(flag, typeof(int));
            int r;
            if (enabled)
                r = (_e | _flag);
            else
                r = (_e & ~_flag);

            return (T)Enum.ToObject(typeof(T), r);
        }
        static double churnDoubleNumber(string a, ref int offset)
        {
            // consider using CultureInfo.NumberFormatInfo for decimal separators.
            // this would come at a performance penalty.

            double val = 0.0;
            int neg = 1;
            bool pls = false;
            bool decfound = false;
            double dec = 1;
            while (offset < a.Length)
            {
                var c = a[offset];
                switch (c)
                {
                    case '-':
                        if (neg == -1 || pls) return neg * val * dec;
                        neg = -1;
                        break;
                    case '.':
                        if (decfound) return neg * val * dec;
                        decfound = true;
                        break;
                    default:
                        int digit = c - '0';
                        if (digit < 0 || digit > 9)
                            return neg * val * dec;
                        val = val * 10 + digit;
                        if (decfound)
                            dec *= 0.1;
                        break;
                }
                offset += 1;
            }
            return neg * val * dec;
        }
        /// <summary>
        /// Natural compare takes numbers into account in comparison of strings. Normal sorted: [1,10,100,11,2,23,3] Natural sorted: [1,2,3,10,11,23,100]
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static int NaturalCompare(string A, string B)
        {

            if (A == null || B == null) // null -> use string.Compare behavior.
                return string.Compare(A, B);

            int ai = 0, bi = 0;
            for (; ; ai++, bi++)
            {
                if (ai == A.Length)
                {
                    if (bi == B.Length) return 0;
                    return -1;
                }
                if (bi == B.Length) return 1;
                int nextai = ai;
                double numA = churnDoubleNumber(A, ref nextai);
                int nextbi = bi;
                double numB = churnDoubleNumber(B, ref nextbi);
                if (nextai != ai && nextbi == bi) return -1;
                if (nextbi != bi && nextai == ai) return 1;
                if (nextai != ai && numA != numB)
                {
                    return numA.CompareTo(numB);
                }
                int cmp = A[ai].CompareTo(B[bi]);
                if (cmp != 0) return cmp;
            }
        }

        /// <summary> Shuffle a list in place. </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col"></param>
        public static void Shuffle<T>(this IList<T> col)
        {
            Random rnd = new Random();
            for (int i = 0; i < col.Count; i++)
            {
                var j = rnd.Next(0, col.Count);
                var a = col[i];
                col[i] = col[j];
                col[j] = a;
            }
        }

        public static string EnumToReadableString(Enum value)
        {
            //if (value == null) return null;
            //var enumType = value.GetType();
            //var mem = enumType.GetMember(value.ToString()).FirstOrDefault();
            //if (mem != null) return mem.GetDisplayAttribute().Name;
            //if (false == enumType.HasAttribute<FlagsAttribute>())
            //    return value.ToString();
            //var zeroValue = Enum.ToObject(enumType, 0);
            //if (value.Equals(zeroValue))
            //    return ""; // this does not happen if zeroValue is declared.

            //var flags = Enum.GetValues(enumType).OfType<Enum>();
            //var activeFlags = flags.Where(value.HasFlag).Except(f => f.Equals(zeroValue));
            //var result = string.Join(" | ", activeFlags.Select(EnumToReadableString));
            //if (string.IsNullOrEmpty(result) == false) return result;

            //// last resort.
            //var val = (long)Convert.ChangeType(value, TypeCode.Int64);
            //return val.ToString();

            return "";
        }

        public static string EnumToDescription(Enum value)
        {
            //if (value == null) return null;
            //var enumType = value.GetType();
            //var mem = enumType.GetMember(value.ToString()).FirstOrDefault();
            //// if member is null, fall back to the readable enum string (or description is null)
            //return mem?.GetDisplayAttribute().Description ?? EnumToReadableString(value);
            return "";
        }

        //public static string SerializeToString(this TestPlan plan, bool throwOnErrors = false)
        //{
        //    using (var mem = new MemoryStream())
        //    {
        //        var serializer = new TapSerializer();
        //        plan.Save(mem, serializer);
        //        if (throwOnErrors && serializer.Errors.Any())
        //            throw new Exception(string.Join("\n", serializer.Errors));
        //        return Encoding.UTF8.GetString(mem.ToArray());
        //    }
        //}


        public static object DeserializeFromString(string str)
        {
            //return new TapSerializer().DeserializeFromString(str);
            return null;
        }

        public static T DeserializeFromString<T>(string str) => (T)DeserializeFromString(str);

        class ActionDisposable : IDisposable
        {
            Action action;
            public ActionDisposable(Action action) => this.action = action;

            public void Dispose()
            {
                action();
                action = null;
            }
        }

        public static IDisposable WithDisposable(Action action)
        {
            return new ActionDisposable(action);
        }

        /// <summary> Gets or creates a value based on the key. This is useful for caches. </summary>
        public static V GetOrCreateValue<K, V>(this Dictionary<K, V> dictionary, K key, Func<K, V> createValue)
        {
            if (dictionary.TryGetValue(key, out V value))
                return value;
            return dictionary[key] = createValue(key);
        }

        public static string BytesToReadable(long bytes)
        {
            if (bytes < 1000) return $"{bytes} B";
            if (bytes < 1000000) return $"{bytes / 1000.0:0.00} kB";
            if (bytes < 1000000000) return $"{bytes / 1000000.0:0.00} MB";
            return $"{bytes / 1000000000.0:0.00} GB";
        }
    }
}
