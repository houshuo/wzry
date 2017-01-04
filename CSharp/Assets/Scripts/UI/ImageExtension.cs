namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public static class ImageExtension
    {
        public static void CustomFillAmount(this Image image, float value)
        {
            if ((image != null) && (image.fillAmount != value))
            {
                image.fillAmount = value;
            }
        }

        public static void SetSprite(this Image image, GameObject prefab)
        {
            CUIUtility.SetImageSprite(image, prefab);
        }

        public static void SetSprite(this Image image, Image targetImage)
        {
            CUIUtility.SetImageSprite(image, targetImage);
        }

        public static void SetSprite(this Image image, Sprite sprite, ImageAlphaTexLayout imageAlphaTexLayout)
        {
            if (image is Image2)
            {
                (image as Image2).alphaTexLayout = imageAlphaTexLayout;
            }
            image.sprite = sprite;
        }

        public static void SetSprite(this Image image, string prefabPath, CUIFormScript formScript, bool loadSync = true, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
        {
            CUIUtility.SetImageSprite(image, prefabPath, formScript, loadSync, needCached, unloadBelongedAssetBundleAfterLoaded);
        }
    }
}

