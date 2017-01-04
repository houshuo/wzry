using System;
using System.Runtime.InteropServices;
using System.Text;

public static class TssSdk
{
    public static bool Is32bit()
    {
        return (IntPtr.Size == 4);
    }

    public static bool Is64bit()
    {
        return (IntPtr.Size == 8);
    }

    public static byte[] String2Bytes(string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }

    [DllImport("tersafe")]
    public static extern void tss_del_report_data(IntPtr info);
    [DllImport("tersafe")]
    private static extern void tss_enable_get_report_data();
    [DllImport("tersafe")]
    public static extern IntPtr tss_get_report_data();
    [DllImport("tersafe")]
    private static extern void tss_log_str(string sdk_version);
    [DllImport("tersafe")]
    private static extern AntiDecryptResult tss_sdk_decryptpacket(DecryptPkgInfo info);
    [DllImport("tersafe")]
    private static extern AntiEncryptResult tss_sdk_encryptpacket(EncryptPkgInfo info);
    [DllImport("tersafe")]
    private static extern void tss_sdk_init(InitInfo info);
    [DllImport("tersafe")]
    private static extern void tss_sdk_rcv_anti_data(IntPtr info);
    [DllImport("tersafe")]
    private static extern void tss_sdk_setgamestatus(GameStatusInfo info);
    [DllImport("tersafe")]
    private static extern void tss_sdk_setuserinfo(UserInfoStrStr info);
    [DllImport("tersafe")]
    private static extern void tss_sdk_setuserinfo_ex(UserInfoEx info);
    public static AntiDecryptResult TssSdkDecrypt(byte[] src, uint src_len, ref byte[] tar, ref uint tar_len)
    {
        AntiDecryptResult result = AntiDecryptResult.ANTI_DECRYPT_FAIL;
        GCHandle handle = GCHandle.Alloc(src, GCHandleType.Pinned);
        GCHandle handle2 = GCHandle.Alloc(tar, GCHandleType.Pinned);
        if (handle.IsAllocated && handle2.IsAllocated)
        {
            DecryptPkgInfo info = new DecryptPkgInfo {
                encrypt_data_ = handle.AddrOfPinnedObject(),
                encrypt_data_len = src_len,
                game_pkg_ = handle2.AddrOfPinnedObject(),
                game_pkg_len_ = tar_len
            };
            result = tss_sdk_decryptpacket(info);
            tar_len = info.game_pkg_len_;
        }
        if (handle.IsAllocated)
        {
            handle.Free();
        }
        if (handle2.IsAllocated)
        {
            handle2.Free();
        }
        return result;
    }

    public static AntiEncryptResult TssSdkEncrypt(int cmd_id, byte[] src, uint src_len, ref byte[] tar, ref uint tar_len)
    {
        AntiEncryptResult result = AntiEncryptResult.ANTI_NOT_NEED_ENCRYPT;
        GCHandle handle = GCHandle.Alloc(src, GCHandleType.Pinned);
        GCHandle handle2 = GCHandle.Alloc(tar, GCHandleType.Pinned);
        if (handle.IsAllocated && handle2.IsAllocated)
        {
            EncryptPkgInfo info = new EncryptPkgInfo {
                cmd_id_ = cmd_id,
                game_pkg_ = handle.AddrOfPinnedObject(),
                game_pkg_len_ = src_len,
                encrpty_data_ = handle2.AddrOfPinnedObject(),
                encrypt_data_len_ = tar_len
            };
            result = tss_sdk_encryptpacket(info);
            tar_len = info.encrypt_data_len_;
        }
        if (handle.IsAllocated)
        {
            handle.Free();
        }
        if (handle2.IsAllocated)
        {
            handle2.Free();
        }
        return result;
    }

    public static void TssSdkInit(uint gameId)
    {
        InitInfo info;
        info = new InitInfo {
            size_ = Marshal.SizeOf(info),
            game_id_ = gameId,
            send_data_to_svr = IntPtr.Zero
        };
        tss_sdk_init(info);
        tss_enable_get_report_data();
        tss_log_str(TssSdkVersion.GetSdkVersion());
        tss_log_str(TssSdtVersion.GetSdtVersion());
    }

    public static void TssSdkRcvAntiData(byte[] data, ushort length)
    {
        IntPtr ptr = Marshal.AllocHGlobal((int) (2 + IntPtr.Size));
        if (ptr != IntPtr.Zero)
        {
            Marshal.WriteInt16(ptr, 0, (short) length);
            IntPtr destination = Marshal.AllocHGlobal(data.Length);
            if (destination != IntPtr.Zero)
            {
                Marshal.Copy(data, 0, destination, data.Length);
                Marshal.WriteIntPtr(ptr, 2, destination);
                tss_sdk_rcv_anti_data(ptr);
                Marshal.FreeHGlobal(destination);
            }
            Marshal.FreeHGlobal(ptr);
        }
    }

    public static void TssSdkSetGameStatus(EGAMESTATUS gameStatus)
    {
        GameStatusInfo info;
        info = new GameStatusInfo {
            size_ = Marshal.SizeOf(info),
            game_status_ = (uint) gameStatus
        };
        tss_sdk_setgamestatus(info);
    }

    public static void TssSdkSetUserInfo(EENTRYID entryId, string uin, string appId)
    {
        TssSdkSetUserInfoEx(entryId, uin, appId, 0, "0");
    }

    public static void TssSdkSetUserInfoEx(EENTRYID entryId, string uin, string appId, uint worldId, string roleId)
    {
        UserInfoEx ex;
        ex = new UserInfoEx {
            size = Marshal.SizeOf(ex),
            entrance_id = (uint) entryId,
            uin_type = 2
        };
        byte[] buffer = new byte[0x40];
        byte[] buffer2 = String2Bytes(uin);
        int index = 0;
        while ((index < buffer2.Length) && (index < 0x40))
        {
            buffer[index] = buffer2[index];
            index++;
        }
        while (index < 0x40)
        {
            buffer[index] = 0;
            index++;
        }
        ex.uin_00 = buffer[0];
        ex.uin_01 = buffer[1];
        ex.uin_02 = buffer[2];
        ex.uin_03 = buffer[3];
        ex.uin_04 = buffer[4];
        ex.uin_05 = buffer[5];
        ex.uin_06 = buffer[6];
        ex.uin_07 = buffer[7];
        ex.uin_08 = buffer[8];
        ex.uin_09 = buffer[9];
        ex.uin_10 = buffer[10];
        ex.uin_11 = buffer[11];
        ex.uin_12 = buffer[12];
        ex.uin_13 = buffer[13];
        ex.uin_14 = buffer[14];
        ex.uin_15 = buffer[15];
        ex.uin_16 = buffer[0x10];
        ex.uin_17 = buffer[0x11];
        ex.uin_18 = buffer[0x12];
        ex.uin_19 = buffer[0x13];
        ex.uin_20 = buffer[20];
        ex.uin_21 = buffer[0x15];
        ex.uin_22 = buffer[0x16];
        ex.uin_23 = buffer[0x17];
        ex.uin_24 = buffer[0x18];
        ex.uin_25 = buffer[0x19];
        ex.uin_26 = buffer[0x1a];
        ex.uin_27 = buffer[0x1b];
        ex.uin_28 = buffer[0x1c];
        ex.uin_29 = buffer[0x1d];
        ex.uin_30 = buffer[30];
        ex.uin_31 = buffer[0x1f];
        ex.uin_32 = buffer[0x20];
        ex.uin_33 = buffer[0x21];
        ex.uin_34 = buffer[0x22];
        ex.uin_35 = buffer[0x23];
        ex.uin_36 = buffer[0x24];
        ex.uin_37 = buffer[0x25];
        ex.uin_38 = buffer[0x26];
        ex.uin_39 = buffer[0x27];
        ex.uin_40 = buffer[40];
        ex.uin_41 = buffer[0x29];
        ex.uin_42 = buffer[0x2a];
        ex.uin_43 = buffer[0x2b];
        ex.uin_44 = buffer[0x2c];
        ex.uin_45 = buffer[0x2d];
        ex.uin_46 = buffer[0x2e];
        ex.uin_47 = buffer[0x2f];
        ex.uin_48 = buffer[0x30];
        ex.uin_49 = buffer[0x31];
        ex.uin_50 = buffer[50];
        ex.uin_51 = buffer[0x33];
        ex.uin_52 = buffer[0x34];
        ex.uin_53 = buffer[0x35];
        ex.uin_54 = buffer[0x36];
        ex.uin_55 = buffer[0x37];
        ex.uin_56 = buffer[0x38];
        ex.uin_57 = buffer[0x39];
        ex.uin_58 = buffer[0x3a];
        ex.uin_59 = buffer[0x3b];
        ex.uin_60 = buffer[60];
        ex.uin_61 = buffer[0x3d];
        ex.uin_62 = buffer[0x3e];
        ex.uin_63 = buffer[0x3f];
        ex.appid_type = 2;
        byte[] buffer3 = String2Bytes(appId);
        index = 0;
        while ((index < buffer3.Length) && (index < 0x40))
        {
            buffer[index] = buffer3[index];
            index++;
        }
        while (index < 0x40)
        {
            buffer[index] = 0;
            index++;
        }
        ex.appid_00 = buffer[0];
        ex.appid_01 = buffer[1];
        ex.appid_02 = buffer[2];
        ex.appid_03 = buffer[3];
        ex.appid_04 = buffer[4];
        ex.appid_05 = buffer[5];
        ex.appid_06 = buffer[6];
        ex.appid_07 = buffer[7];
        ex.appid_08 = buffer[8];
        ex.appid_09 = buffer[9];
        ex.appid_10 = buffer[10];
        ex.appid_11 = buffer[11];
        ex.appid_12 = buffer[12];
        ex.appid_13 = buffer[13];
        ex.appid_14 = buffer[14];
        ex.appid_15 = buffer[15];
        ex.appid_16 = buffer[0x10];
        ex.appid_17 = buffer[0x11];
        ex.appid_18 = buffer[0x12];
        ex.appid_19 = buffer[0x13];
        ex.appid_20 = buffer[20];
        ex.appid_21 = buffer[0x15];
        ex.appid_22 = buffer[0x16];
        ex.appid_23 = buffer[0x17];
        ex.appid_24 = buffer[0x18];
        ex.appid_25 = buffer[0x19];
        ex.appid_26 = buffer[0x1a];
        ex.appid_27 = buffer[0x1b];
        ex.appid_28 = buffer[0x1c];
        ex.appid_29 = buffer[0x1d];
        ex.appid_30 = buffer[30];
        ex.appid_31 = buffer[0x1f];
        ex.appid_32 = buffer[0x20];
        ex.appid_33 = buffer[0x21];
        ex.appid_34 = buffer[0x22];
        ex.appid_35 = buffer[0x23];
        ex.appid_36 = buffer[0x24];
        ex.appid_37 = buffer[0x25];
        ex.appid_38 = buffer[0x26];
        ex.appid_39 = buffer[0x27];
        ex.appid_40 = buffer[40];
        ex.appid_41 = buffer[0x29];
        ex.appid_42 = buffer[0x2a];
        ex.appid_43 = buffer[0x2b];
        ex.appid_44 = buffer[0x2c];
        ex.appid_45 = buffer[0x2d];
        ex.appid_46 = buffer[0x2e];
        ex.appid_47 = buffer[0x2f];
        ex.appid_48 = buffer[0x30];
        ex.appid_49 = buffer[0x31];
        ex.appid_50 = buffer[50];
        ex.appid_51 = buffer[0x33];
        ex.appid_52 = buffer[0x34];
        ex.appid_53 = buffer[0x35];
        ex.appid_54 = buffer[0x36];
        ex.appid_55 = buffer[0x37];
        ex.appid_56 = buffer[0x38];
        ex.appid_57 = buffer[0x39];
        ex.appid_58 = buffer[0x3a];
        ex.appid_59 = buffer[0x3b];
        ex.appid_60 = buffer[60];
        ex.appid_61 = buffer[0x3d];
        ex.appid_62 = buffer[0x3e];
        ex.appid_63 = buffer[0x3f];
        ex.world_id = worldId;
        byte[] buffer4 = String2Bytes(roleId);
        index = 0;
        while (index < buffer4.Length)
        {
            buffer[index] = buffer4[index];
            index++;
        }
        while (index < 0x40)
        {
            buffer[index] = 0;
            index++;
        }
        ex.role_id_00 = buffer[0];
        ex.role_id_01 = buffer[1];
        ex.role_id_02 = buffer[2];
        ex.role_id_03 = buffer[3];
        ex.role_id_04 = buffer[4];
        ex.role_id_05 = buffer[5];
        ex.role_id_06 = buffer[6];
        ex.role_id_07 = buffer[7];
        ex.role_id_08 = buffer[8];
        ex.role_id_09 = buffer[9];
        ex.role_id_10 = buffer[10];
        ex.role_id_11 = buffer[11];
        ex.role_id_12 = buffer[12];
        ex.role_id_13 = buffer[13];
        ex.role_id_14 = buffer[14];
        ex.role_id_15 = buffer[15];
        ex.role_id_16 = buffer[0x10];
        ex.role_id_17 = buffer[0x11];
        ex.role_id_18 = buffer[0x12];
        ex.role_id_19 = buffer[0x13];
        ex.role_id_20 = buffer[20];
        ex.role_id_21 = buffer[0x15];
        ex.role_id_22 = buffer[0x16];
        ex.role_id_23 = buffer[0x17];
        ex.role_id_24 = buffer[0x18];
        ex.role_id_25 = buffer[0x19];
        ex.role_id_26 = buffer[0x1a];
        ex.role_id_27 = buffer[0x1b];
        ex.role_id_28 = buffer[0x1c];
        ex.role_id_29 = buffer[0x1d];
        ex.role_id_30 = buffer[30];
        ex.role_id_31 = buffer[0x1f];
        ex.role_id_32 = buffer[0x20];
        ex.role_id_33 = buffer[0x21];
        ex.role_id_34 = buffer[0x22];
        ex.role_id_35 = buffer[0x23];
        ex.role_id_36 = buffer[0x24];
        ex.role_id_37 = buffer[0x25];
        ex.role_id_38 = buffer[0x26];
        ex.role_id_39 = buffer[0x27];
        ex.role_id_40 = buffer[40];
        ex.role_id_41 = buffer[0x29];
        ex.role_id_42 = buffer[0x2a];
        ex.role_id_43 = buffer[0x2b];
        ex.role_id_44 = buffer[0x2c];
        ex.role_id_45 = buffer[0x2d];
        ex.role_id_46 = buffer[0x2e];
        ex.role_id_47 = buffer[0x2f];
        ex.role_id_48 = buffer[0x30];
        ex.role_id_49 = buffer[0x31];
        ex.role_id_50 = buffer[50];
        ex.role_id_51 = buffer[0x33];
        ex.role_id_52 = buffer[0x34];
        ex.role_id_53 = buffer[0x35];
        ex.role_id_54 = buffer[0x36];
        ex.role_id_55 = buffer[0x37];
        ex.role_id_56 = buffer[0x38];
        ex.role_id_57 = buffer[0x39];
        ex.role_id_58 = buffer[0x3a];
        ex.role_id_59 = buffer[0x3b];
        ex.role_id_60 = buffer[60];
        ex.role_id_61 = buffer[0x3d];
        ex.role_id_62 = buffer[0x3e];
        ex.role_id_63 = buffer[0x3f];
        tss_sdk_setuserinfo_ex(ex);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AntiDataInfo
    {
        public ushort anti_data_len;
        public IntPtr anti_data;
    }

    public enum AntiDecryptResult
    {
        ANTI_DECRYPT_OK,
        ANTI_DECRYPT_FAIL
    }

    public enum AntiEncryptResult
    {
        ANTI_ENCRYPT_OK,
        ANTI_NOT_NEED_ENCRYPT
    }

    [StructLayout(LayoutKind.Sequential)]
    public class APPID_STR
    {
        public uint type;
        public TssSdk.AppIdInfoStr app_id;
    }

    [StructLayout(LayoutKind.Explicit, Size=0x40)]
    public struct AppIdInfoStr
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40), FieldOffset(0)]
        public string app_id;
    }

    [StructLayout(LayoutKind.Explicit, Size=0x10)]
    public class DecryptPkgInfo
    {
        [FieldOffset(0)]
        public IntPtr encrypt_data_;
        [FieldOffset(4)]
        public uint encrypt_data_len;
        [FieldOffset(8)]
        public IntPtr game_pkg_;
        [FieldOffset(12)]
        public uint game_pkg_len_;
    }

    public enum EAPPIDTYPE
    {
        APP_ID_TYPE_INT = 1,
        APP_ID_TYPE_STR = 2
    }

    public enum EENTRYID
    {
        ENTRY_ID_MM = 2,
        ENTRY_ID_OTHERS = 3,
        ENTRY_ID_QZONE = 1
    }

    public enum EGAMESTATUS
    {
        GAME_STATUS_BACKEND = 2,
        GAME_STATUS_FRONTEND = 1
    }

    [StructLayout(LayoutKind.Explicit, Size=20)]
    public class EncryptPkgInfo
    {
        [FieldOffset(0)]
        public int cmd_id_;
        [FieldOffset(12)]
        public IntPtr encrpty_data_;
        [FieldOffset(0x10)]
        public uint encrypt_data_len_;
        [FieldOffset(4)]
        public IntPtr game_pkg_;
        [FieldOffset(8)]
        public uint game_pkg_len_;
    }

    public enum EUINTYPE
    {
        UIN_TYPE_INT = 1,
        UIN_TYPE_STR = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public class GameStatusInfo
    {
        public uint size_;
        public uint game_status_;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class InitInfo
    {
        public uint size_;
        public uint game_id_;
        public IntPtr send_data_to_svr;
    }

    [StructLayout(LayoutKind.Explicit, Size=0x40)]
    public struct RoleIdInfoStr
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40), FieldOffset(0)]
        public string role_id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class UIN_STR
    {
        public uint type;
        public TssSdk.UinInfoStr uin;
    }

    [StructLayout(LayoutKind.Explicit, Size=0x40)]
    public struct UinInfoStr
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40), FieldOffset(0)]
        public string uin;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class UserInfoEx
    {
        public int size;
        public uint entrance_id;
        public uint uin_type;
        public byte uin_00;
        public byte uin_01;
        public byte uin_02;
        public byte uin_03;
        public byte uin_04;
        public byte uin_05;
        public byte uin_06;
        public byte uin_07;
        public byte uin_08;
        public byte uin_09;
        public byte uin_10;
        public byte uin_11;
        public byte uin_12;
        public byte uin_13;
        public byte uin_14;
        public byte uin_15;
        public byte uin_16;
        public byte uin_17;
        public byte uin_18;
        public byte uin_19;
        public byte uin_20;
        public byte uin_21;
        public byte uin_22;
        public byte uin_23;
        public byte uin_24;
        public byte uin_25;
        public byte uin_26;
        public byte uin_27;
        public byte uin_28;
        public byte uin_29;
        public byte uin_30;
        public byte uin_31;
        public byte uin_32;
        public byte uin_33;
        public byte uin_34;
        public byte uin_35;
        public byte uin_36;
        public byte uin_37;
        public byte uin_38;
        public byte uin_39;
        public byte uin_40;
        public byte uin_41;
        public byte uin_42;
        public byte uin_43;
        public byte uin_44;
        public byte uin_45;
        public byte uin_46;
        public byte uin_47;
        public byte uin_48;
        public byte uin_49;
        public byte uin_50;
        public byte uin_51;
        public byte uin_52;
        public byte uin_53;
        public byte uin_54;
        public byte uin_55;
        public byte uin_56;
        public byte uin_57;
        public byte uin_58;
        public byte uin_59;
        public byte uin_60;
        public byte uin_61;
        public byte uin_62;
        public byte uin_63;
        public uint appid_type;
        public byte appid_00;
        public byte appid_01;
        public byte appid_02;
        public byte appid_03;
        public byte appid_04;
        public byte appid_05;
        public byte appid_06;
        public byte appid_07;
        public byte appid_08;
        public byte appid_09;
        public byte appid_10;
        public byte appid_11;
        public byte appid_12;
        public byte appid_13;
        public byte appid_14;
        public byte appid_15;
        public byte appid_16;
        public byte appid_17;
        public byte appid_18;
        public byte appid_19;
        public byte appid_20;
        public byte appid_21;
        public byte appid_22;
        public byte appid_23;
        public byte appid_24;
        public byte appid_25;
        public byte appid_26;
        public byte appid_27;
        public byte appid_28;
        public byte appid_29;
        public byte appid_30;
        public byte appid_31;
        public byte appid_32;
        public byte appid_33;
        public byte appid_34;
        public byte appid_35;
        public byte appid_36;
        public byte appid_37;
        public byte appid_38;
        public byte appid_39;
        public byte appid_40;
        public byte appid_41;
        public byte appid_42;
        public byte appid_43;
        public byte appid_44;
        public byte appid_45;
        public byte appid_46;
        public byte appid_47;
        public byte appid_48;
        public byte appid_49;
        public byte appid_50;
        public byte appid_51;
        public byte appid_52;
        public byte appid_53;
        public byte appid_54;
        public byte appid_55;
        public byte appid_56;
        public byte appid_57;
        public byte appid_58;
        public byte appid_59;
        public byte appid_60;
        public byte appid_61;
        public byte appid_62;
        public byte appid_63;
        public uint world_id;
        public byte role_id_00;
        public byte role_id_01;
        public byte role_id_02;
        public byte role_id_03;
        public byte role_id_04;
        public byte role_id_05;
        public byte role_id_06;
        public byte role_id_07;
        public byte role_id_08;
        public byte role_id_09;
        public byte role_id_10;
        public byte role_id_11;
        public byte role_id_12;
        public byte role_id_13;
        public byte role_id_14;
        public byte role_id_15;
        public byte role_id_16;
        public byte role_id_17;
        public byte role_id_18;
        public byte role_id_19;
        public byte role_id_20;
        public byte role_id_21;
        public byte role_id_22;
        public byte role_id_23;
        public byte role_id_24;
        public byte role_id_25;
        public byte role_id_26;
        public byte role_id_27;
        public byte role_id_28;
        public byte role_id_29;
        public byte role_id_30;
        public byte role_id_31;
        public byte role_id_32;
        public byte role_id_33;
        public byte role_id_34;
        public byte role_id_35;
        public byte role_id_36;
        public byte role_id_37;
        public byte role_id_38;
        public byte role_id_39;
        public byte role_id_40;
        public byte role_id_41;
        public byte role_id_42;
        public byte role_id_43;
        public byte role_id_44;
        public byte role_id_45;
        public byte role_id_46;
        public byte role_id_47;
        public byte role_id_48;
        public byte role_id_49;
        public byte role_id_50;
        public byte role_id_51;
        public byte role_id_52;
        public byte role_id_53;
        public byte role_id_54;
        public byte role_id_55;
        public byte role_id_56;
        public byte role_id_57;
        public byte role_id_58;
        public byte role_id_59;
        public byte role_id_60;
        public byte role_id_61;
        public byte role_id_62;
        public byte role_id_63;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class UserInfoStrStr
    {
        public uint size;
        public uint entrance_id;
        public TssSdk.UIN_STR uin;
        public TssSdk.APPID_STR app_id;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class UserInfoStrStrEx
    {
        public uint size;
        public uint entrance_id;
        public TssSdk.UIN_STR uin;
        public TssSdk.APPID_STR app_id;
        public uint world_id;
        public TssSdk.RoleIdInfoStr role_id;
    }
}

