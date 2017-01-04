namespace Gif
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Loader
    {
        public List<GifFrame> _frames = new List<GifFrame>();
        public Color[] _global_color_table;
        public string _header = string.Empty;
        private ERROR _last_error;
        public LogicalScreenDesc _logical_screen_desc;
        private BinaryReader _reader;
        private STATUS _status;
        public static readonly int[] POW = new int[] { 1, 2, 4, 8, 0x10, 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800 };

        public bool AsyncLoad(Stream stream)
        {
            this.SetError(ERROR.OK);
            this.SetStatus(STATUS.LOADING);
            this._reader = new BinaryReader(stream);
            this.LoadHeader();
            this.SetStatus(STATUS.LOADED_HEADER);
            if (this._last_error != ERROR.OK)
            {
                this._reader.Close();
                this._reader = null;
                this.SetStatus(STATUS.DONE);
                return false;
            }
            return true;
        }

        public bool AsyncNextLoad()
        {
            if ((this._last_error != ERROR.OK) || (this._reader == null))
            {
                return false;
            }
            switch (this._status)
            {
                case STATUS.LOADED_HEADER:
                    this.LoadLogicalScreenDescriptor();
                    this.SetStatus(STATUS.LOADED_SCREEN_DESC);
                    break;

                case STATUS.LOADED_SCREEN_DESC:
                    this.LoadGlobalColorTable();
                    this.SetStatus(STATUS.LOADED_GLOBAL_COLOR_TABLE);
                    break;

                case STATUS.LOADED_GLOBAL_COLOR_TABLE:
                    this.SetStatus(STATUS.LOADING_FRAME);
                    break;

                case STATUS.LOADING_FRAME:
                {
                    ERROR error = this.LoadFrame();
                    if (error != ERROR.OK)
                    {
                        this._reader.Close();
                        this._reader = null;
                        this.SetStatus(STATUS.DONE);
                        if (error != ERROR.FOUND_TRAILER)
                        {
                            this.SetError(error);
                        }
                        else
                        {
                            return true;
                        }
                        break;
                    }
                    break;
                }
            }
            return (this._last_error == ERROR.OK);
        }

        public ERROR GetLastError()
        {
            return this._last_error;
        }

        public STATUS GetStatus()
        {
            return this._status;
        }

        public bool HasError()
        {
            return (this._last_error != ERROR.OK);
        }

        private void ImageDataFlipY(ref Image image)
        {
            byte[] destinationArray = new byte[image.desc.image_width * image.desc.image_height];
            if (image.desc.interlace_flag)
            {
                int num = 0;
                int num2 = 0;
                while (num2 < image.desc.image_height)
                {
                    int sourceIndex = num * image.desc.image_width;
                    int destinationIndex = ((image.desc.image_height - 1) - num2) * image.desc.image_width;
                    Array.Copy(image.data, sourceIndex, destinationArray, destinationIndex, image.desc.image_width);
                    num2 += 8;
                    num++;
                }
                int num5 = 4;
                while (num5 < image.desc.image_height)
                {
                    int num6 = num * image.desc.image_width;
                    int num7 = ((image.desc.image_height - 1) - num5) * image.desc.image_width;
                    Array.Copy(image.data, num6, destinationArray, num7, image.desc.image_width);
                    num5 += 8;
                    num++;
                }
                int num8 = 2;
                while (num8 < image.desc.image_height)
                {
                    int num9 = num * image.desc.image_width;
                    int num10 = ((image.desc.image_height - 1) - num8) * image.desc.image_width;
                    Array.Copy(image.data, num9, destinationArray, num10, image.desc.image_width);
                    num8 += 4;
                    num++;
                }
                int num11 = 1;
                while (num11 < image.desc.image_height)
                {
                    int num12 = num * image.desc.image_width;
                    int num13 = ((image.desc.image_height - 1) - num11) * image.desc.image_width;
                    Array.Copy(image.data, num12, destinationArray, num13, image.desc.image_width);
                    num11 += 2;
                    num++;
                }
            }
            else
            {
                for (int i = 0; i < image.desc.image_height; i++)
                {
                    int num15 = i * image.desc.image_width;
                    int num16 = ((image.desc.image_height - 1) - i) * image.desc.image_width;
                    Array.Copy(image.data, num15, destinationArray, num16, image.desc.image_width);
                }
            }
            image.desc.image_top = (this._logical_screen_desc.image_height - image.desc.image_top) - image.desc.image_height;
            image.data = destinationArray;
        }

        public bool IsDone()
        {
            return (this._status == STATUS.DONE);
        }

        public bool IsDoneWithoutError()
        {
            return ((this._status == STATUS.DONE) && (this._last_error == ERROR.OK));
        }

        public bool IsLoading()
        {
            return (((this._status > STATUS.VOID) && (this._status < STATUS.DONE)) && (this._last_error == ERROR.OK));
        }

        public bool IsVoid()
        {
            return (this._status == STATUS.VOID);
        }

        public bool Load(Stream stream)
        {
            this.SetError(ERROR.OK);
            this.SetStatus(STATUS.LOADING);
            this._reader = new BinaryReader(stream);
            if ((this.LoadHeader() && this.LoadLogicalScreenDescriptor()) && this.LoadGlobalColorTable())
            {
                this.LoadFrames();
            }
            this._reader.Close();
            this._reader = null;
            this.SetStatus(STATUS.DONE);
            return (this._last_error == ERROR.OK);
        }

        private ERROR LoadFrame()
        {
            GifFrame item = new GifFrame();
            ERROR error = item.Load(this._reader);
            if (error == ERROR.OK)
            {
                this._frames.Add(item);
                this.ImageDataFlipY(ref item._image);
                return error;
            }
            item = null;
            return error;
        }

        private bool LoadFrames()
        {
            ERROR error;
        Label_0000:
            error = this.LoadFrame();
            switch (error)
            {
                case ERROR.FOUND_TRAILER:
                    break;

                case ERROR.OK:
                    goto Label_0000;

                default:
                    this.SetError(error);
                    break;
            }
            return (this._last_error == ERROR.OK);
        }

        private bool LoadGlobalColorTable()
        {
            bool flag = this._logical_screen_desc.global_color_table_flag;
            int num = this._logical_screen_desc.global_color_table_size;
            if (flag)
            {
                this._global_color_table = new Color[num];
                for (int i = 0; i < num; i++)
                {
                    this._global_color_table[i].r = this._reader.ReadByte();
                    this._global_color_table[i].g = this._reader.ReadByte();
                    this._global_color_table[i].b = this._reader.ReadByte();
                }
            }
            return true;
        }

        private bool LoadHeader()
        {
            foreach (char ch in this._reader.ReadChars(6))
            {
                this._header = this._header + ch;
            }
            if (this._header != "GIF89a")
            {
                this.SetError(ERROR.NO_SUPPORT_VERSION);
                return false;
            }
            return true;
        }

        private bool LoadLogicalScreenDescriptor()
        {
            this._logical_screen_desc.image_width = this._reader.ReadInt16();
            this._logical_screen_desc.image_height = this._reader.ReadInt16();
            int num = this._reader.ReadByte();
            int num2 = 0;
            num2 = 7 & num;
            this._logical_screen_desc.global_color_table_size = POW[num2 + 1];
            num2 = (8 & num) >> 3;
            this._logical_screen_desc.color_table_sort_flag = num2 == 1;
            num2 = (0x70 & num) >> 4;
            this._logical_screen_desc.bit_per_pixel = num2 + 1;
            num2 = (0x80 & num) >> 7;
            this._logical_screen_desc.global_color_table_flag = num2 == 1;
            this._logical_screen_desc.background_color = this._reader.ReadByte();
            this._logical_screen_desc.pixel_aspect_ratio = this._reader.ReadByte();
            return true;
        }

        public static byte[] ReadBlock(BinaryReader reader)
        {
            List<byte> list = new List<byte>();
            while (true)
            {
                byte[] buffer = ReadSubBlock(reader);
                if (buffer == null)
                {
                    return list.ToArray();
                }
                foreach (byte num in buffer)
                {
                    list.Add(num);
                }
            }
        }

        public static byte[] ReadSubBlock(BinaryReader reader)
        {
            byte[] buffer = null;
            int count = reader.ReadByte();
            if (count > 0)
            {
                buffer = reader.ReadBytes(count);
            }
            return buffer;
        }

        private void SetError(ERROR error)
        {
            this._last_error = error;
        }

        private void SetStatus(STATUS status)
        {
            this._status = status;
        }

        public static ERROR SkipBlock(BinaryReader reader)
        {
            while (true)
            {
                int count = reader.ReadByte();
                if (count == 0)
                {
                    return ERROR.OK;
                }
                reader.ReadBytes(count);
            }
        }
    }
}

