namespace Apollo
{
    using System;

    public enum ApolloPermission
    {
        AddAlbum = 8,
        AddIdol = 0x10,
        AddOneBlog = 0x20,
        AddPicT = 0x40,
        AddShare = 0x80,
        AddTopic = 0x100,
        All = 0xffffff,
        CheckPageFans = 0x200,
        DelIdol = 0x400,
        DelT = 0x800,
        GetAppFriends = 0x800000,
        GetFansList = 0x1000,
        GetIdolList = 0x2000,
        GetInfo = 0x4000,
        GetIntimateFriendsWeibo = 0x200000,
        GetOhterInfo = 0x8000,
        GetRepostList = 0x10000,
        GetSimpleUserInfo = 4,
        GetUserInfo = 2,
        GetVipInfo = 0x80000,
        GetVipRichInfo = 0x100000,
        ListAlbum = 0x20000,
        MatchNickTipsWeibo = 0x400000,
        None = 0,
        UploadPic = 0x40000
    }
}

