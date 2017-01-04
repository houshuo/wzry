namespace ApolloTdr
{
    using System;
    using System.Text;

    public class TdrBufUtil
    {
        public static TdrError.ErrorType printArray(ref TdrVisualBuf buf, int indent, char sep, string variable, long count)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable, count };
                type = buf.sprintf("{0}[0:{1:d}]: ", args);
            }
            return type;
        }

        public static TdrError.ErrorType printMultiStr(ref TdrVisualBuf buf, string str, int times)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            for (int i = 0; i < times; i++)
            {
                object[] args = new object[] { str };
                type = buf.sprintf("{0}", args);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public static TdrError.ErrorType printString(ref TdrVisualBuf buf, int indent, char sep, string variable, byte[] bStr)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            string str = string.Empty;
            int count = TdrTypeUtil.cstrlen(bStr);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = printMultiStr(ref buf, "    ", indent);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                str = Encoding.ASCII.GetString(bStr, 0, count);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable, str, sep };
                type = buf.sprintf("{0}: {1}{2}", args);
            }
            return type;
        }

        public static TdrError.ErrorType printString(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, byte[] bStr)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            string str = string.Empty;
            int count = TdrTypeUtil.cstrlen(bStr);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = printMultiStr(ref buf, "    ", indent);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                str = Encoding.ASCII.GetString(bStr, 0, count);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable, arrIdx, str, sep };
                type = buf.sprintf("{0}[{1:d}]: {2}{3}", args);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrDate(ref TdrVisualBuf buf, int indent, char sep, string variable, uint date)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable };
                type = buf.sprintf("{0}: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrDate2Str(ref buf, date);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrDate(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, uint date)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable, arrIdx };
                type = buf.sprintf("{0}[{1:d}]: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrDate2Str(ref buf, date);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrDateTime(ref TdrVisualBuf buf, int indent, char sep, string variable, ulong datetime)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable };
                type = buf.sprintf("{0}: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrDateTime2Str(ref buf, datetime);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrDateTime(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, ulong datetime)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable, arrIdx };
                type = buf.sprintf("{0}[{1:d}]: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrDateTime2Str(ref buf, datetime);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrIP(ref TdrVisualBuf buf, int indent, char sep, string variable, uint ip)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable };
                type = buf.sprintf("{0}: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrIP2Str(ref buf, ip);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrIP(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, uint ip)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable, arrIdx };
                type = buf.sprintf("{0}[{1:d}]: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrIP2Str(ref buf, ip);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrTime(ref TdrVisualBuf buf, int indent, char sep, string variable, uint time)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable };
                type = buf.sprintf("{0}: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrTime2Str(ref buf, time);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printTdrTime(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, uint time)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] args = new object[] { variable, arrIdx };
                type = buf.sprintf("{0}[{1:d}]: ", args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrTypeUtil.tdrTime2Str(ref buf, time);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, bool withSep)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type != TdrError.ErrorType.TDR_NO_ERROR)
            {
                return type;
            }
            if (withSep)
            {
                object[] objArray1 = new object[] { variable, sep };
                return buf.sprintf("{0}{1}", objArray1);
            }
            object[] args = new object[] { variable };
            return buf.sprintf("{0}: ", args);
        }

        public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, bool withSep)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type != TdrError.ErrorType.TDR_NO_ERROR)
            {
                return type;
            }
            if (withSep)
            {
                object[] objArray1 = new object[] { variable, arrIdx, sep };
                return buf.sprintf("{0}[{1:d}]{2}", objArray1);
            }
            object[] args = new object[] { variable, arrIdx };
            return buf.sprintf("{0}[{1:d}]: ", args);
        }

        public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, string format, params object[] args)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray1 = new object[] { variable };
                type = buf.sprintf("{0}: ", objArray1);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = buf.sprintf(format, args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printVariable(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, string format, params object[] args)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = printMultiStr(ref buf, "    ", indent);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray1 = new object[] { variable, arrIdx };
                type = buf.sprintf("{0}[{1:d}]: ", objArray1);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = buf.sprintf(format, args);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray2 = new object[] { sep };
                type = buf.sprintf("{0}", objArray2);
            }
            return type;
        }

        public static TdrError.ErrorType printWString(ref TdrVisualBuf buf, int indent, char sep, string variable, short[] str)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = TdrTypeUtil.wstrlen(str) + 1;
            object[] args = new object[] { variable };
            type = buf.sprintf("{0}:  ", args);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int num2 = TdrTypeUtil.wstrlen(str);
                for (int i = 0; i < num2; i++)
                {
                    object[] objArray2 = new object[] { str[i] };
                    type = buf.sprintf("0x{0:X4}", objArray2);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        break;
                    }
                }
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray3 = new object[] { sep };
                type = buf.sprintf("{0}", objArray3);
            }
            return type;
        }

        public static TdrError.ErrorType printWString(ref TdrVisualBuf buf, int indent, char sep, string variable, int arrIdx, short[] str)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = TdrTypeUtil.wstrlen(str) + 1;
            object[] args = new object[] { variable, arrIdx };
            type = buf.sprintf("{0}[{1:d}]", args);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int num2 = TdrTypeUtil.wstrlen(str);
                for (int i = 0; i < num2; i++)
                {
                    object[] objArray2 = new object[] { str[i] };
                    type = buf.sprintf("0x{0:X4}", objArray2);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        break;
                    }
                }
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                object[] objArray3 = new object[] { sep };
                type = buf.sprintf("{0}", objArray3);
            }
            return type;
        }
    }
}

