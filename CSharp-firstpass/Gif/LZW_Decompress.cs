namespace Gif
{
    using System;
    using System.IO;

    public class LZW_Decompress
    {
        private byte[] block = new byte[0x100];
        private int blockSize;
        private int MaxStackSize = 0x1000;
        private byte[] pixelStack;
        private short[] prefix;
        private ERROR status;
        private byte[] suffix;

        public ERROR Decompress(int iw, int ih, ref byte[] pixels, BinaryReader reader)
        {
            int num;
            int num2;
            int num3;
            int num4;
            int num5;
            int num6;
            int num7 = -1;
            int num8 = iw * ih;
            if ((pixels == null) || (pixels.Length < num8))
            {
                pixels = new byte[num8];
            }
            if (this.prefix == null)
            {
                this.prefix = new short[this.MaxStackSize];
            }
            if (this.suffix == null)
            {
                this.suffix = new byte[this.MaxStackSize];
            }
            if (this.pixelStack == null)
            {
                this.pixelStack = new byte[this.MaxStackSize + 1];
            }
            int num9 = reader.ReadByte();
            int num10 = ((int) 1) << num9;
            int num11 = num10 + 1;
            int index = num10 + 2;
            int num13 = num7;
            int num14 = num9 + 1;
            int num15 = (((int) 1) << num14) - 1;
            int num16 = 0;
            while (num16 < num10)
            {
                this.prefix[num16] = 0;
                this.suffix[num16] = (byte) num16;
                num16++;
            }
            int num17 = num = num2 = num3 = num4 = num6 = num5 = 0;
            int num18 = 0;
            while (num18 < num8)
            {
                if (num4 == 0)
                {
                    if (num < num14)
                    {
                        if (num2 == 0)
                        {
                            num2 = this.ReadBlock(reader);
                            if (num2 <= 0)
                            {
                                break;
                            }
                            num5 = 0;
                        }
                        num17 += (this.block[num5] & 0xff) << num;
                        num += 8;
                        num5++;
                        num2--;
                        continue;
                    }
                    num16 = num17 & num15;
                    num17 = num17 >> num14;
                    num -= num14;
                    if ((num16 > index) || (num16 == num11))
                    {
                        break;
                    }
                    if (num16 == num10)
                    {
                        num14 = num9 + 1;
                        num15 = (((int) 1) << num14) - 1;
                        index = num10 + 2;
                        num13 = num7;
                        continue;
                    }
                    if (num13 == num7)
                    {
                        this.pixelStack[num4++] = this.suffix[num16];
                        num13 = num16;
                        num3 = num16;
                        continue;
                    }
                    int num19 = num16;
                    if (num16 == index)
                    {
                        this.pixelStack[num4++] = (byte) num3;
                        num16 = num13;
                    }
                    while (num16 > num10)
                    {
                        this.pixelStack[num4++] = this.suffix[num16];
                        num16 = this.prefix[num16];
                    }
                    num3 = this.suffix[num16] & 0xff;
                    if (index >= this.MaxStackSize)
                    {
                        break;
                    }
                    this.pixelStack[num4++] = (byte) num3;
                    this.prefix[index] = (short) num13;
                    this.suffix[index] = (byte) num3;
                    index++;
                    if (((index & num15) == 0) && (index < this.MaxStackSize))
                    {
                        num14++;
                        num15 += index;
                    }
                    num13 = num19;
                }
                num4--;
                pixels[num6++] = this.pixelStack[num4];
                num18++;
            }
            for (num18 = num6; num18 < num8; num18++)
            {
                pixels[num18] = 0;
            }
            return this.status;
        }

        protected int ReadBlock(BinaryReader reader)
        {
            this.blockSize = reader.ReadByte();
            if (this.blockSize > 0)
            {
                this.block = reader.ReadBytes(this.blockSize);
            }
            return this.blockSize;
        }
    }
}

