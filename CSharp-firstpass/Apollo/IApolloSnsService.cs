namespace Apollo
{
    using System;

    public interface IApolloSnsService : IApolloServiceBase
    {
        event OnBindGroupNotifyHandle onBindGroupEvent;

        event OnQueryGroupInfoNotifyHandle onQueryGroupInfoEvent;

        event OnQueryGroupKeyNotifyHandle onQueryGroupKeyEvent;

        event OnRelationNotifyHandle onRelationEvent;

        event OnApolloShareEvenHandle onShareEvent;

        event OnUnbindGroupNotifyHandle onUnBindGroupEvent;

        void AddGameFriendToQQ(string cFopenid, string cDesc, string cMessage);
        void BindQQGroup(string cUnionid, string cUnion_name, string cZoneid, string cSignature);
        void JoinQQGroup(string qqGroupKey);
        bool QueryGameFriendsInfo(ApolloPlatform platform);
        bool QueryMyInfo(ApolloPlatform platform);
        void QueryQQGroupInfo(string cUnionid, string cZoneid);
        void QueryQQGroupKey(string cGroupOpenid);
        void SendToQQ(ApolloShareScene scene, string title, string desc, string url, string thumbImageUrl);
        bool SendToQQGameFriend(int act, string fopenid, string title, string summary, string targetUrl, string imgUrl, string previewText, string gameTag, string msdkExtInfo);
        void SendToQQWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string imgUrl);
        void SendToQQWithPhoto(ApolloShareScene scene, string imgFilePath);
        void SendToWeixin(string title, string desc, string mediaTagName, byte[] thumbImgData, int thumbDataLen, string extInfo);
        void SendToWeixinWithMusic(ApolloShareScene aScene, string title, string desc, string musicUrl, string musicDataUrl, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction);
        void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imageDataLen);
        void SendToWeixinWithPhoto(ApolloShareScene aScene, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt, string messageAction);
        void SendToWeixinWithUrl(ApolloShareScene aScene, string title, string desc, string url, string mediaTagName, byte[] imageData, int imageDataLen, string messageExt);
        bool SendToWXGameFriend(string fOpenId, string title, string description, string mediaId, string messageExt, string mediaTagName, string msdkExtInfo);
        void UnbindQQGroup(string cGroupOpenid, string cUnionid);
    }
}

