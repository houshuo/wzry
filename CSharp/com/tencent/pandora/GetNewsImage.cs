namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class GetNewsImage : MonoBehaviour
    {
        public static Dictionary<string, bool> dicFailImgs = new Dictionary<string, bool>();
        public static Dictionary<string, bool> dicSuccImgs = new Dictionary<string, bool>();
        public Image gUItexture;
        public Dictionary<string, Image> gUItextures = new Dictionary<string, Image>();
        public bool isLoading;
        public int iSourceImgHeight;
        public int iSourceImgWidth;
        public bool isResize;
        private int iTryNum;
        public List<string> ListPreToLoading = new List<string>();
        public int m_nDownLoadNumber;
        private string mPreFix = string.Empty;
        private string path = FileUtils.path;
        public Dictionary<string, Sprite> picDic = new Dictionary<string, Sprite>();
        private string strCallCurLuaFile = string.Empty;
        public float waitTimeOut = 3f;

        public event callbackFuc OnFailCallBack;

        public event Callback OnFailCallBackWithTex;

        public event callbackFuc OnSuccCallBack;

        public event Callback OnSuccCallBackWithTex;

        private void addNumber()
        {
            this.m_nDownLoadNumber++;
        }

        private void Awake()
        {
            try
            {
                if (!Directory.Exists(Application.temporaryCachePath + "/TPlayCache/"))
                {
                    Directory.CreateDirectory(Application.temporaryCachePath + "/TPlayCache/");
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d(exception.ToString());
            }
        }

        public void BreakCoroutine()
        {
            base.StopAllCoroutines();
        }

        public void CacheImage(string url)
        {
            try
            {
                if (!this.isImageCached(url))
                {
                    base.StartCoroutine(this.DownloadCacheImage(url));
                }
                else
                {
                    dicSuccImgs.Add(url, true);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d(exception.Message.ToString());
            }
        }

        private void CallLuaFailInfo(string strUrl)
        {
            if (this.strCallCurLuaFile != string.Empty)
            {
                try
                {
                    com.tencent.pandora.Logger.d("CallLuaFailInfo 1");
                    object[] args = new object[] { strUrl };
                    Util.CallMethod(this.strCallCurLuaFile, "OnPicGetFail", args);
                    com.tencent.pandora.Logger.d("CallLuaFailInfo 2");
                }
                catch (Exception exception)
                {
                    com.tencent.pandora.Logger.d("CallLuaFailInfo:" + exception.Message);
                }
            }
        }

        private void CallLuaFinshInfo(string strUrl)
        {
            try
            {
                if (this.strCallCurLuaFile != string.Empty)
                {
                    try
                    {
                        com.tencent.pandora.Logger.d("CallLuaFailInfo 3");
                        object[] args = new object[] { strUrl };
                        Util.CallMethod(this.strCallCurLuaFile, "OnPicGetFish", args);
                        com.tencent.pandora.Logger.d("CallLuaFailInfo 4");
                    }
                    catch (Exception exception)
                    {
                        com.tencent.pandora.Logger.d("CallLuaFailInfo:" + exception.Message);
                    }
                }
            }
            catch (Exception exception2)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception2.Message);
            }
        }

        private bool checkIsJpg(string strUrl)
        {
            char[] separator = new char[] { '.' };
            string[] strArray = strUrl.Split(separator);
            return ((strArray.Length > 0) && (strArray[strArray.Length - 1].ToLower() == "jpg"));
        }

        [DebuggerHidden]
        private IEnumerator DownloadCacheImage(string url)
        {
            return new <DownloadCacheImage>c__Iterator3 { url = url, <$>url = url, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator DownloadImage(string url, Image texture)
        {
            return new <DownloadImage>c__Iterator1 { url = url, texture = texture, <$>url = url, <$>texture = texture, <>f__this = this };
        }

        public int downloadTotalCnt()
        {
            return this.m_nDownLoadNumber;
        }

        public void GetImage(string strPicName, Image texture, string panelName)
        {
            com.tencent.pandora.Logger.d("getImage BEGIN ");
            char[] separator = new char[] { '#' };
            string[] strArray = strPicName.Split(separator);
            if (strArray.Length >= 3)
            {
                strPicName = strArray[0];
                string url = strArray[1];
                string name = strArray[2];
                if (texture != null)
                {
                    com.tencent.pandora.Logger.d("ShowImageByUrl begin");
                    this.ShowImageByUrl(url, texture, name);
                }
            }
        }

        public string getImageCacheFile(string url)
        {
            if (this.path == string.Empty)
            {
                this.path = FileUtils.path;
            }
            string str = string.Empty;
            if ((url != null) && (url != string.Empty))
            {
                str = url.GetHashCode().ToString();
            }
            return (this.path + this.mPreFix + "_" + str);
        }

        public void GetImageOfMuti(string strPicName, string str_null, Image texture)
        {
            string str = strPicName;
            char[] separator = new char[] { '#' };
            string[] strArray = strPicName.Split(separator);
            if (strArray.Length >= 3)
            {
                strPicName = strArray[0];
                string str2 = strArray[1];
                string panelName = strArray[2];
                this.GetImage(str, texture, panelName);
            }
        }

        public Sprite GetTextureIO(string filePath, Image texture)
        {
            try
            {
                byte[] data = new byte[0];
                data = File.ReadAllBytes(filePath);
                com.tencent.pandora.Logger.d("get local pic bytes.Length = " + data.Length);
                Texture2D textured = new Texture2D((int) texture.rectTransform.sizeDelta.x, (int) texture.rectTransform.sizeDelta.x, TextureFormat.RGB24, false);
                textured.LoadImage(data);
                int x = (int) texture.rectTransform.sizeDelta.x;
                int y = (int) texture.rectTransform.sizeDelta.y;
                Sprite sprite = Sprite.Create(textured, new Rect(0f, 0f, (float) textured.width, (float) textured.height), new Vector2(0f, 0f));
                texture.sprite = sprite;
                return sprite;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("use io load local pic failed : PandoraUITexture's name  = " + texture.name + "/ error :" + exception.Message);
                return null;
            }
        }

        public bool isImageCached(string url)
        {
            try
            {
                if (File.Exists(this.getImageCacheFile(url)))
                {
                    if (new FileInfo(this.getImageCacheFile(url)).Length < 150L)
                    {
                        com.tencent.pandora.Logger.d("return f2");
                        return false;
                    }
                    com.tencent.pandora.Logger.d("return f1");
                    return true;
                }
                com.tencent.pandora.Logger.d("return f2");
                return false;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("return f3");
                com.tencent.pandora.Logger.d(exception.Message);
                return false;
            }
        }

        public static int IsImgDownSucc(string strImgUrl)
        {
            if (dicSuccImgs.ContainsKey(strImgUrl))
            {
                return 0;
            }
            if (dicFailImgs.ContainsKey(strImgUrl))
            {
                return -1;
            }
            return -2;
        }

        [DebuggerHidden]
        private IEnumerator LoadLocalImage(string url, Image texture)
        {
            return new <LoadLocalImage>c__Iterator2 { url = url, texture = texture, <$>url = url, <$>texture = texture, <>f__this = this };
        }

        private void OnDestroy()
        {
            try
            {
                if (this.picDic != null)
                {
                    com.tencent.pandora.Logger.d("内存中存在" + this.picDic.Count + "张图片");
                    if (this.picDic.Count > 0)
                    {
                        foreach (UnityEngine.Object obj2 in this.picDic.Values)
                        {
                            UnityEngine.Object.DestroyImmediate(obj2);
                        }
                    }
                    this.picDic.Clear();
                    Resources.UnloadUnusedAssets();
                }
                com.tencent.pandora.Logger.d("Img OnDestroy");
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message.ToString());
            }
        }

        public void PreDownImg()
        {
            this.iTryNum++;
            if (this.ListPreToLoading.Count != 0)
            {
                string item = this.ListPreToLoading[0];
                this.ListPreToLoading.Remove(item);
                if (this.isImageCached(item))
                {
                    if (!dicSuccImgs.ContainsKey(item))
                    {
                        dicSuccImgs.Add(item, true);
                    }
                    this.PreDownImg();
                }
                else
                {
                    base.StartCoroutine(this.DownloadCacheImage(item));
                }
            }
        }

        private void resetnumber()
        {
            this.m_nDownLoadNumber = 0;
        }

        public void SetLuaFileName(string strFileName)
        {
            this.strCallCurLuaFile = strFileName;
        }

        public void ShowImageByUrl(string url, Image texture, string name)
        {
            try
            {
                this.SetLuaFileName(name);
                com.tencent.pandora.Logger.d("start ShowImageByUrl img :" + url);
                if (((this.picDic != null) && this.picDic.ContainsKey(url)) && (this.picDic[url] != null))
                {
                    com.tencent.pandora.Logger.d("找到内存中相同图片, url: " + url);
                    texture.sprite = this.picDic[url];
                    this.CallLuaFinshInfo(url);
                }
                else
                {
                    if (this.isLoading)
                    {
                    }
                    this.isLoading = true;
                    if (!this.isImageCached(url))
                    {
                        base.StartCoroutine(this.DownloadImage(url, texture));
                    }
                    else
                    {
                        base.StartCoroutine(this.LoadLocalImage(url, texture));
                    }
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message.ToString());
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadCacheImage>c__Iterator3 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>url;
            internal GetNewsImage <>f__this;
            internal AssetBundle <ab>__5;
            internal Exception <ex>__3;
            internal Exception <ex2>__4;
            internal Texture2D <image>__1;
            internal byte[] <pngData>__2;
            internal WWW <www>__0;
            internal string url;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<www>__0 = new WWW(this.url);
                        this.$current = this.<www>__0;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.isLoading = false;
                        if (this.<www>__0.error != null)
                        {
                            if (!GetNewsImage.dicFailImgs.ContainsKey(this.url))
                            {
                                GetNewsImage.dicFailImgs.Add(this.url, false);
                            }
                            com.tencent.pandora.Logger.e("cache error:" + this.<www>__0.error + "," + this.url);
                            break;
                        }
                        this.<image>__1 = this.<www>__0.texture;
                        try
                        {
                            this.<pngData>__2 = null;
                            if (this.<>f__this.checkIsJpg(this.url))
                            {
                                this.<pngData>__2 = this.<image>__1.EncodeToJPG();
                            }
                            else
                            {
                                this.<pngData>__2 = this.<image>__1.EncodeToPNG();
                            }
                            File.WriteAllBytes(this.<>f__this.getImageCacheFile(this.url), this.<pngData>__2);
                            if (!GetNewsImage.dicSuccImgs.ContainsKey(this.url))
                            {
                                GetNewsImage.dicSuccImgs.Add(this.url, true);
                            }
                        }
                        catch (Exception exception)
                        {
                            this.<ex>__3 = exception;
                            if (!GetNewsImage.dicFailImgs.ContainsKey(this.url))
                            {
                                GetNewsImage.dicFailImgs.Add(this.url, false);
                            }
                            com.tencent.pandora.Logger.d(this.<ex>__3.ToString());
                            try
                            {
                                File.Delete(this.<>f__this.getImageCacheFile(this.url));
                            }
                            catch (Exception exception2)
                            {
                                this.<ex2>__4 = exception2;
                                com.tencent.pandora.Logger.d(this.<ex2>__4.ToString());
                            }
                        }
                        this.<ab>__5 = this.<www>__0.assetBundle;
                        this.<www>__0.Dispose();
                        if (this.<ab>__5 != null)
                        {
                            this.<ab>__5.Unload(true);
                        }
                        this.<image>__1 = null;
                        this.<www>__0 = null;
                        break;

                    default:
                        goto Label_0226;
                }
                this.<>f__this.PreDownImg();
                this.$PC = -1;
            Label_0226:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadImage>c__Iterator1 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Image <$>texture;
            internal string <$>url;
            internal GetNewsImage <>f__this;
            internal AssetBundle <ab>__7;
            internal float <downTimeSpan>__0;
            internal Exception <ex>__4;
            internal Exception <ex>__8;
            internal Exception <ex2>__5;
            internal Texture2D <pic>__2;
            internal Sprite <picSprite>__6;
            internal byte[] <pngData>__3;
            internal WWW <www>__1;
            internal Image texture;
            internal string url;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        com.tencent.pandora.Logger.d("downloading new image:" + this.url);
                        if (!object.Equals(this.texture, null))
                        {
                            this.<downTimeSpan>__0 = Time.time;
                            this.<www>__1 = new WWW(this.url);
                            this.$current = this.<www>__1;
                            this.$PC = 2;
                        }
                        else
                        {
                            com.tencent.pandora.Logger.d("pic component is null");
                            this.<>f__this.CallLuaFailInfo(this.url);
                            this.$current = 0;
                            this.$PC = 1;
                        }
                        return true;

                    case 1:
                        break;

                    case 2:
                        this.<>f__this.isLoading = false;
                        try
                        {
                            com.tencent.pandora.Logger.d("downloading  end");
                            if ((Time.time - this.<downTimeSpan>__0) > this.<>f__this.waitTimeOut)
                            {
                                com.tencent.pandora.Logger.d("download timeout");
                                this.<>f__this.CallLuaFailInfo(this.url);
                            }
                            else if (string.IsNullOrEmpty(this.<www>__1.error))
                            {
                                this.<pic>__2 = this.<www>__1.texture;
                                if ((this.<pic>__2 != null) && (this.url != null))
                                {
                                    try
                                    {
                                        this.<pngData>__3 = null;
                                        if (this.<>f__this.checkIsJpg(this.url))
                                        {
                                            com.tencent.pandora.Logger.d("start to EncodeToJPG");
                                            this.<pngData>__3 = this.<pic>__2.EncodeToJPG();
                                        }
                                        else
                                        {
                                            com.tencent.pandora.Logger.d("start to EncodeToPNG");
                                            this.<pngData>__3 = this.<pic>__2.EncodeToPNG();
                                        }
                                        com.tencent.pandora.Logger.d("end encode");
                                        if ((this.<pngData>__3 != null) && (this.<pngData>__3.Length > 0))
                                        {
                                            com.tencent.pandora.Logger.d("save data to local ");
                                            File.WriteAllBytes(this.<>f__this.getImageCacheFile(this.url), this.<pngData>__3);
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        this.<ex>__4 = exception;
                                        com.tencent.pandora.Logger.d("save pic buff error :" + this.<ex>__4.ToString());
                                        try
                                        {
                                            com.tencent.pandora.Logger.d("delete pic buff");
                                            File.Delete(this.<>f__this.getImageCacheFile(this.url));
                                        }
                                        catch (Exception exception2)
                                        {
                                            this.<ex2>__5 = exception2;
                                            com.tencent.pandora.Logger.d("delete pic buff error:" + this.<ex2>__5.ToString());
                                        }
                                    }
                                    this.texture.sprite = null;
                                    com.tencent.pandora.Logger.d("create sprite , url:" + this.url);
                                    this.<picSprite>__6 = Sprite.Create(this.<pic>__2, new Rect(0f, 0f, (float) this.<pic>__2.width, (float) this.<pic>__2.height), new Vector2(0f, 0f));
                                    this.texture.sprite = this.<picSprite>__6;
                                    this.<ab>__7 = this.<www>__1.assetBundle;
                                    this.<www>__1.Dispose();
                                    com.tencent.pandora.Logger.d("ab dispose");
                                    if (this.<ab>__7 != null)
                                    {
                                        this.<ab>__7.Unload(true);
                                    }
                                    this.<>f__this.picDic[this.url] = this.<picSprite>__6;
                                    this.<www>__1 = null;
                                    this.<pic>__2 = null;
                                    this.<picSprite>__6 = null;
                                    if (this.<>f__this.OnSuccCallBack != null)
                                    {
                                        this.<>f__this.OnSuccCallBack(this.url, this.texture.gameObject);
                                    }
                                }
                                else
                                {
                                    com.tencent.pandora.Logger.e("pic component is null");
                                }
                                this.<>f__this.CallLuaFinshInfo(this.url);
                            }
                            else
                            {
                                if (this.<>f__this.OnFailCallBack != null)
                                {
                                    this.<>f__this.OnFailCallBack(this.url, this.texture.gameObject);
                                }
                                com.tencent.pandora.Logger.d("down fail:" + this.<www>__1.error + "," + this.url);
                                this.<>f__this.CallLuaFailInfo(this.url);
                            }
                        }
                        catch (Exception exception3)
                        {
                            this.<ex>__8 = exception3;
                            this.<>f__this.CallLuaFailInfo(this.url);
                            com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, this.<ex>__8.Message.ToString());
                        }
                        break;

                    default:
                        goto Label_0452;
                }
                this.$PC = -1;
            Label_0452:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadLocalImage>c__Iterator2 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Image <$>texture;
            internal string <$>url;
            internal GetNewsImage <>f__this;
            internal Exception <ex>__3;
            internal string <file>__1;
            internal bool <isLoadSucc>__0;
            internal Sprite <pic>__2;
            internal Image texture;
            internal string url;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<isLoadSucc>__0 = false;
                        try
                        {
                            com.tencent.pandora.Logger.d("downloading local image, url:" + this.url);
                            if (!object.Equals(this.texture, null))
                            {
                                this.<file>__1 = this.<>f__this.getImageCacheFile(this.url);
                                if (this.<>f__this.picDic.ContainsKey(this.url) && (this.<>f__this.picDic[this.url] != null))
                                {
                                    this.texture.sprite = this.<>f__this.picDic[this.url];
                                    this.<isLoadSucc>__0 = true;
                                }
                                else
                                {
                                    com.tencent.pandora.Logger.d("start to load pic in local");
                                    this.<pic>__2 = this.<>f__this.GetTextureIO(this.<file>__1, this.texture);
                                    com.tencent.pandora.Logger.d("load end in local");
                                    if (this.<pic>__2 != null)
                                    {
                                        this.<>f__this.picDic[this.url] = this.<pic>__2;
                                        this.<isLoadSucc>__0 = true;
                                    }
                                }
                            }
                            else
                            {
                                com.tencent.pandora.Logger.d("texture component is null");
                            }
                        }
                        catch (Exception exception)
                        {
                            this.<ex>__3 = exception;
                            com.tencent.pandora.Logger.d("downloading local pic error:" + this.<ex>__3.Message);
                        }
                        if (this.<isLoadSucc>__0)
                        {
                            this.<>f__this.CallLuaFinshInfo(this.url);
                        }
                        else
                        {
                            this.<>f__this.CallLuaFailInfo(this.url);
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public delegate void Callback(string strImgUrl, Texture2D goTex);

        public delegate void callbackFuc(string strImgUrl, GameObject goTex);
    }
}

