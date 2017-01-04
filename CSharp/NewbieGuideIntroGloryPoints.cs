using Assets.Scripts.UI;
using System;

internal class NewbieGuideIntroGloryPoints : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
        string text = Singleton<CTextManager>.GetInstance().GetText("Tutorial_Old_Glory_Title");
        string btnName = Singleton<CTextManager>.GetInstance().GetText("Tutorial_Old_Glory_Button");
        string[] imgPath = new string[] { string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "Newbie_Honor1"), string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "Newbie_Honor2") };
        Singleton<CBattleGuideManager>.GetInstance().OpenBannerIntroDialog(imgPath, 2, null, text, btnName, true);
        this.CompleteHandler();
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }
}

