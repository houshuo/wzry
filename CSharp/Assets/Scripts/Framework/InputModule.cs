namespace Assets.Scripts.Framework
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class InputModule : Singleton<InputModule>, IGameModule
    {
        public event TouchActions TouchAction;

        private void CheckTouchState()
        {
            if (((EventSystem.current == null) || !EventSystem.current.IsPointerOverGameObject()) && (this.TouchAction != null))
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    this.TouchAction(this, new TouchEventArgs(Input.GetTouch(i), i));
                }
            }
        }

        public override void Init()
        {
        }

        public void UpdateFrame()
        {
            this.CheckTouchState();
        }
    }
}

