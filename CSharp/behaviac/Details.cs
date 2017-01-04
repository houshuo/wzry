namespace behaviac
{
    using System;

    public static class Details
    {
        private static DictionaryView<System.Type, ICompareValue> ms_comparers;
        private static DictionaryView<System.Type, IComputeValue> ms_computers;

        public static object ComputeValue(object value1, object value2, EComputeOperator opr)
        {
            System.Type key = value1.GetType();
            if (ms_computers.ContainsKey(key))
            {
                IComputeValue value3 = ms_computers[key];
                switch (opr)
                {
                    case EComputeOperator.E_ADD:
                        return value3.Add(value1, value2);

                    case EComputeOperator.E_SUB:
                        return value3.Sub(value1, value2);

                    case EComputeOperator.E_MUL:
                        return value3.Mul(value1, value2);

                    case EComputeOperator.E_DIV:
                        return value3.Div(value1, value2);
                }
            }
            return null;
        }

        public static bool Equal(object lhs, object rhs)
        {
            return object.Equals(lhs, rhs);
        }

        public static bool Greater(object lhs, object rhs)
        {
            System.Type key = lhs.GetType();
            if (ms_comparers.ContainsKey(key))
            {
                ICompareValue value2 = ms_comparers[key];
                return value2.Greater(lhs, rhs);
            }
            return false;
        }

        public static bool GreaterEqual(object lhs, object rhs)
        {
            System.Type key = lhs.GetType();
            if (ms_comparers.ContainsKey(key))
            {
                ICompareValue value2 = ms_comparers[key];
                return value2.GreaterEqual(lhs, rhs);
            }
            return false;
        }

        public static bool Less(object lhs, object rhs)
        {
            System.Type key = lhs.GetType();
            if (ms_comparers.ContainsKey(key))
            {
                ICompareValue value2 = ms_comparers[key];
                return value2.Less(lhs, rhs);
            }
            return false;
        }

        public static bool LessEqual(object lhs, object rhs)
        {
            System.Type key = lhs.GetType();
            if (ms_comparers.ContainsKey(key))
            {
                ICompareValue value2 = ms_comparers[key];
                return value2.LessEqual(lhs, rhs);
            }
            return false;
        }

        public static void RegisterCompareValue()
        {
            ms_comparers = new DictionaryView<System.Type, ICompareValue>();
            ms_comparers[typeof(int)] = new CompareValueInt();
            ms_comparers[typeof(long)] = new CompareValueLong();
            ms_comparers[typeof(short)] = new CompareValueShort();
            ms_comparers[typeof(sbyte)] = new CompareValueByte();
            ms_comparers[typeof(float)] = new CompareValueFloat();
            ms_comparers[typeof(uint)] = new CompareValueUInt();
            ms_comparers[typeof(ulong)] = new CompareValueULong();
            ms_comparers[typeof(ushort)] = new CompareValueUShort();
            ms_comparers[typeof(byte)] = new CompareValueUByte();
            ms_comparers[typeof(double)] = new CompareValueDouble();
        }

        public static void RegisterComputeValue()
        {
            ms_computers = new DictionaryView<System.Type, IComputeValue>();
            ms_computers[typeof(int)] = new ComputeValueInt();
            ms_computers[typeof(long)] = new ComputeValueLong();
            ms_computers[typeof(short)] = new ComputeValueShort();
            ms_computers[typeof(sbyte)] = new ComputeValueByte();
            ms_computers[typeof(float)] = new ComputeValueFloat();
            ms_computers[typeof(uint)] = new ComputeValueUInt();
            ms_computers[typeof(ulong)] = new ComputeValueULong();
            ms_computers[typeof(ushort)] = new ComputeValueUShort();
            ms_computers[typeof(byte)] = new ComputeValueUByte();
            ms_computers[typeof(double)] = new ComputeValueDouble();
        }

        private class CompareValueByte : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((sbyte) lhs) > ((sbyte) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((sbyte) lhs) >= ((sbyte) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((sbyte) lhs) < ((sbyte) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((sbyte) lhs) <= ((sbyte) rhs));
            }
        }

        private class CompareValueDouble : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((double) lhs) > ((double) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((double) lhs) >= ((double) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((double) lhs) < ((double) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((double) lhs) <= ((double) rhs));
            }
        }

        private class CompareValueFloat : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((float) lhs) > ((float) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((float) lhs) >= ((float) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((float) lhs) < ((float) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((float) lhs) <= ((float) rhs));
            }
        }

        private class CompareValueInt : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((int) lhs) > ((int) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((int) lhs) >= ((int) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((int) lhs) < ((int) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((int) lhs) <= ((int) rhs));
            }
        }

        private class CompareValueLong : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((long) lhs) > ((long) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((long) lhs) >= ((long) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((long) lhs) < ((long) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((long) lhs) <= ((long) rhs));
            }
        }

        private class CompareValueShort : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((short) lhs) > ((short) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((short) lhs) >= ((short) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((short) lhs) < ((short) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((short) lhs) <= ((short) rhs));
            }
        }

        private class CompareValueUByte : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((byte) lhs) > ((byte) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((byte) lhs) >= ((byte) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((byte) lhs) < ((byte) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((byte) lhs) <= ((byte) rhs));
            }
        }

        private class CompareValueUInt : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((uint) lhs) > ((uint) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((uint) lhs) >= ((uint) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((uint) lhs) < ((uint) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((uint) lhs) <= ((uint) rhs));
            }
        }

        private class CompareValueULong : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((ulong) lhs) > ((ulong) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((ulong) lhs) >= ((ulong) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((ulong) lhs) < ((ulong) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((ulong) lhs) <= ((ulong) rhs));
            }
        }

        private class CompareValueUShort : Details.ICompareValue
        {
            public bool Greater(object lhs, object rhs)
            {
                return (((ushort) lhs) > ((ushort) rhs));
            }

            public bool GreaterEqual(object lhs, object rhs)
            {
                return (((ushort) lhs) >= ((ushort) rhs));
            }

            public bool Less(object lhs, object rhs)
            {
                return (((ushort) lhs) < ((ushort) rhs));
            }

            public bool LessEqual(object lhs, object rhs)
            {
                return (((ushort) lhs) <= ((ushort) rhs));
            }
        }

        private class ComputeValueByte : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (sbyte) (((sbyte) lhs) + ((sbyte) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (sbyte) (((int) ((sbyte) lhs)) / ((int) ((sbyte) rhs)));
            }

            public object Mul(object lhs, object rhs)
            {
                return (sbyte) (((sbyte) lhs) * ((sbyte) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (sbyte) (((sbyte) lhs) - ((sbyte) rhs));
            }
        }

        private class ComputeValueDouble : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (((double) lhs) + ((double) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (((double) lhs) / ((double) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (((double) lhs) * ((double) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (((double) lhs) - ((double) rhs));
            }
        }

        private class ComputeValueFloat : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (((float) lhs) + ((float) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (((float) lhs) / ((float) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (((float) lhs) * ((float) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (((float) lhs) - ((float) rhs));
            }
        }

        private class ComputeValueInt : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (((int) lhs) + ((int) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (((int) lhs) / ((int) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (((int) lhs) * ((int) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (((int) lhs) - ((int) rhs));
            }
        }

        private class ComputeValueLong : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (((long) lhs) + ((long) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (((long) lhs) / ((long) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (((long) lhs) * ((long) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (((long) lhs) - ((long) rhs));
            }
        }

        private class ComputeValueShort : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (short) (((short) lhs) + ((short) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (short) (((short) lhs) / ((short) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (short) (((short) lhs) * ((short) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (short) (((short) lhs) - ((short) rhs));
            }
        }

        private class ComputeValueUByte : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (byte) (((byte) lhs) + ((byte) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (byte) (((byte) lhs) / ((byte) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (byte) (((byte) lhs) * ((byte) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (byte) (((byte) lhs) - ((byte) rhs));
            }
        }

        private class ComputeValueUInt : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (((uint) lhs) + ((uint) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (((uint) lhs) / ((uint) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (((uint) lhs) * ((uint) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (((uint) lhs) - ((uint) rhs));
            }
        }

        private class ComputeValueULong : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (((ulong) lhs) + ((ulong) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (((ulong) lhs) / ((ulong) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (((ulong) lhs) * ((ulong) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (((ulong) lhs) - ((ulong) rhs));
            }
        }

        private class ComputeValueUShort : Details.IComputeValue
        {
            public object Add(object lhs, object rhs)
            {
                return (ushort) (((ushort) lhs) + ((ushort) rhs));
            }

            public object Div(object lhs, object rhs)
            {
                return (ushort) (((ushort) lhs) / ((ushort) rhs));
            }

            public object Mul(object lhs, object rhs)
            {
                return (ushort) (((ushort) lhs) * ((ushort) rhs));
            }

            public object Sub(object lhs, object rhs)
            {
                return (ushort) (((ushort) lhs) - ((ushort) rhs));
            }
        }

        private interface ICompareValue
        {
            bool Greater(object lhs, object rhs);
            bool GreaterEqual(object lhs, object rhs);
            bool Less(object lhs, object rhs);
            bool LessEqual(object lhs, object rhs);
        }

        private interface IComputeValue
        {
            object Add(object opr1, object opr2);
            object Div(object opr1, object opr2);
            object Mul(object opr1, object opr2);
            object Sub(object opr1, object opr2);
        }
    }
}

