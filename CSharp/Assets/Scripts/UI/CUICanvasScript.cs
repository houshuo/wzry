namespace Assets.Scripts.UI
{
    using System;

    public class CUICanvasScript : CUIComponent
    {
        public override void Appear()
        {
            base.Appear();
            CUIUtility.SetGameObjectLayer(base.gameObject, 5);
        }

        public override void Hide()
        {
            base.Hide();
            CUIUtility.SetGameObjectLayer(base.gameObject, 0x1f);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
            }
        }
    }
}

