namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class HeroChooseLogic : Singleton<HeroChooseLogic>
    {
        private List<GameObject> cacheObjList = new List<GameObject>();
        private Text heroDescTxt;
        public static string s_heroInitChooseFormPath = "UGUI/Form/System/HeroInitChoose/Form_Hero_InitChoose.prefab";
        private GameObject selectHeroBtn;
        private uint selectHeroId;

        public void CloseInitChooseHeroForm()
        {
            this.OnDestroyActorList();
            Singleton<CUIManager>.GetInstance().CloseForm(s_heroInitChooseFormPath);
        }

        private void CreateHeroPreview(uint heroId, int i)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_heroInitChooseFormPath);
            if (form != null)
            {
                string name = string.Format("RawImage{0}", i);
                GameObject gameObject = form.transform.Find(name).gameObject;
                CUIRawImageScript component = gameObject.GetComponent<CUIRawImageScript>();
                ObjData data = CUICommonSystem.GetHero3DObj(heroId, true);
                if (data.Object != null)
                {
                    component.AddGameObject(name, data.Object, Vector3.zero, Quaternion.identity, data.Object.transform.localScale);
                    this.cacheObjList.Add(data.Object);
                    CUIEventScript script3 = gameObject.GetComponent<CUIEventScript>();
                    if (script3 == null)
                    {
                        script3 = gameObject.AddComponent<CUIEventScript>();
                        script3.Initialize(form);
                    }
                    stUIEventParams eventParams = new stUIEventParams {
                        heroId = heroId
                    };
                    script3.SetUIEvent(enUIEventType.Click, enUIEventID.Hero_Init_Select, eventParams);
                }
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Hero_Init_Select, new CUIEventManager.OnUIEventHandler(this.OnClickHeroModel));
        }

        private void InitHeroPanel()
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 7);
            this.CreateHeroPreview(dataByKey.dwConfValue, 0);
            ResGlobalInfo info2 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 8);
            this.CreateHeroPreview(info2.dwConfValue, 1);
            ResGlobalInfo info3 = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 9);
            this.CreateHeroPreview(info3.dwConfValue, 2);
        }

        public void OnClickHeroModel(CUIEvent uiEvent)
        {
            this.selectHeroId = uiEvent.m_eventParams.heroId;
            this.heroDescTxt.text = string.Empty;
            this.selectHeroBtn.CustomSetActive(true);
        }

        private void OnConfirmChooseHero(GameObject go)
        {
            Singleton<LobbyLogic>.GetInstance().ReqSelectHero(this.selectHeroId);
        }

        private void OnDestroyActorList()
        {
            for (int i = 0; i < this.cacheObjList.Count; i++)
            {
                UnityEngine.Object.DestroyObject(this.cacheObjList[i]);
            }
            this.cacheObjList.Clear();
        }

        public void OpenInitChooseHeroForm()
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_heroInitChooseFormPath, false, true);
            if (script != null)
            {
                this.heroDescTxt = script.transform.Find("heroDescTxt").GetComponent<Text>();
                this.heroDescTxt.text = string.Empty;
                this.selectHeroBtn = script.transform.Find("selectHeroBtn").gameObject;
                GUIEventListener.Get(this.selectHeroBtn).onClick += new GUIEventListener.VoidDelegate(this.OnConfirmChooseHero);
                this.selectHeroBtn.CustomSetActive(false);
                this.InitHeroPanel();
            }
        }
    }
}

