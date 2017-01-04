namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    public class SkinResourceHelper
    {
        public static string GetResourceName(ref PoolObjHandle<ActorRoot> _attack, string _resName, bool _bUseAdvanceSkin)
        {
            uint num;
            if (((_attack == 0) || (_attack.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)) || (_attack.handle.ActorControl == null))
            {
                return _resName;
            }
            HeroWrapper actorControl = (HeroWrapper) _attack.handle.ActorControl;
            if ((actorControl == null) || !actorControl.GetSkinCfgID(out num))
            {
                return _resName;
            }
            int length = _resName.LastIndexOf('/');
            StringBuilder builder = new StringBuilder(_resName);
            StringBuilder builder2 = new StringBuilder(actorControl.GetSkinEffectPath());
            if (length < 0)
            {
                return _resName;
            }
            builder.Remove(0, length);
            builder2.Append(builder);
            if (_bUseAdvanceSkin)
            {
                int advanceSkinIndex = actorControl.GetAdvanceSkinIndex();
                if (advanceSkinIndex > 0)
                {
                    builder2.AppendFormat("_level{0}", advanceSkinIndex);
                }
            }
            return builder2.ToString();
        }

        public static string GetResourceName(AGE.Action _action, string _resName, bool _bUseAdvanceSkin)
        {
            uint num;
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            if (((refParamObject == null) || (refParamObject.Originator == 0)) || ((refParamObject.Originator.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero) || (refParamObject.Originator.handle.ActorControl == null)))
            {
                return _resName;
            }
            HeroWrapper actorControl = (HeroWrapper) refParamObject.Originator.handle.ActorControl;
            if ((actorControl == null) || !actorControl.GetSkinCfgID(out num))
            {
                return _resName;
            }
            int length = _resName.LastIndexOf('/');
            StringBuilder builder = new StringBuilder(_resName);
            StringBuilder builder2 = new StringBuilder(actorControl.GetSkinEffectPath());
            if (length < 0)
            {
                return _resName;
            }
            builder.Remove(0, length);
            builder2.Append(builder);
            if (_bUseAdvanceSkin)
            {
                int advanceSkinIndex = actorControl.GetAdvanceSkinIndex();
                if (advanceSkinIndex > 0)
                {
                    builder2.AppendFormat("_level{0}", advanceSkinIndex);
                }
            }
            return builder2.ToString();
        }

        public static string GetSkinResourceName(int configID, int markID, string resName, int advancelevel = 0)
        {
            if ((configID == 0) || (markID == 0))
            {
                return resName;
            }
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((long) configID);
            if (dataByKey == null)
            {
                return resName;
            }
            int length = resName.LastIndexOf('/');
            StringBuilder builder = new StringBuilder(resName);
            string str = "prefab_skill_effects/hero_skill_effects/";
            StringBuilder builder2 = new StringBuilder(str);
            builder2.AppendFormat("{0}_{1}/{2}", configID, dataByKey.szNamePinYin, markID);
            if (length < 0)
            {
                return resName;
            }
            builder.Remove(0, length);
            builder2.Append(builder);
            if (advancelevel > 0)
            {
                builder2.AppendFormat("_level{0}", advancelevel);
            }
            return builder2.ToString();
        }
    }
}

