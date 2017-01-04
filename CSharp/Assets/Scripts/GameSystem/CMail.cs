namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public class CMail
    {
        public ListView<CUseable> accessUseable;
        public int accessUseableGeted;
        public bool autoDel;
        public string from;
        public bool isAccess;
        public bool isReceive;
        public string mailContent;
        public string mailHyperlink;
        public int mailIndex;
        public COM_MAIL_STATE mailState;
        public COM_MAIL_TYPE mailType;
        public uint sendTime;
        public string subject;
        public byte subType;

        public CMail()
        {
            this.accessUseable = new ListView<CUseable>();
        }

        public CMail(COM_MAIL_TYPE mailType, ref CSDT_GETMAIL_RES pkg)
        {
            this.accessUseable = new ListView<CUseable>();
            this.mailType = mailType;
            this.subType = pkg.bMailType;
            this.mailIndex = pkg.iMailIndex;
            this.mailState = (COM_MAIL_STATE) pkg.bMailState;
            this.autoDel = pkg.bAutoDel > 0;
            this.from = Utility.UTF8Convert(pkg.szFrom);
            this.sendTime = pkg.dwSendTime;
            this.subject = Utility.UTF8Convert(pkg.szSubject, pkg.bSubjectLen);
        }

        public void ParseContentAndHyperlink(sbyte[] srcContent, int srcContentLength, ref string content, ref string hyperlink)
        {
            int num = -1;
            int num2 = -1;
            for (int i = 0; i < srcContentLength; i++)
            {
                if (srcContent[i].Equals((sbyte) 0x5b))
                {
                    num = i;
                }
                else if (srcContent[i].Equals((sbyte) 0x5d))
                {
                    num2 = i;
                }
            }
            if ((0 < num) && (num < num2))
            {
                sbyte[] p = new sbyte[num];
                for (int j = 0; j < p.Length; j++)
                {
                    p[j] = srcContent[j];
                }
                content = Utility.UTF8Convert(p, p.Length);
                sbyte[] numArray2 = new sbyte[((num2 - num) + 1) - 2];
                for (int k = 0; k < numArray2.Length; k++)
                {
                    numArray2[k] = srcContent[(num + 1) + k];
                }
                hyperlink = Utility.UTF8Convert(numArray2, numArray2.Length);
            }
            if (!CHyperLink.IsStandCommond(hyperlink))
            {
                content = Utility.UTF8Convert(srcContent, srcContentLength);
                hyperlink = null;
            }
        }

        public void Read(CSDT_MAILOPTRES_READMAIL pkg)
        {
            this.isReceive = true;
            this.accessUseable.Clear();
            this.mailState = (COM_MAIL_STATE) pkg.bMailState;
            this.ParseContentAndHyperlink(pkg.szContent, pkg.wContentLen, ref this.mailContent, ref this.mailHyperlink);
            this.accessUseableGeted = 0;
            this.accessUseable = CMailSys.StAccessToUseable(pkg.astAccess, null, pkg.bAccessCnt);
            for (int i = 0; i < this.accessUseable.Count; i++)
            {
                if (pkg.astAccess[i].bGeted == 1)
                {
                    this.accessUseableGeted |= ((int) 1) << i;
                }
            }
        }

        public int CanBeDeleted
        {
            get
            {
                int num = 0;
                if (this.mailState == COM_MAIL_STATE.COM_MAIL_HAVEREAD)
                {
                    if (this.accessUseable.Count > 0)
                    {
                        for (int i = 0; i < this.accessUseable.Count; i++)
                        {
                            if ((this.accessUseableGeted & (((int) 1) << i)) == 1)
                            {
                                return 0x2717;
                            }
                        }
                    }
                    return num;
                }
                return 0x2716;
            }
        }
    }
}

