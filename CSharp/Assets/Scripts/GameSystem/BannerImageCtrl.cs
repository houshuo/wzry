namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;

    public class BannerImageCtrl : MonoBehaviour
    {
        private BannerImageSys.BannerImageInfo[] m_AllLoadImageInfo;
        private BannerImageSys.BannerImage m_BannerImage;
        private bool m_bStopAutoMove;
        private bool m_bUpdateStart;
        private int m_CurIdxImagePage;
        public BannerImageSys.BannerPosition m_DisplayPosition;
        private int m_fAdd = 1;
        private float m_fBeginTime;
        public float m_fSpeed = 2f;
        private Vector2 m_lastPos;
        private int[] m_PickIdxList = new int[3];
        public GameObject m_PickObject;
        public CUIStepListScript m_UIListScript;
        public BannerImageSys.BannerType[] m_UseTypeList;

        private void AutoMoveBannerImage()
        {
            if (this.m_UIListScript != null)
            {
                CUIListScript uIListScript = this.m_UIListScript;
                if (uIListScript != null)
                {
                    int elementAmount = uIListScript.GetElementAmount();
                    if (this.m_CurIdxImagePage < 0)
                    {
                        this.m_CurIdxImagePage = 0;
                        this.m_fAdd = 1;
                    }
                    this.m_CurIdxImagePage += this.m_fAdd;
                    if (this.m_CurIdxImagePage >= elementAmount)
                    {
                        this.m_CurIdxImagePage = elementAmount - 1;
                    }
                    if (this.m_CurIdxImagePage == (elementAmount - 1))
                    {
                        this.m_fAdd = -1;
                    }
                    if (this.m_CurIdxImagePage < 0)
                    {
                        this.m_CurIdxImagePage = 0;
                    }
                    if (this.m_CurIdxImagePage == 0)
                    {
                        this.m_fAdd = 1;
                    }
                }
                uIListScript.MoveElementInScrollArea(this.m_CurIdxImagePage, false);
                this.EnablePickObj(this.m_CurIdxImagePage);
            }
        }

        private void BannerImage_OnClickItem(CUIEvent uiEvent)
        {
            if (((this.m_BannerImage != null) && (uiEvent.m_srcWidgetBelongedListScript == this.m_UIListScript)) && (this.m_AllLoadImageInfo != null))
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                int length = this.m_AllLoadImageInfo.Length;
                if (srcWidgetIndexInBelongedList < length)
                {
                    BannerImageSys.BannerImageInfo info = this.m_AllLoadImageInfo[srcWidgetIndexInBelongedList];
                    if (info != null)
                    {
                        if (info.resImgInfo.dwBannerType == 1)
                        {
                            CUICommonSystem.OpenUrl(info.resImgInfo.szHttpUrl, true);
                        }
                        else if (info.resImgInfo.dwBannerType == 2)
                        {
                            CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE) info.resImgInfo.dwJumpEntrance);
                        }
                    }
                }
            }
        }

        private bool checkImageType(BannerImageSys.BannerType kType)
        {
            int length = this.m_UseTypeList.Length;
            for (int i = 0; i < length; i++)
            {
                if (this.m_UseTypeList[i] == kType)
                {
                    return true;
                }
            }
            return false;
        }

        private void EnablePickObj(int idx)
        {
            GameObject pickObject = this.m_PickObject;
            if (pickObject != null)
            {
                CUIContainerScript component = pickObject.GetComponent<CUIContainerScript>();
                if (component != null)
                {
                    for (int i = 0; i < this.m_PickIdxList.Length; i++)
                    {
                        if (i == idx)
                        {
                            GameObject element = component.GetElement(this.m_PickIdxList[i]);
                            if (element != null)
                            {
                                Transform transform = element.transform.FindChild("Image_Pointer");
                                if (transform != null)
                                {
                                    transform.gameObject.CustomSetActive(true);
                                }
                            }
                        }
                        else
                        {
                            GameObject obj4 = component.GetElement(this.m_PickIdxList[i]);
                            if (obj4 != null)
                            {
                                Transform transform2 = obj4.transform.FindChild("Image_Pointer");
                                if (transform2 != null)
                                {
                                    transform2.gameObject.CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitPickObjElement(int nImageCount)
        {
            GameObject pickObject = this.m_PickObject;
            if (pickObject != null)
            {
                CUIContainerScript component = pickObject.GetComponent<CUIContainerScript>();
                if (component != null)
                {
                    component.RecycleAllElement();
                    for (int i = 0; i < nImageCount; i++)
                    {
                        this.m_PickIdxList[i] = component.GetElement();
                    }
                }
            }
        }

        public bool InitSys()
        {
            this.m_BannerImage = MonoSingleton<BannerImageSys>.GetInstance().GetCurBannerImage();
            if (this.m_BannerImage != null)
            {
                long currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                int amount = 0;
                ListView<BannerImageSys.BannerImageInfo> view = new ListView<BannerImageSys.BannerImageInfo>();
                for (int i = 0; i < this.m_BannerImage.ImageListCount; i++)
                {
                    BannerImageSys.BannerImageInfo item = this.m_BannerImage.m_ImageInfoList[i];
                    if ((((item != null) && item.imgLoadSucc) && (this.checkImageType((BannerImageSys.BannerType) item.resImgInfo.dwBannerType) && (((BannerImageSys.BannerPosition) item.resImgInfo.dwLocation) == this.m_DisplayPosition))) && ((item.resImgInfo.dwStartTime < currentUTCTime) && (item.resImgInfo.dwEndTime >= currentUTCTime)))
                    {
                        view.Add(item);
                        amount++;
                    }
                }
                if (amount > 0)
                {
                    if (view != null)
                    {
                        this.m_AllLoadImageInfo = new BannerImageSys.BannerImageInfo[view.Count];
                        for (int j = 0; j < view.Count; j++)
                        {
                            this.m_AllLoadImageInfo[j] = view[j];
                        }
                    }
                    Array.Sort<BannerImageSys.BannerImageInfo>(this.m_AllLoadImageInfo, new Comparison<BannerImageSys.BannerImageInfo>(BannerImageSys.ComparebyShowIdx));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BannerImage_HoldStart, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_Item));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BannerImage_HoldEnd, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_Item));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BannerImage_ClickItem, new CUIEventManager.OnUIEventHandler(this.BannerImage_OnClickItem));
                    CUIStepListScript uIListScript = this.m_UIListScript;
                    uIListScript.SetDontUpdate(true);
                    uIListScript.SetElementAmount(amount);
                    this.m_PickIdxList = new int[amount];
                    this.InitPickObjElement(amount);
                    this.EnablePickObj(0);
                    this.LoadBannerImage();
                    this.m_bUpdateStart = true;
                    this.m_fBeginTime = Time.time;
                    return true;
                }
                Debug.Log("not valide bannerImage");
            }
            return false;
        }

        private void LoadBannerImage()
        {
            if (this.m_AllLoadImageInfo != null)
            {
                int length = this.m_AllLoadImageInfo.Length;
                for (int i = 0; i < length; i++)
                {
                    string szImgUrl = this.m_AllLoadImageInfo[i].resImgInfo.szImgUrl;
                    szImgUrl = string.Format("{0}{1}", BannerImageSys.GlobalLoadPath, szImgUrl);
                    base.StartCoroutine(MonoSingleton<IDIPSys>.GetInstance().DownloadImageByTag(szImgUrl, i, (text2, imageIDX) => this.SetElemntTexture(imageIDX, text2), MonoSingleton<BannerImageSys>.GetInstance().GlobalBannerImagePath));
                }
            }
        }

        private void OnDestroy()
        {
            this.m_BannerImage = null;
            this.m_AllLoadImageInfo = null;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BannerImage_HoldStart, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_Item));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BannerImage_HoldEnd, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_Item));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BannerImage_ClickItem, new CUIEventManager.OnUIEventHandler(this.BannerImage_OnClickItem));
        }

        private void OnHoldEnd_Item(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidgetBelongedListScript == this.m_UIListScript)
            {
                if (uiEvent.m_pointerEventData.position.x <= this.m_lastPos.x)
                {
                    this.m_fAdd = 1;
                }
                else
                {
                    this.m_fAdd = -1;
                }
                this.AutoMoveBannerImage();
                this.m_bStopAutoMove = false;
                this.m_fBeginTime = Time.time;
            }
        }

        private void OnHoldStart_Item(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcWidgetBelongedListScript == this.m_UIListScript)
            {
                this.m_lastPos = uiEvent.m_pointerEventData.position;
                this.m_bStopAutoMove = true;
            }
        }

        private void SetElemntTexture(int idx, Texture2D texture2D)
        {
            if (this.m_UIListScript != null)
            {
                CUIListElementScript elemenet = this.m_UIListScript.GetElemenet(idx);
                if (elemenet != null)
                {
                    elemenet.transform.Find("Img").GetComponent<Image>().SetSprite(Sprite.Create(texture2D, new Rect(0f, 0f, (float) texture2D.width, (float) texture2D.height), new Vector2(0.5f, 0.5f)), ImageAlphaTexLayout.None);
                }
            }
        }

        private void StopAutoMove()
        {
            this.m_bStopAutoMove = true;
        }

        private void Update()
        {
            if ((this.m_bUpdateStart && !this.m_bStopAutoMove) && ((Time.time - this.m_fBeginTime) >= this.m_fSpeed))
            {
                this.AutoMoveBannerImage();
                this.m_fBeginTime = Time.time;
            }
        }
    }
}

