namespace Pathfinding.Util
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [StructLayout(LayoutKind.Sequential)]
    public struct Guid
    {
        private const string hex = "0123456789ABCDEF";
        public static readonly Pathfinding.Util.Guid zero;
        public static readonly string zeroString;
        private ulong _a;
        private ulong _b;
        private static Random random;
        private static StringBuilder text;
        public Guid(byte[] bytes)
        {
            this._a = (ulong) (((((((bytes[0] | (bytes[1] << 8)) | (bytes[2] << 0x10)) | (bytes[3] << 0x18)) | (bytes[4] << 0x20)) | (bytes[5] << 40)) | (bytes[6] << 0x30)) | (bytes[7] << 0x38));
            this._b = (ulong) (((((((bytes[8] | (bytes[9] << 8)) | (bytes[10] << 0x10)) | (bytes[11] << 0x18)) | (bytes[12] << 0x20)) | (bytes[13] << 40)) | (bytes[14] << 0x30)) | (bytes[15] << 0x38));
        }

        public Guid(string str)
        {
            this._a = 0L;
            this._b = 0L;
            if (str.Length < 0x20)
            {
                throw new FormatException("Invalid Guid format");
            }
            int num = 0;
            int num2 = 0;
            int num3 = 60;
            while (num < 0x10)
            {
                if (num2 >= str.Length)
                {
                    throw new FormatException("Invalid Guid format. String too short");
                }
                char c = str[num2];
                if (c != '-')
                {
                    int index = "0123456789ABCDEF".IndexOf(char.ToUpperInvariant(c));
                    if (index == -1)
                    {
                        throw new FormatException("Invalid Guid format : " + c + " is not a hexadecimal character");
                    }
                    this._a |= ((ulong) index) << num3;
                    num3 -= 4;
                    num++;
                }
                num2++;
            }
            num3 = 60;
            while (num < 0x20)
            {
                if (num2 >= str.Length)
                {
                    throw new FormatException("Invalid Guid format. String too short");
                }
                char ch2 = str[num2];
                if (ch2 != '-')
                {
                    int num5 = "0123456789ABCDEF".IndexOf(char.ToUpperInvariant(ch2));
                    if (num5 == -1)
                    {
                        throw new FormatException("Invalid Guid format : " + ch2 + " is not a hexadecimal character");
                    }
                    this._b |= ((ulong) num5) << num3;
                    num3 -= 4;
                    num++;
                }
                num2++;
            }
        }

        static Guid()
        {
            zero = new Pathfinding.Util.Guid(new byte[0x10]);
            zeroString = new Pathfinding.Util.Guid(new byte[0x10]).ToString();
            random = new Random();
        }

        public static Pathfinding.Util.Guid Parse(string input)
        {
            return new Pathfinding.Util.Guid(input);
        }

        public byte[] ToByteArray()
        {
            byte[] buffer = new byte[0x10];
            byte[] bytes = BitConverter.GetBytes(this._a);
            byte[] buffer3 = BitConverter.GetBytes(this._b);
            for (int i = 0; i < 8; i++)
            {
                buffer[i] = bytes[i];
                buffer[i + 8] = buffer3[i];
            }
            return buffer;
        }

        public static Pathfinding.Util.Guid NewGuid()
        {
            byte[] buffer = new byte[0x10];
            random.NextBytes(buffer);
            return new Pathfinding.Util.Guid(buffer);
        }

        public override bool Equals(object _rhs)
        {
            if (!(_rhs is Pathfinding.Util.Guid))
            {
                return false;
            }
            Pathfinding.Util.Guid guid = (Pathfinding.Util.Guid) _rhs;
            return ((this._a == guid._a) && (this._b == guid._b));
        }

        public override int GetHashCode()
        {
            ulong num = this._a ^ this._b;
            return (((int) (num >> 0x20)) ^ ((int) num));
        }

        public override string ToString()
        {
            if (text == null)
            {
                text = new StringBuilder();
            }
            text.Length = 0;
            text.Append(this._a.ToString("x16")).Append('-').Append(this._b.ToString("x16"));
            return text.ToString();
        }

        public static bool operator ==(Pathfinding.Util.Guid lhs, Pathfinding.Util.Guid rhs)
        {
            return ((lhs._a == rhs._a) && (lhs._b == rhs._b));
        }

        public static bool operator !=(Pathfinding.Util.Guid lhs, Pathfinding.Util.Guid rhs)
        {
            return ((lhs._a != rhs._a) || (lhs._b != rhs._b));
        }
    }
}

