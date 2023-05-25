using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Decen.Common.Logs.Structs
{
    struct ShortTimeSpan
    {
        public enum UnitKind
        {
            Seconds,
            Milliseconds,
            Microseconds,
            Nanoseconds
        };

        UnitKind unit;

        public UnitKind Unit => unit;

        public double Value { get; set; }

        public double Seconds
        {
            get { return Value * Scale; }
        }

        public static ShortTimeSpan FromSeconds(double seconds)
        {

            var s = seconds;
            UnitKind unit = UnitKind.Seconds;

            if (s < 1)
            {
                s = seconds * 1e3;
                unit = UnitKind.Milliseconds;
                if (s < 1)
                {
                    s = seconds * 1e6;
                    unit = UnitKind.Microseconds;

                    if (s < 1)
                    {
                        s = seconds * 1e9;
                        unit = UnitKind.Nanoseconds;
                        if (s < 1) // treat < 0 ns as 0 s.
                            return default(ShortTimeSpan);
                    }
                }
            }

            return new ShortTimeSpan { unit = unit, Value = Math.Round(s, 3) };
        }

        public static ShortTimeSpan FromString(string str)
        {
            str = str.Trim();
            UnitKind scale = UnitKind.Seconds;
            if (str.EndsWith("ms"))
            {
                scale = UnitKind.Milliseconds;
            }
            else if (str.EndsWith("μs") || str.EndsWith("us")) // support mu symbol for forward compatibility use
            {
                scale = UnitKind.Microseconds;
            }
            else if (str.EndsWith("ns"))
            {
                scale = UnitKind.Nanoseconds;
            }
            else if (str.EndsWith("s"))
            {

            }

            int index = 0;
            for (; index < str.Length; index++)
            {
                if (char.IsNumber(str[index]) || str[index] == '.')
                    continue;
                else
                    break;
            }
            str = str.Substring(0, index);
            var val = double.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
            return new ShortTimeSpan { Value = val, unit = scale };
        }

        static string getUnitString(UnitKind unit)
        {
            switch (unit)
            {
                case UnitKind.Microseconds:
                    return "us"; // "μs" support this notation at a later time.
                case UnitKind.Milliseconds:
                    return "ms";
                case UnitKind.Nanoseconds:
                    return "ns";
                default:
                    return "s";
            }
        }

        public double Scale
        {
            get
            {
                switch (unit)
                {
                    case UnitKind.Milliseconds:
                        return 1e-3;
                    case UnitKind.Microseconds:
                        return 1e-6;
                    case UnitKind.Nanoseconds:
                        return 1e-9;
                    default:
                        return 1;
                }
            }
        }

        public override string ToString()
        {
            if (Value < 10)
                return string.Format(CultureInfo.InvariantCulture, "{0:0.00} {1}", Math.Floor(Value * 100) * 0.01, getUnitString(Unit));
            else if (Value < 100)
                return string.Format(CultureInfo.InvariantCulture, "{0:0.0} {1}", Math.Round(Value * 10) * 0.1, getUnitString(Unit));
            else
                return string.Format(CultureInfo.InvariantCulture, "{0} {1}", Math.Floor(Value), getUnitString(Unit));
        }

        /// <summary> To avoid generating extra garbage during formatting, this can be used with StringBuilder. </summary>
        /// <param name="output"></param>
        public void ToString(System.Text.StringBuilder output)
        {
            if (Value < 0.01)
            {
                output.Append("0 ns");
                return;
            };
            if (Value < 10)
                output.Append((Math.Floor(Value * 100) * 0.01).ToString("0.00", CultureInfo.InvariantCulture));
            else if (Value < 100)
                output.Append((Math.Floor(Value * 10) * 0.1).ToString("0.0", CultureInfo.InvariantCulture));
            else
                output.Append(Math.Floor(Value).ToString(CultureInfo.InvariantCulture));
            output.Append(' ');
            output.Append(getUnitString(Unit));
        }

        public (string, string) ToStringParts()
        {
            if (Value < 10)
                return (string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Math.Floor(Value * 100) * 0.01), getUnitString(Unit));
            else if (Value < 100)
                return (string.Format(CultureInfo.InvariantCulture, "{0:0.0}", Math.Floor(Value * 10) * 0.1), getUnitString(Unit));
            else
                return (string.Format(CultureInfo.InvariantCulture, "{0:0}", Math.Floor(Value)), getUnitString(Unit));
        }
    }
}
