namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MinimapSys
    {
        private CUIFormScript _ownerForm;
        private EMapType curMapType;
        public static string enemy_base = "UGUI/Sprite/Battle/Img_Map_Base_Red";
        public static string enemy_base_outline = "UGUI/Sprite/Battle/Img_Map_Base_Red_outline";
        public static string enemy_Eye = "UGUI/Sprite/Battle/Img_Map_RedEye";
        public static string enemy_tower = "UGUI/Sprite/Battle/Img_Map_Tower_Red";
        public static string enemy_tower_outline = "UGUI/Sprite/Battle/Img_Map_Tower_Red_outline";
        private DragonIcon m_dragonIcon;
        public MiniMapCameraFrame m_miniMapCameraFrame;
        public static string self_base = "UGUI/Sprite/Battle/Img_Map_Base_Green";
        public static string self_base_outline = "UGUI/Sprite/Battle/Img_Map_Base_Green_outline";
        public static string self_Eye = "UGUI/Sprite/Battle/Img_Map_BlueEye";
        public static string self_tower = "UGUI/Sprite/Battle/Img_Map_Tower_Green";
        public static string self_tower_outline = "UGUI/Sprite/Battle/Img_Map_Tower_Green_outline";

        public void Clear()
        {
            this.unRegEvent();
            if (this.m_dragonIcon != null)
            {
                this.m_dragonIcon.Clear();
                this.m_dragonIcon = null;
            }
            if (this.m_miniMapCameraFrame != null)
            {
                this.m_miniMapCameraFrame.Clear();
                this.m_miniMapCameraFrame = null;
            }
            this.mmRoot = null;
            this.bmRoot = null;
            this.mmpcAlies = null;
            this.mmpcHero = null;
            this.mmpcEnemy = null;
            this.mmpcOrgan = null;
            this.mmpcSignal = null;
            this.mmpcDragon = null;
            this.mmpcEffect = null;
            this.mmpcEye = null;
            this.bmpcAlies = null;
            this.bmpcHero = null;
            this.bmpcEnemy = null;
            this.bmpcOrgan = null;
            this.bmpcSignal = null;
            this.bmpcDragon = null;
            this.mmpcEffect = null;
            this.bmpcEye = null;
            this._ownerForm = null;
        }

        public EMapType CurMapType()
        {
            return this.curMapType;
        }

        public void Init(CUIFormScript formObj, SLevelContext levelContext)
        {
            if (formObj != null)
            {
                this._ownerForm = formObj;
                this.mmRoot = Utility.FindChild(formObj.gameObject, "MapPanel/Mini");
                this.bmRoot = Utility.FindChild(formObj.gameObject, "MapPanel/Big");
                if ((this.mmRoot != null) && (this.bmRoot != null))
                {
                    if (!levelContext.IsMobaMode())
                    {
                        this.mmRoot.SetActive(false);
                        this.bmRoot.SetActive(false);
                    }
                    else if (levelContext != null)
                    {
                        this.regEvent();
                        this.mmpcAlies = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Alies");
                        this.mmpcHero = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Hero");
                        this.mmpcEnemy = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Enemy");
                        this.mmpcOrgan = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Organ");
                        this.mmpcSignal = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Signal");
                        this.mmpcDragon = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Dragon");
                        this.mmpcEffect = Utility.FindChild(this.mmRoot, "BigMapEffectRoot");
                        this.mmpcEye = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Eye");
                        this.bmpcAlies = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Alies");
                        this.bmpcHero = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Hero");
                        this.bmpcEnemy = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Enemy");
                        this.bmpcOrgan = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Organ");
                        this.bmpcSignal = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Signal");
                        this.bmpcDragon = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Dragon");
                        this.mmpcEffect = Utility.FindChild(this.bmRoot, "BigMapEffectRoot");
                        this.bmpcEye = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Eye");
                        this.mmRoot.CustomSetActive(true);
                        this.bmRoot.CustomSetActive(true);
                        RectTransform transform = null;
                        if (levelContext.IsMobaMode())
                        {
                            float num;
                            this.Switch(EMapType.Mini);
                            transform = this.initMap(this.bmRoot, levelContext, false, out num);
                            transform.anchoredPosition = new Vector2(transform.rect.width * 0.5f, -transform.rect.height * 0.5f);
                            transform = this.initMap(this.mmRoot, levelContext, true, out num);
                            if (levelContext.m_pvpPlayerNum == 6)
                            {
                                transform.anchoredPosition = new Vector2(transform.anchoredPosition.x + ((transform.rect.width * 0.5f) - (num * 0.5f)), transform.anchoredPosition.y);
                                GameObject obj2 = Utility.FindChild(this._ownerForm.gameObject, "MapPanel/DragonInfo");
                                GameObject obj3 = Utility.FindChild(this._ownerForm.gameObject, "MapPanel/Button_Signal_1");
                                if (obj2 != null)
                                {
                                    RectTransform transform2 = obj2.gameObject.transform as RectTransform;
                                    transform2.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform2.anchoredPosition.y);
                                }
                                if (obj3 != null)
                                {
                                    RectTransform transform3 = obj3.gameObject.transform as RectTransform;
                                    transform3.anchoredPosition = new Vector2((transform.rect.width - (transform3.rect.width * 0.5f)) + 43f, transform3.anchoredPosition.y);
                                }
                            }
                        }
                        else
                        {
                            this.Switch(EMapType.None);
                        }
                        this.curMapType = EMapType.Mini;
                        bool flag = false;
                        bool flag2 = false;
                        if (levelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE)
                        {
                            switch (levelContext.m_mapID)
                            {
                                case 2:
                                    flag = true;
                                    flag2 = false;
                                    break;

                                case 3:
                                case 6:
                                case 7:
                                    flag = true;
                                    flag2 = true;
                                    break;
                            }
                        }
                        else if ((levelContext.m_pvpPlayerNum == 6) || (levelContext.m_pvpPlayerNum == 10))
                        {
                            flag = true;
                            flag2 = levelContext.m_pvpPlayerNum == 10;
                        }
                        if (flag && (this.mmpcDragon != null))
                        {
                            this.m_dragonIcon = new DragonIcon();
                            this.m_dragonIcon.Init(this.mmpcDragon, this.bmpcDragon, flag2);
                        }
                        GameObject gameObject = this.mmRoot.transform.Find("CameraFrame").gameObject;
                        if (gameObject != null)
                        {
                            this.m_miniMapCameraFrame = new MiniMapCameraFrame(gameObject, transform.sizeDelta.x, transform.sizeDelta.y);
                            this.m_miniMapCameraFrame.SetFrameSize((CameraHeightType) GameSettings.CameraHeight);
                        }
                    }
                }
            }
        }

        private RectTransform initMap(GameObject mobjRoot, SLevelContext levelContext, bool bMinimap, out float preWidth)
        {
            preWidth = 0f;
            DebugHelper.Assert(mobjRoot != null, "initMap GetWidget is null");
            if (mobjRoot == null)
            {
                return null;
            }
            Image component = mobjRoot.GetComponent<Image>();
            DebugHelper.Assert(component != null, "initMap map.GetComponent<Image>() is null");
            if (component == null)
            {
                return null;
            }
            RectTransform transform = mobjRoot.transform as RectTransform;
            preWidth = transform.rect.width;
            Vector2 sizeDelta = transform.sizeDelta;
            string prefabPath = CUIUtility.s_Sprite_Dynamic_Map_Dir + (!bMinimap ? levelContext.m_bigMapPath : levelContext.m_miniMapPath);
            component.SetSprite(prefabPath, this._ownerForm, true, false, false);
            component.SetNativeSize();
            if (bMinimap)
            {
                this.initWorldUITransformFactor(sizeDelta, levelContext, bMinimap, out Singleton<CBattleSystem>.instance.world_UI_Factor_Small, out Singleton<CBattleSystem>.instance.UI_world_Factor_Small);
                return transform;
            }
            this.initWorldUITransformFactor(sizeDelta, levelContext, bMinimap, out Singleton<CBattleSystem>.instance.world_UI_Factor_Big, out Singleton<CBattleSystem>.instance.UI_world_Factor_Big);
            return transform;
        }

        private void initWorldUITransformFactor(Vector2 mapImgSize, SLevelContext levelContext, bool bMiniMap, out Vector2 world_UI_Factor, out Vector2 UI_world_Factor)
        {
            int num = !bMiniMap ? levelContext.m_bigMapWidth : levelContext.m_mapWidth;
            int num2 = !bMiniMap ? levelContext.m_bigMapHeight : levelContext.m_mapHeight;
            float x = mapImgSize.x / ((float) num);
            float y = mapImgSize.y / ((float) num2);
            world_UI_Factor = new Vector2(x, y);
            float num5 = ((float) num) / mapImgSize.x;
            float num6 = ((float) num2) / mapImgSize.y;
            UI_world_Factor = new Vector2(num5, num6);
            if (levelContext.m_isCameraFlip)
            {
                world_UI_Factor = new Vector2(-x, -y);
                UI_world_Factor = new Vector2(-num5, -num6);
            }
            GameObject obj2 = !bMiniMap ? this.bmRoot : this.mmRoot;
            if (obj2 != null)
            {
                Image2 component = obj2.GetComponent<Image2>();
                if (component != null)
                {
                    float num7 = !bMiniMap ? levelContext.m_bigMapFowScale : levelContext.m_mapFowScale;
                    float num8 = !levelContext.m_isCameraFlip ? 1f : 0f;
                    component.SetMaterialVector("_FowParams", new Vector4(num7, num8, 1f, 1f));
                }
            }
        }

        public void Move_CameraToClickPosition(CUIEvent uiEvent)
        {
            if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
                if (Singleton<CBattleSystem>.GetInstance().WatchForm != null)
                {
                    Singleton<CBattleSystem>.GetInstance().WatchForm.OnCamerFreed();
                }
            }
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (((srcFormScript != null) && (uiEvent.m_srcWidget != null)) && (uiEvent.m_pointerEventData != null))
            {
                Vector2 position = uiEvent.m_pointerEventData.position;
                Vector2 vector2 = CUIUtility.WorldToScreenPoint(srcFormScript.GetCamera(), uiEvent.m_srcWidget.transform.position);
                float num = position.x - vector2.x;
                float num2 = position.y - vector2.y;
                num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
                num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
                float x = num * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.x;
                float z = num2 * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.y;
                if (MonoSingleton<CameraSystem>.instance.MobaCamera != null)
                {
                    MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation(new Vector3(x, 1f, z));
                    if (this.m_miniMapCameraFrame != null)
                    {
                        if (!this.m_miniMapCameraFrame.IsCameraFrameShow)
                        {
                            this.m_miniMapCameraFrame.Show();
                            this.m_miniMapCameraFrame.ShowNormal();
                        }
                        this.m_miniMapCameraFrame.SetPos(num, num2);
                    }
                }
            }
        }

        public void OnActorDamage(ref HurtEventResultInfo hri)
        {
            if (((this.m_miniMapCameraFrame != null) && this.m_miniMapCameraFrame.IsCameraFrameShow) && (hri.hurtInfo.hurtType != HurtTypeDef.Therapic))
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (((hri.src != 0) && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain == hri.src)))
                {
                    this.m_miniMapCameraFrame.ShowRed();
                }
            }
        }

        private void OnBigMap_Click_3_long(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, 0);
        }

        private void OnBigMap_Click_5_Dalong(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, 0);
        }

        private void OnBigMap_Click_5_Xiaolong(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, 0);
        }

        private void OnBigMap_Click_Hero(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, 0);
        }

        private void OnBigMap_Click_Map(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, 1);
        }

        private void OnBigMap_Click_Organ(CUIEvent uievent)
        {
            this.send_signal(uievent, this.bmRoot, 0);
        }

        private void OnBigMap_Open_BigMap(CUIEvent uievent)
        {
            CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_OpenBigMap);
            this.Switch(EMapType.Big);
            this.RefreshMapPointers();
        }

        private void OnBuildingAttacked(ref DefaultGameEventParam evtParam)
        {
            if (evtParam.src != 0)
            {
                ActorRoot handle = evtParam.src.handle;
                if ((handle != null) && HudUT.IsTower(handle))
                {
                    HudComponent3D hudControl = handle.HudControl;
                    if (hudControl != null)
                    {
                        GameObject target = hudControl.GetCurrent_MapObj();
                        TowerHitMgr towerHitMgr = Singleton<CBattleSystem>.GetInstance().TowerHitMgr;
                        if ((target != null) && (towerHitMgr != null))
                        {
                            towerHitMgr.TryActive(handle.ObjID, target);
                        }
                        Image image = hudControl.GetBigTower_Img();
                        Image image2 = hudControl.GetSmallTower_Img();
                        if (((image != null) && (image2 != null)) && (handle.ValueComponent != null))
                        {
                            float single = handle.ValueComponent.GetHpRate().single;
                            image.fillAmount = single;
                            image2.fillAmount = single;
                        }
                    }
                }
            }
        }

        private void OnCloseBigMap(CUIEvent uiEvent)
        {
            if (this.curMapType == EMapType.Big)
            {
                this.Switch(EMapType.Mini);
            }
        }

        private void OnDragMiniMap(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (((uiEvent != null) && (srcFormScript != null)) && ((uiEvent.m_pointerEventData != null) && (uiEvent.m_srcWidget != null)))
            {
                if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
                {
                    MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
                    if (Singleton<CBattleSystem>.GetInstance().WatchForm != null)
                    {
                        Singleton<CBattleSystem>.GetInstance().WatchForm.OnCamerFreed();
                    }
                }
                Vector2 position = uiEvent.m_pointerEventData.position;
                Vector2 vector2 = CUIUtility.WorldToScreenPoint(srcFormScript.GetCamera(), uiEvent.m_srcWidget.transform.position);
                float num = position.x - vector2.x;
                float num2 = position.y - vector2.y;
                num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
                num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
                float x = num * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.x;
                float z = num2 * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.y;
                if (MonoSingleton<CameraSystem>.instance.MobaCamera != null)
                {
                    MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation(new Vector3(x, 1f, z));
                }
                if (this.m_miniMapCameraFrame != null)
                {
                    if (!this.m_miniMapCameraFrame.IsCameraFrameShow)
                    {
                        this.m_miniMapCameraFrame.Show();
                        this.m_miniMapCameraFrame.ShowNormal();
                    }
                    this.m_miniMapCameraFrame.SetPos(num, num2);
                }
            }
        }

        private void OnDragMiniMapEnd(CUIEvent uievent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((!Singleton<WatchController>.GetInstance().IsWatching && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain.handle != null))) && ((hostPlayer.Captain.handle.ActorControl != null) && !hostPlayer.Captain.handle.ActorControl.IsDeadState))
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
                if (this.m_miniMapCameraFrame != null)
                {
                    this.m_miniMapCameraFrame.Hide();
                }
            }
        }

        private void OnMiniMap_Click_Down(CUIEvent uievent)
        {
            SignalPanel theSignalPanel = Singleton<CBattleSystem>.GetInstance().TheSignalPanel;
            if (theSignalPanel == null)
            {
                this.Move_CameraToClickPosition(uievent);
            }
            else if (!theSignalPanel.IsUseSingalButton())
            {
                this.Move_CameraToClickPosition(uievent);
            }
        }

        private void OnMiniMap_Click_Up(CUIEvent uievent)
        {
            if (this.m_miniMapCameraFrame != null)
            {
                this.m_miniMapCameraFrame.Hide();
            }
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((!Singleton<WatchController>.GetInstance().IsWatching && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && (hostPlayer.Captain.handle != null))) && ((hostPlayer.Captain.handle.ActorControl != null) && !hostPlayer.Captain.handle.ActorControl.IsDeadState))
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
            }
        }

        private void RefreshMapPointers()
        {
            List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
            for (int i = 0; i < heroActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = heroActors[i];
                if (((handle != 0) && (handle.handle != null)) && (handle.handle.HudControl != null))
                {
                    handle.handle.HudControl.RefreshMapPointerBig();
                }
            }
        }

        private void regEvent()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Open_BigMap, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Open_BigMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CloseBigMap, new CUIEventManager.OnUIEventHandler(this.OnCloseBigMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_5_Dalong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Dalong));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_5_Xiaolong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Xiaolong));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_3_long, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_3_long));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Organ, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Organ));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Hero, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Hero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Map, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Map));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBuildingAttacked));
        }

        private void send_signal(CUIEvent uiEvent, GameObject img, int signal_id = 0)
        {
            if ((uiEvent != null) && (img != null))
            {
                byte type = (byte) uiEvent.m_eventParams.tag2;
                uint tagUInt = uiEvent.m_eventParams.tagUInt;
                if (signal_id == 0)
                {
                    signal_id = uiEvent.m_eventParams.tag3;
                }
                this.Switch(EMapType.Mini);
                SignalPanel panel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel();
                if (panel != null)
                {
                    switch (type)
                    {
                        case 3:
                        case 1:
                        case 2:
                        case 6:
                        case 4:
                        case 5:
                            panel.SendCommand_SignalMiniMap_Target((byte) signal_id, type, tagUInt);
                            return;
                    }
                    Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                    if (hostPlayer != null)
                    {
                        ActorRoot root = ((hostPlayer == null) || (hostPlayer.Captain == 0)) ? null : hostPlayer.Captain.handle;
                        if (root != null)
                        {
                            Vector2 vector = CUIUtility.WorldToScreenPoint(uiEvent.m_srcFormScript.GetCamera(), img.transform.position);
                            float num4 = uiEvent.m_pointerEventData.position.x - vector.x;
                            float num5 = uiEvent.m_pointerEventData.position.y - vector.y;
                            num4 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num4);
                            num5 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num5);
                            VInt3 zero = VInt3.zero;
                            zero.x = (int) (num4 * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.x);
                            zero.y = (root == null) ? ((int) 0.15f) : ((int) ((Vector3) root.location).y);
                            zero.z = (int) (num5 * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.y);
                            panel.SendCommand_SignalMiniMap_Position((byte) signal_id, zero);
                        }
                    }
                }
            }
        }

        public static Image SetTower_Image(bool bAlie, int value, GameObject mapPointer, CUIFormScript formScript)
        {
            if ((mapPointer == null) || (formScript == null))
            {
                return null;
            }
            Image component = mapPointer.GetComponent<Image>();
            Image image = mapPointer.transform.Find("front").GetComponent<Image>();
            if ((component == null) || (image == null))
            {
                return null;
            }
            if (value == 2)
            {
                component.SetSprite(!bAlie ? enemy_base : self_base, formScript, true, false, false);
                image.SetSprite(!bAlie ? enemy_base_outline : self_base_outline, formScript, true, false, false);
                return component;
            }
            if ((value == 1) || (value == 4))
            {
                component.SetSprite(!bAlie ? enemy_tower : self_tower, formScript, true, false, false);
                image.SetSprite(!bAlie ? enemy_tower_outline : self_tower_outline, formScript, true, false, false);
            }
            return component;
        }

        public void Switch(EMapType type)
        {
            this.curMapType = type;
            if (this._ownerForm != null)
            {
                GameObject widget = this._ownerForm.GetWidget(0x2d);
                if (widget != null)
                {
                    if (this.curMapType == EMapType.Mini)
                    {
                        if (this.mmRoot != null)
                        {
                            CUICommonSystem.SetObjActive(this.mmRoot, true);
                        }
                        if (this.bmRoot != null)
                        {
                            CUICommonSystem.SetObjActive(this.bmRoot, false);
                        }
                        if (widget != null)
                        {
                            widget.CustomSetActive(true);
                        }
                    }
                    else if (this.curMapType == EMapType.Big)
                    {
                        if (this.mmRoot != null)
                        {
                            CUICommonSystem.SetObjActive(this.mmRoot, false);
                        }
                        if (this.bmRoot != null)
                        {
                            CUICommonSystem.SetObjActive(this.bmRoot, true);
                        }
                        if (widget != null)
                        {
                            widget.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        if (this.mmRoot != null)
                        {
                            this.mmRoot.CustomSetActive(false);
                        }
                        if (this.bmRoot != null)
                        {
                            this.bmRoot.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void unRegEvent()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Open_BigMap, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Open_BigMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CloseBigMap, new CUIEventManager.OnUIEventHandler(this.OnCloseBigMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_5_Dalong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Dalong));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_5_Xiaolong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Xiaolong));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_3_long, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_3_long));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Organ, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Organ));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Hero, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Hero));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Map, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Map));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBuildingAttacked));
        }

        public void Update()
        {
            if (this.m_miniMapCameraFrame != null)
            {
                this.m_miniMapCameraFrame.Update();
            }
        }

        public GameObject bmpcAlies { get; private set; }

        public GameObject bmpcDragon { get; private set; }

        public GameObject bmpcEffect { get; private set; }

        public GameObject bmpcEnemy { get; private set; }

        public GameObject bmpcEye { get; private set; }

        public GameObject bmpcHero { get; private set; }

        public GameObject bmpcOrgan { get; private set; }

        public GameObject bmpcSignal { get; private set; }

        public GameObject bmRoot { get; private set; }

        public GameObject mmpcAlies { get; private set; }

        public GameObject mmpcDragon { get; private set; }

        public GameObject mmpcEffect { get; private set; }

        public GameObject mmpcEnemy { get; private set; }

        public GameObject mmpcEye { get; private set; }

        public GameObject mmpcHero { get; private set; }

        public GameObject mmpcOrgan { get; private set; }

        public GameObject mmpcSignal { get; private set; }

        public GameObject mmRoot { get; private set; }

        public enum ElementType
        {
            None,
            Tower,
            Base,
            Hero,
            Dragon_5_big,
            Dragon_5_small,
            Dragon_3
        }

        public enum EMapType
        {
            None,
            Mini,
            Big
        }
    }
}

