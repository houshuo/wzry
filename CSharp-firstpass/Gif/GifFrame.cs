namespace Gif
{
    using System;
    using System.IO;
    using System.Text;

    public class GifFrame
    {
        public CommentExtensionData _CME_data;
        public GraphicControlExtensionData _GCE_data;
        public Image _image;

        public ERROR Load(BinaryReader reader)
        {
            int num;
            bool flag = false;
            ERROR oK = ERROR.OK;
        Label_0004:
            num = reader.ReadByte();
            if (num > 0x21)
            {
                switch (num)
                {
                    case 0x2c:
                        oK = this.LoadImage(reader);
                        flag = true;
                        goto Label_0062;

                    case 0x3b:
                        oK = ERROR.FOUND_TRAILER;
                        goto Label_0062;
                }
            }
            else
            {
                switch (num)
                {
                    case 0:
                        goto Label_0004;

                    case 0x21:
                        oK = this.LoadExtension(reader);
                        goto Label_0062;
                }
            }
            oK = ERROR.UNKNOWN_CONTENT_ID;
        Label_0062:
            if ((oK == ERROR.OK) && !flag)
            {
                goto Label_0004;
            }
            return oK;
        }

        private ERROR LoadApplicationExtension(BinaryReader reader)
        {
            Loader.SkipBlock(reader);
            return ERROR.OK;
        }

        private ERROR LoadCommentExtension(BinaryReader reader)
        {
            byte[] bytes = Loader.ReadBlock(reader);
            this._CME_data.comment = Encoding.Default.GetString(bytes);
            return ERROR.OK;
        }

        private ERROR LoadExtension(BinaryReader reader)
        {
            switch (reader.ReadByte())
            {
                case 0xfe:
                    return this.LoadCommentExtension(reader);

                case 0xff:
                    return this.LoadApplicationExtension(reader);

                case 1:
                    return this.LoadPlainTextExtension(reader);

                case 0xf9:
                    return this.LoadGraphicControlExtension(reader);
            }
            return ERROR.UNKNOWN_EXTENSION_ID;
        }

        private ERROR LoadGraphicControlExtension(BinaryReader reader)
        {
            reader.ReadByte();
            int num = reader.ReadByte();
            this._GCE_data.transparent_color_flag = (1 & num) == 1;
            this._GCE_data.user_input_flag = ((0x10 & num) >> 1) == 1;
            this._GCE_data.disposal_method = (0x1c & num) >> 2;
            this._GCE_data.delay_time = reader.ReadInt16();
            if (this._GCE_data.transparent_color_flag)
            {
                this._GCE_data.transparent_color = reader.ReadByte();
            }
            reader.ReadByte();
            return ERROR.OK;
        }

        private ERROR LoadImage(BinaryReader reader)
        {
            ERROR oK = ERROR.OK;
            oK = this.LoadImageDescriptor(reader);
            if (oK == ERROR.OK)
            {
                oK = this.LoadImageData(reader);
            }
            return oK;
        }

        private ERROR LoadImageData(BinaryReader reader)
        {
            ERROR error = new LZW_Decompress().Decompress(this._image.desc.image_width, this._image.desc.image_height, ref this._image.data, reader);
            Loader.SkipBlock(reader);
            return error;
        }

        private ERROR LoadImageDescriptor(BinaryReader reader)
        {
            this._image.desc.image_left = reader.ReadInt16();
            this._image.desc.image_top = reader.ReadInt16();
            this._image.desc.image_width = reader.ReadInt16();
            this._image.desc.image_height = reader.ReadInt16();
            int num = reader.ReadByte();
            int num2 = 0;
            num2 = 7 & num;
            this._image.desc.local_color_table_size = Loader.POW[num2 + 1];
            this._image.desc.sort_flag = ((0x20 & num) >> 5) == 1;
            this._image.desc.interlace_flag = ((0x40 & num) >> 6) == 1;
            this._image.desc.local_color_table_flag = ((0x80 & num) >> 7) == 1;
            this._image.desc.local_color_table = null;
            if (this._image.desc.local_color_table_flag)
            {
                int num3 = this._image.desc.local_color_table_size;
                this._image.desc.local_color_table = new Color[num3];
                for (int i = 0; i < num3; i++)
                {
                    this._image.desc.local_color_table[i].r = reader.ReadByte();
                    this._image.desc.local_color_table[i].g = reader.ReadByte();
                    this._image.desc.local_color_table[i].b = reader.ReadByte();
                }
            }
            return ERROR.OK;
        }

        private ERROR LoadPlainTextExtension(BinaryReader reader)
        {
            Loader.SkipBlock(reader);
            return ERROR.OK;
        }
    }
}

