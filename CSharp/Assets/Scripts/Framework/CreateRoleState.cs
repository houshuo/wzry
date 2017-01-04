namespace Assets.Scripts.Framework
{
    using System;
    using UnityEngine;

    [GameState]
    public class CreateRoleState : BaseState
    {
        private void OnCreateRoleSceneCompleted()
        {
            Debug.Log("CreateRoleState Load Complete");
        }

        private void OnMoviePlayComplete(int timerSequence)
        {
            Debug.Log("OnMoviePlayComplete  " + Time.realtimeSinceStartup);
            Singleton<CRoleRegisterSys>.GetInstance().SetRoleCreateFormVisible(true);
        }

        public override void OnStateEnter()
        {
            Debug.Log("CreateRoleState enter");
            Singleton<ResourceLoader>.GetInstance().LoadScene("CreateRoleScene", new ResourceLoader.LoadCompletedDelegate(this.OnCreateRoleSceneCompleted));
            Singleton<CRoleRegisterSys>.instance.OpenRoleCreateForm();
        }

        public override void OnStateLeave()
        {
            base.OnStateLeave();
            Singleton<CRoleRegisterSys>.instance.CloseRoleCreateForm();
        }

        private void PlayWorldStartMovie()
        {
            bool flag = false;
            bool flag2 = false;
            try
            {
                Debug.Log("worldstart exist and play " + Time.realtimeSinceStartup);
                flag2 = true;
                Handheld.PlayFullScreenMovie("Video/worldstart.mp4", Color.black, 2, 1);
            }
            catch (Exception)
            {
                Debug.Log("PlayFullScreenMovie faild worldstart.mp4");
            }
            if (flag)
            {
                Debug.Log("bAddFormVisibleTimer  " + Time.realtimeSinceStartup);
                Singleton<CTimerManager>.GetInstance().AddTimer(100, 1, new CTimer.OnTimeUpHandler(this.OnMoviePlayComplete));
            }
            else
            {
                Singleton<CRoleRegisterSys>.GetInstance().SetRoleCreateFormVisible(true);
            }
        }
    }
}

