namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class ApolloSnsService : ApolloObject, IApolloSnsService, IApolloServiceBase
    {
        public static readonly ApolloSnsService Instance = new ApolloSnsService();

        public event OnBindGroupNotifyHandle onBindGroupEvent;

        public event OnQueryGroupInfoNotifyHandle onQueryGroupInfoEvent;

        public event OnQueryGroupKeyNotifyHandle onQueryGroupKeyEvent;

        public event OnRelationNotifyHandle onRelationEvent;

        public event OnApolloShareEvenHandle onShareEvent;

        public event OnUnbindGroupNotifyHandle onUnBindGroupEvent;

        private ApolloSnsService()
        {
        }

        public void AddGameFriendToQQ(string cFopenid, string cDesc, string cMessage)
        {
            Apollo_Sns_AddGameFriendToQQ(base.ObjectId, cFopenid, cDesc, cMessage);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_AddGameFriendToQQ(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cFopenid, [MarshalAs(UnmanagedType.LPStr)] string cDesc, [MarshalAs(UnmanagedType.LPStr)] string cMessage);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_BindQQGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cUnionid, [MarshalAs(UnmanagedType.LPStr)] string cUnion_name, [MarshalAs(UnmanagedType.LPStr)] string cZoneid, [MarshalAs(UnmanagedType.LPStr)] string cSignature);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_JoinQQGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string qqGroupKey);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool Apollo_Sns_QueryGameFriendsInfo(ulong objId, ApolloPlatform platform);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool Apollo_Sns_QueryMyInfo(ulong objId, ApolloPlatform platform);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_QueryQQGroupInfo(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cUnionid, [MarshalAs(UnmanagedType.LPStr)] string cZoneid);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_QueryQQGroupKey(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cGroupOpenid);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool Apollo_Sns_SendToQQGameFriend(ulong objId, int act, [MarshalAs(UnmanagedType.LPStr)] string fopenid, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string summary, [MarshalAs(UnmanagedType.LPStr)] string targetUrl, [MarshalAs(UnmanagedType.LPStr)] string imgUrl, [MarshalAs(UnmanagedType.LPStr)] string previewText, [MarshalAs(UnmanagedType.LPStr)] string gameTag, [MarshalAs(UnmanagedType.LPStr)] string msdkExtInfo);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_SendToQQWithMusic(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string musicUrl, [MarshalAs(UnmanagedType.LPStr)] string musicDataUrl, [MarshalAs(UnmanagedType.LPStr)] string imgUrl);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_SendToWeixinWithMusic(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string musicUrl, [MarshalAs(UnmanagedType.LPStr)] string musicDataUrl, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] imgData, int imgDataLen, [MarshalAs(UnmanagedType.LPStr)] string messageExt, [MarshalAs(UnmanagedType.LPStr)] string messageAction);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_SendToWeixinWithUrl(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string Url, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] imgData, int imgDataLen, [MarshalAs(UnmanagedType.LPStr)] string messageExt);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool Apollo_Sns_SendToWXGameFriend(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string fOpenId, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string description, [MarshalAs(UnmanagedType.LPStr)] string mediaId, [MarshalAs(UnmanagedType.LPStr)] string messageExt, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPStr)] string msdkExtInfo);
        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void Apollo_Sns_UnbindQQGroup(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string cGroupOpenid, [MarshalAs(UnmanagedType.LPStr)] string cUnionid);
        public void BindQQGroup(string cUnionid, string cUnion_name, string cZoneid, string cSignature)
        {
            Apollo_Sns_BindQQGroup(base.ObjectId, cUnionid, cUnion_name, cZoneid, cSignature);
        }

        public void JoinQQGroup(string qqGroupKey)
        {
            Apollo_Sns_JoinQQGroup(base.ObjectId, qqGroupKey);
        }

        private void OnBindGroupNotify(string msg)
        {
            ADebug.Log("OnBindGroupNotify");
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloGroupResult groupRet = null;
                groupRet = parser.GetObject<ApolloGroupResult>("GroupResult");
                if (this.onBindGroupEvent != null)
                {
                    try
                    {
                        this.onBindGroupEvent(groupRet);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("OnBindGroupNotify:" + exception);
                    }
                }
            }
        }

        private void OnQueryGroupInfoNotify(string msg)
        {
            ADebug.Log("OnQueryGroupInfoNotify");
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloGroupResult groupRet = null;
                groupRet = parser.GetObject<ApolloGroupResult>("GroupResult");
                if (this.onQueryGroupInfoEvent != null)
                {
                    try
                    {
                        this.onQueryGroupInfoEvent(groupRet);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("OnQueryGroupInfoNotify:" + exception);
                    }
                }
            }
        }

        private void OnQueryGroupKeyNotify(string msg)
        {
            ADebug.Log("OnQueryGroupKeyNotify");
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloGroupResult groupRet = null;
                groupRet = parser.GetObject<ApolloGroupResult>("GroupResult");
                if (this.onQueryGroupKeyEvent != null)
                {
                    try
                    {
                        this.onQueryGroupKeyEvent(groupRet);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("OnQueryGroupKeyNotify:" + exception);
                    }
                }
            }
        }

        private void OnRelationNotify(string msg)
        {
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloRelation aRelation = null;
                aRelation = parser.GetObject<ApolloRelation>("Relation");
                if (this.onRelationEvent != null)
                {
                    try
                    {
                        this.onRelationEvent(aRelation);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("OnRelationNotify:" + exception);
                    }
                }
            }
        }

        private void OnShareNotify(string msg)
        {
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloShareResult shareResponseInfo = null;
                shareResponseInfo = parser.GetObject<ApolloShareResult>("ShareResult");
                if (this.onShareEvent != null)
                {
                    try
                    {
                        this.onShareEvent(shareResponseInfo);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("onShareEvent:" + exception);
                    }
                }
            }
        }

        private void OnUnbindGroupNotify(string msg)
        {
            ADebug.Log("OnUnbindGroupNotify");
            if (msg.Length > 0)
            {
                ApolloStringParser parser = new ApolloStringParser(msg);
                ApolloGroupResult groupRet = null;
                groupRet = parser.GetObject<ApolloGroupResult>("GroupResult");
                if (this.onUnBindGroupEvent != null)
                {
                    try
                    {
                        this.onUnBindGroupEvent(groupRet);
                    }
                    catch (Exception exception)
                    {
                        ADebug.Log("OnUnbindGroupNotify:" + exception);
                    }
                }
            }
        }

        public bool QueryGameFriendsInfo(ApolloPlatform platform)
        {
            return Apollo_Sns_QueryGameFriendsInfo(base.ObjectId, platform);
        }

        public bool QueryMyInfo(ApolloPlatform platform)
        {
            return Apollo_Sns_QueryMyInfo(base.ObjectId, platform);
        }

        public void QueryQQGroupInfo(string cUnionid, string cZoneid)
        {
            Apollo_Sns_QueryQQGroupInfo(base.ObjectId, cUnionid, cZoneid);
        }

        public void QueryQQGroupKey(string cGroupOpenid)
        {
            Apollo_Sns_QueryQQGroupKey(base.ObjectId, cGroupOpenid);
        }

        public void SendToQQ(ApolloShareScene scene, string title, string desc, string url, string thumbImageUrl)
        {
            Console.WriteLine("ApolloSnsLZK SendToQQ:{0}", thumbImageUrl.Length);
            SendToQQ(base.ObjectId, scene, title, desc, url, thumbImageUrl, thumbImageUrl.Length);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SendToQQ(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string imgUrl, int imgUrlLen);
        public bool SendToQQGameFriend(int act, string fopenid, string title, string summary, string targetUrl, string imgUrl, string previewText, string gameTag, string msdkExtInfo)
        {
            ADebug.Log(string.Concat(new object[] { 
                "CApolloSnsService::SendToQQGameFriend act:", act, "fopenid:", fopenid, "title:", title, "summary:", summary, "targetUrl:", targetUrl, "imgUrl:", imgUrl, "previewText:", previewText, "gameTag:", gameTag, 
                "msdkExtInfo:", msdkExtInfo
             }));
            return Apollo_Sns_SendToQQGameFriend(base.ObjectId, act, fopenid, title, summary, targetUrl, imgUrl, previewText, gameTag, msdkExtInfo);
        }

        public void SendToQQWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string imgUrl)
        {
            Apollo_Sns_SendToQQWithMusic(base.ObjectId, aScene, title, desc, musicUrl, musicDataUrl, imgUrl);
        }

        public void SendToQQWithPhoto(ApolloShareScene scene, string imgFilePath)
        {
            SendToQQWithPhoto(base.ObjectId, scene, imgFilePath);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SendToQQWithPhoto(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string imgFilePath);
        public void SendToWeixin(string title, string desc, string mediaTagName, byte[] thumbImgData, int thumbDataLen, string extInfo)
        {
            ADebug.Log("ApolloSnsLZK SendToWeixin");
            SendToWeixin(base.ObjectId, title, desc, mediaTagName, thumbImgData, thumbDataLen, extInfo);
        }

        public void SendToWeixin(ApolloShareScene scene, string title, string desc, string url, string mediaTagName, byte[] thumbImgData, int thumbDataLen)
        {
            throw new Exception("Api Not supported at current version");
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SendToWeixin(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string title, [MarshalAs(UnmanagedType.LPStr)] string desc, [MarshalAs(UnmanagedType.LPStr)] string mediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] thumbImgData, int thumbDataLen, [MarshalAs(UnmanagedType.LPStr)] string extInfo);
        public void SendToWeixinWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction)
        {
            ADebug.Log("CApolloSnsService::SendToWeixinWithMusic title:" + title + "desc:" + desc + "musicUrl:" + musicUrl + "musicDataUrl:" + musicDataUrl + "mediaTagName:" + mediaTagName + "messageExt:" + messageExt + "messageAction:" + messageAction);
            Apollo_Sns_SendToWeixinWithMusic(base.ObjectId, aScene, title, desc, musicUrl, musicDataUrl, mediaTagName, imageData, imageDataLen, messageExt, messageAction);
        }

        public void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imgDataLen)
        {
            SendToWeixinWithPhoto(base.ObjectId, aScene, mediaTagName, imageData, imgDataLen);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SendToWeixinWithPhoto(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string pszMediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] pImgData, int nImgDataLen);
        public void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction)
        {
            SendToWeixinWithPhotoWithTail(base.ObjectId, aScene, mediaTagName, imageData, imageDataLen, messageExt, messageAction);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern void SendToWeixinWithPhotoWithTail(ulong objId, ApolloShareScene scene, [MarshalAs(UnmanagedType.LPStr)] string pszMediaTagName, [MarshalAs(UnmanagedType.LPArray)] byte[] pImgData, int nImgDataLen, [MarshalAs(UnmanagedType.LPStr)] string messageExt, [MarshalAs(UnmanagedType.LPStr)] string messageAction);
        public void SendToWeixinWithUrl(ApolloShareScene aScene, string title, string desc, string url, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt)
        {
            Apollo_Sns_SendToWeixinWithUrl(base.ObjectId, aScene, title, desc, url, mediaTagName, imageData, imageDataLen, messageExt);
        }

        public bool SendToWXGameFriend(string fOpenId, string title, string description, string mediaId, string messageExt, string mediaTagName, string msdkExtInfo)
        {
            ADebug.Log("CApolloSnsService::SendToWXGameFriend fOpenId:" + fOpenId + "title:" + title + "description:" + description + "mediaId:" + mediaId + "messageExt:" + messageExt + "mediaTagName:" + mediaTagName + "msdkExtInfo:" + msdkExtInfo);
            return Apollo_Sns_SendToWXGameFriend(base.ObjectId, fOpenId, title, description, mediaId, messageExt, mediaTagName, msdkExtInfo);
        }

        public void UnbindQQGroup(string cGroupOpenid, string cUnionid)
        {
            Apollo_Sns_UnbindQQGroup(base.ObjectId, cGroupOpenid, cUnionid);
        }
    }
}

