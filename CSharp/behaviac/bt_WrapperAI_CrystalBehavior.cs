namespace behaviac
{
    using System;

    public static class bt_WrapperAI_CrystalBehavior
    {
        public static bool build_behavior_tree(BehaviorTree bt)
        {
            bt.SetClassNameString("BehaviorTree");
            bt.SetId(-1);
            bt.SetName("WrapperAI/CrystalBehavior");
            bt.AddPar("Assets.Scripts.GameLogic.SkillSlotType", "p_curSlotType", "SLOT_SKILL_0", string.Empty);
            bt.AddPar("uint", "p_enemyID", "0", string.Empty);
            bt.AddPar("int", "p_srchRange", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_AttackMoveDest", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("bool", "p_IsAttackMove_Attack", "false", string.Empty);
            bt.AddPar("bool", "p_AttackIsFinished", "true", string.Empty);
            bt.AddPar("uint", "p_CmdID", "0", string.Empty);
            DecoratorLoop_bt_WrapperAI_CrystalBehavior_node14 pChild = new DecoratorLoop_bt_WrapperAI_CrystalBehavior_node14();
            pChild.SetClassNameString("DecoratorLoop");
            pChild.SetId(14);
            bt.AddChild(pChild);
            Sequence sequence = new Sequence();
            sequence.SetClassNameString("Sequence");
            sequence.SetId(0);
            pChild.AddChild(sequence);
            Condition_bt_WrapperAI_CrystalBehavior_node3 _node2 = new Condition_bt_WrapperAI_CrystalBehavior_node3();
            _node2.SetClassNameString("Condition");
            _node2.SetId(3);
            sequence.AddChild(_node2);
            sequence.SetHasEvents(sequence.HasEvents() | _node2.HasEvents());
            Sequence sequence2 = new Sequence();
            sequence2.SetClassNameString("Sequence");
            sequence2.SetId(0x3b);
            sequence.AddChild(sequence2);
            Action_bt_WrapperAI_CrystalBehavior_node4 _node3 = new Action_bt_WrapperAI_CrystalBehavior_node4();
            _node3.SetClassNameString("Action");
            _node3.SetId(4);
            sequence2.AddChild(_node3);
            sequence2.SetHasEvents(sequence2.HasEvents() | _node3.HasEvents());
            Action_bt_WrapperAI_CrystalBehavior_node5 _node4 = new Action_bt_WrapperAI_CrystalBehavior_node5();
            _node4.SetClassNameString("Action");
            _node4.SetId(5);
            sequence2.AddChild(_node4);
            sequence2.SetHasEvents(sequence2.HasEvents() | _node4.HasEvents());
            DecoratorLoop_bt_WrapperAI_CrystalBehavior_node65 _node5 = new DecoratorLoop_bt_WrapperAI_CrystalBehavior_node65();
            _node5.SetClassNameString("DecoratorLoop");
            _node5.SetId(0x41);
            sequence2.AddChild(_node5);
            Noop noop = new Noop();
            noop.SetClassNameString("Noop");
            noop.SetId(0x42);
            _node5.AddChild(noop);
            _node5.SetHasEvents(_node5.HasEvents() | noop.HasEvents());
            sequence2.SetHasEvents(sequence2.HasEvents() | _node5.HasEvents());
            sequence.SetHasEvents(sequence.HasEvents() | sequence2.HasEvents());
            pChild.SetHasEvents(pChild.HasEvents() | sequence.HasEvents());
            bt.SetHasEvents(bt.HasEvents() | pChild.HasEvents());
            return true;
        }
    }
}

