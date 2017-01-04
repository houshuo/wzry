namespace behaviac
{
    using System;

    public static class bt_WrapperAI_Monster_BTMonsterBaoxiang
    {
        public static bool build_behavior_tree(BehaviorTree bt)
        {
            bt.SetClassNameString("BehaviorTree");
            bt.SetId(-1);
            bt.SetName("WrapperAI/Monster/BTMonsterBaoxiang");
            bt.AddPar("Assets.Scripts.GameLogic.SkillSlotType", "p_curSlotType", "SLOT_SKILL_0", string.Empty);
            bt.AddPar("uint", "p_targetID", "0", string.Empty);
            bt.AddPar("int", "p_srchRange", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_AttackMoveDest", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("bool", "p_IsAttackMove_Attack", "false", string.Empty);
            bt.AddPar("bool", "p_AttackIsFinished", "true", string.Empty);
            bt.AddPar("uint", "p_CmdID", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_attackPathCurTargetPos", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("int", "p_skillAttackRange", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_orignalPos", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("int", "p_follow_range", "9000", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_orignalDirection", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("int", "p_pursuitRange", "0", string.Empty);
            bt.AddPar("int", "p_waitFramesToSearch", "10", string.Empty);
            bt.AddPar("uint", "p_selfID", "0", string.Empty);
            DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node14 pChild = new DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node14();
            pChild.SetClassNameString("DecoratorLoop");
            pChild.SetId(14);
            bt.AddChild(pChild);
            Sequence sequence = new Sequence();
            sequence.SetClassNameString("Sequence");
            sequence.SetId(0x16);
            pChild.AddChild(sequence);
            Condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node3 _node2 = new Condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node3();
            _node2.SetClassNameString("Condition");
            _node2.SetId(3);
            sequence.AddChild(_node2);
            sequence.SetHasEvents(sequence.HasEvents() | _node2.HasEvents());
            Action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node4 _node3 = new Action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node4();
            _node3.SetClassNameString("Action");
            _node3.SetId(4);
            sequence.AddChild(_node3);
            sequence.SetHasEvents(sequence.HasEvents() | _node3.HasEvents());
            DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node23 _node4 = new DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node23();
            _node4.SetClassNameString("DecoratorLoop");
            _node4.SetId(0x17);
            sequence.AddChild(_node4);
            Noop noop = new Noop();
            noop.SetClassNameString("Noop");
            noop.SetId(0x1ca);
            _node4.AddChild(noop);
            _node4.SetHasEvents(_node4.HasEvents() | noop.HasEvents());
            sequence.SetHasEvents(sequence.HasEvents() | _node4.HasEvents());
            pChild.SetHasEvents(pChild.HasEvents() | sequence.HasEvents());
            bt.SetHasEvents(bt.HasEvents() | pChild.HasEvents());
            return true;
        }
    }
}

