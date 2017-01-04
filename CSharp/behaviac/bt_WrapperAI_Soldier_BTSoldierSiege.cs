﻿namespace behaviac
{
    using System;

    public static class bt_WrapperAI_Soldier_BTSoldierSiege
    {
        public static bool build_behavior_tree(BehaviorTree bt)
        {
            bt.SetClassNameString("BehaviorTree");
            bt.SetId(-1);
            bt.SetName("WrapperAI/Soldier/BTSoldierSiege");
            bt.AddPar("Assets.Scripts.GameLogic.SkillSlotType", "p_curSlotType", "SLOT_SKILL_0", string.Empty);
            bt.AddPar("uint", "p_targetID", "0", string.Empty);
            bt.AddPar("int", "p_srchRange", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_AttackMoveDest", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            bt.AddPar("bool", "p_IsAttackMove_Attack", "false", string.Empty);
            bt.AddPar("bool", "p_AttackIsFinished", "true", string.Empty);
            bt.AddPar("uint", "p_CmdID", "0", string.Empty);
            bt.AddPar("UnityEngine.Vector3", "p_attackPathCurTargetPos", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
            Sequence pChild = new Sequence();
            pChild.SetClassNameString("Sequence");
            pChild.SetId(0);
            bt.AddChild(pChild);
            Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node106 _node = new Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node106();
            _node.SetClassNameString("Assignment");
            _node.SetId(0x6a);
            pChild.AddChild(_node);
            pChild.SetHasEvents(pChild.HasEvents() | _node.HasEvents());
            DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierSiege_node14 _node2 = new DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierSiege_node14();
            _node2.SetClassNameString("DecoratorLoop");
            _node2.SetId(14);
            pChild.AddChild(_node2);
            SelectorLoop loop = new SelectorLoop();
            loop.SetClassNameString("SelectorLoop");
            loop.SetId(1);
            _node2.AddChild(loop);
            WithPrecondition precondition = new WithPrecondition();
            precondition.SetClassNameString("WithPrecondition");
            precondition.SetId(0x3b);
            loop.AddChild(precondition);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node78 _node3 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node78();
            _node3.SetClassNameString("Condition");
            _node3.SetId(0x4e);
            precondition.AddChild(_node3);
            precondition.SetHasEvents(precondition.HasEvents() | _node3.HasEvents());
            Sequence sequence2 = new Sequence();
            sequence2.SetClassNameString("Sequence");
            sequence2.SetId(0x69);
            precondition.AddChild(sequence2);
            Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node107 _node4 = new Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node107();
            _node4.SetClassNameString("Assignment");
            _node4.SetId(0x6b);
            sequence2.AddChild(_node4);
            sequence2.SetHasEvents(sequence2.HasEvents() | _node4.HasEvents());
            IfElse @else = new IfElse();
            @else.SetClassNameString("IfElse");
            @else.SetId(0x6c);
            sequence2.AddChild(@else);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node109 _node5 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node109();
            _node5.SetClassNameString("Condition");
            _node5.SetId(0x6d);
            @else.AddChild(_node5);
            @else.SetHasEvents(@else.HasEvents() | _node5.HasEvents());
            Sequence sequence3 = new Sequence();
            sequence3.SetClassNameString("Sequence");
            sequence3.SetId(110);
            @else.AddChild(sequence3);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node111 _node6 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node111();
            _node6.SetClassNameString("Action");
            _node6.SetId(0x6f);
            sequence3.AddChild(_node6);
            sequence3.SetHasEvents(sequence3.HasEvents() | _node6.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node112 _node7 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node112();
            _node7.SetClassNameString("Action");
            _node7.SetId(0x70);
            sequence3.AddChild(_node7);
            sequence3.SetHasEvents(sequence3.HasEvents() | _node7.HasEvents());
            IfElse else2 = new IfElse();
            else2.SetClassNameString("IfElse");
            else2.SetId(5);
            sequence3.AddChild(else2);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node6 _node8 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node6();
            _node8.SetClassNameString("Condition");
            _node8.SetId(6);
            else2.AddChild(_node8);
            else2.SetHasEvents(else2.HasEvents() | _node8.HasEvents());
            Sequence sequence4 = new Sequence();
            sequence4.SetClassNameString("Sequence");
            sequence4.SetId(7);
            else2.AddChild(sequence4);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node23 _node9 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node23();
            _node9.SetClassNameString("Action");
            _node9.SetId(0x17);
            sequence4.AddChild(_node9);
            sequence4.SetHasEvents(sequence4.HasEvents() | _node9.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node113 _node10 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node113();
            _node10.SetClassNameString("Action");
            _node10.SetId(0x71);
            sequence4.AddChild(_node10);
            sequence4.SetHasEvents(sequence4.HasEvents() | _node10.HasEvents());
            Selector selector = new Selector();
            selector.SetClassNameString("Selector");
            selector.SetId(0x11);
            sequence4.AddChild(selector);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node114 _node11 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node114();
            _node11.SetClassNameString("Action");
            _node11.SetId(0x72);
            selector.AddChild(_node11);
            selector.SetHasEvents(selector.HasEvents() | _node11.HasEvents());
            DecoratorAlwaysFailure_bt_WrapperAI_Soldier_BTSoldierSiege_node19 _node12 = new DecoratorAlwaysFailure_bt_WrapperAI_Soldier_BTSoldierSiege_node19();
            _node12.SetClassNameString("DecoratorAlwaysFailure");
            _node12.SetId(0x13);
            selector.AddChild(_node12);
            Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node18 _node13 = new Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node18();
            _node13.SetClassNameString("Assignment");
            _node13.SetId(0x12);
            _node12.AddChild(_node13);
            _node12.SetHasEvents(_node12.HasEvents() | _node13.HasEvents());
            selector.SetHasEvents(selector.HasEvents() | _node12.HasEvents());
            sequence4.SetHasEvents(sequence4.HasEvents() | selector.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node140 _node14 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node140();
            _node14.SetClassNameString("Action");
            _node14.SetId(140);
            sequence4.AddChild(_node14);
            sequence4.SetHasEvents(sequence4.HasEvents() | _node14.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node116 _node15 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node116();
            _node15.SetClassNameString("Action");
            _node15.SetId(0x74);
            sequence4.AddChild(_node15);
            sequence4.SetHasEvents(sequence4.HasEvents() | _node15.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node117 _node16 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node117();
            _node16.SetClassNameString("DecoratorLoopUntil");
            _node16.SetId(0x75);
            sequence4.AddChild(_node16);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node118 _node17 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node118();
            _node17.SetClassNameString("Condition");
            _node17.SetId(0x76);
            _node16.AddChild(_node17);
            _node16.SetHasEvents(_node16.HasEvents() | _node17.HasEvents());
            sequence4.SetHasEvents(sequence4.HasEvents() | _node16.HasEvents());
            else2.SetHasEvents(else2.HasEvents() | sequence4.HasEvents());
            Sequence sequence5 = new Sequence();
            sequence5.SetClassNameString("Sequence");
            sequence5.SetId(9);
            else2.AddChild(sequence5);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node4 _node18 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node4();
            _node18.SetClassNameString("Action");
            _node18.SetId(4);
            sequence5.AddChild(_node18);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node18.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node13 _node19 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node13();
            _node19.SetClassNameString("Action");
            _node19.SetId(13);
            sequence5.AddChild(_node19);
            sequence5.SetHasEvents(sequence5.HasEvents() | _node19.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node15 _node20 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node15();
            _node20.SetClassNameString("DecoratorLoopUntil");
            _node20.SetId(15);
            sequence5.AddChild(_node20);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node16 _node21 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node16();
            _node21.SetClassNameString("Condition");
            _node21.SetId(0x10);
            _node20.AddChild(_node21);
            _node20.SetHasEvents(_node20.HasEvents() | _node21.HasEvents());
            sequence5.SetHasEvents(sequence5.HasEvents() | _node20.HasEvents());
            else2.SetHasEvents(else2.HasEvents() | sequence5.HasEvents());
            sequence3.SetHasEvents(sequence3.HasEvents() | else2.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node28 _node22 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node28();
            _node22.SetClassNameString("Action");
            _node22.SetId(0x1c);
            sequence3.AddChild(_node22);
            sequence3.SetHasEvents(sequence3.HasEvents() | _node22.HasEvents());
            @else.SetHasEvents(@else.HasEvents() | sequence3.HasEvents());
            Sequence sequence6 = new Sequence();
            sequence6.SetClassNameString("Sequence");
            sequence6.SetId(0x80);
            @else.AddChild(sequence6);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node129 _node23 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node129();
            _node23.SetClassNameString("Action");
            _node23.SetId(0x81);
            sequence6.AddChild(_node23);
            sequence6.SetHasEvents(sequence6.HasEvents() | _node23.HasEvents());
            IfElse else3 = new IfElse();
            else3.SetClassNameString("IfElse");
            else3.SetId(0x77);
            sequence6.AddChild(else3);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node120 _node24 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node120();
            _node24.SetClassNameString("Condition");
            _node24.SetId(120);
            else3.AddChild(_node24);
            else3.SetHasEvents(else3.HasEvents() | _node24.HasEvents());
            Sequence sequence7 = new Sequence();
            sequence7.SetClassNameString("Sequence");
            sequence7.SetId(0x79);
            else3.AddChild(sequence7);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node24 _node25 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node24();
            _node25.SetClassNameString("Action");
            _node25.SetId(0x18);
            sequence7.AddChild(_node25);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node25.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node444 _node26 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node444();
            _node26.SetClassNameString("Action");
            _node26.SetId(0x1bc);
            sequence7.AddChild(_node26);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node26.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node122 _node27 = new Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node122();
            _node27.SetClassNameString("Assignment");
            _node27.SetId(0x7a);
            sequence7.AddChild(_node27);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node27.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node123 _node28 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node123();
            _node28.SetClassNameString("Action");
            _node28.SetId(0x7b);
            sequence7.AddChild(_node28);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node28.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node124 _node29 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node124();
            _node29.SetClassNameString("Action");
            _node29.SetId(0x7c);
            sequence7.AddChild(_node29);
            sequence7.SetHasEvents(sequence7.HasEvents() | _node29.HasEvents());
            else3.SetHasEvents(else3.HasEvents() | sequence7.HasEvents());
            Sequence sequence8 = new Sequence();
            sequence8.SetClassNameString("Sequence");
            sequence8.SetId(0x7d);
            else3.AddChild(sequence8);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node126 _node30 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node126();
            _node30.SetClassNameString("Action");
            _node30.SetId(0x7e);
            sequence8.AddChild(_node30);
            sequence8.SetHasEvents(sequence8.HasEvents() | _node30.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node127 _node31 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node127();
            _node31.SetClassNameString("Action");
            _node31.SetId(0x7f);
            sequence8.AddChild(_node31);
            sequence8.SetHasEvents(sequence8.HasEvents() | _node31.HasEvents());
            else3.SetHasEvents(else3.HasEvents() | sequence8.HasEvents());
            sequence6.SetHasEvents(sequence6.HasEvents() | else3.HasEvents());
            @else.SetHasEvents(@else.HasEvents() | sequence6.HasEvents());
            sequence2.SetHasEvents(sequence2.HasEvents() | @else.HasEvents());
            precondition.SetHasEvents(precondition.HasEvents() | sequence2.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition.HasEvents());
            WithPrecondition precondition2 = new WithPrecondition();
            precondition2.SetClassNameString("WithPrecondition");
            precondition2.SetId(0x203);
            loop.AddChild(precondition2);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node516 _node32 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node516();
            _node32.SetClassNameString("Condition");
            _node32.SetId(0x204);
            precondition2.AddChild(_node32);
            precondition2.SetHasEvents(precondition2.HasEvents() | _node32.HasEvents());
            Selector selector2 = new Selector();
            selector2.SetClassNameString("Selector");
            selector2.SetId(2);
            precondition2.AddChild(selector2);
            Sequence sequence9 = new Sequence();
            sequence9.SetClassNameString("Sequence");
            sequence9.SetId(3);
            selector2.AddChild(sequence9);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node10 _node33 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node10();
            _node33.SetClassNameString("Condition");
            _node33.SetId(10);
            sequence9.AddChild(_node33);
            sequence9.SetHasEvents(sequence9.HasEvents() | _node33.HasEvents());
            Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node11 _node34 = new Assignment_bt_WrapperAI_Soldier_BTSoldierSiege_node11();
            _node34.SetClassNameString("Assignment");
            _node34.SetId(11);
            sequence9.AddChild(_node34);
            sequence9.SetHasEvents(sequence9.HasEvents() | _node34.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node12 _node35 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node12();
            _node35.SetClassNameString("Action");
            _node35.SetId(12);
            sequence9.AddChild(_node35);
            sequence9.SetHasEvents(sequence9.HasEvents() | _node35.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node8 _node36 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node8();
            _node36.SetClassNameString("Action");
            _node36.SetId(8);
            sequence9.AddChild(_node36);
            sequence9.SetHasEvents(sequence9.HasEvents() | _node36.HasEvents());
            selector2.SetHasEvents(selector2.HasEvents() | sequence9.HasEvents());
            Sequence sequence10 = new Sequence();
            sequence10.SetClassNameString("Sequence");
            sequence10.SetId(0x205);
            selector2.AddChild(sequence10);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node20 _node37 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node20();
            _node37.SetClassNameString("Condition");
            _node37.SetId(20);
            sequence10.AddChild(_node37);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node37.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node518 _node38 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node518();
            _node38.SetClassNameString("Action");
            _node38.SetId(0x206);
            sequence10.AddChild(_node38);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node38.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node519 _node39 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node519();
            _node39.SetClassNameString("Action");
            _node39.SetId(0x207);
            sequence10.AddChild(_node39);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node39.HasEvents());
            IfElse else4 = new IfElse();
            else4.SetClassNameString("IfElse");
            else4.SetId(520);
            sequence10.AddChild(else4);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node521 _node40 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node521();
            _node40.SetClassNameString("Condition");
            _node40.SetId(0x209);
            else4.AddChild(_node40);
            else4.SetHasEvents(else4.HasEvents() | _node40.HasEvents());
            Sequence sequence11 = new Sequence();
            sequence11.SetClassNameString("Sequence");
            sequence11.SetId(0x20a);
            else4.AddChild(sequence11);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node523 _node41 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node523();
            _node41.SetClassNameString("Action");
            _node41.SetId(0x20b);
            sequence11.AddChild(_node41);
            sequence11.SetHasEvents(sequence11.HasEvents() | _node41.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node524 _node42 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node524();
            _node42.SetClassNameString("Action");
            _node42.SetId(0x20c);
            sequence11.AddChild(_node42);
            sequence11.SetHasEvents(sequence11.HasEvents() | _node42.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node526 _node43 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node526();
            _node43.SetClassNameString("Action");
            _node43.SetId(0x20e);
            sequence11.AddChild(_node43);
            sequence11.SetHasEvents(sequence11.HasEvents() | _node43.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node25 _node44 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node25();
            _node44.SetClassNameString("Action");
            _node44.SetId(0x19);
            sequence11.AddChild(_node44);
            sequence11.SetHasEvents(sequence11.HasEvents() | _node44.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node530 _node45 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node530();
            _node45.SetClassNameString("Action");
            _node45.SetId(530);
            sequence11.AddChild(_node45);
            sequence11.SetHasEvents(sequence11.HasEvents() | _node45.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node531 _node46 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node531();
            _node46.SetClassNameString("DecoratorLoopUntil");
            _node46.SetId(0x213);
            sequence11.AddChild(_node46);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node532 _node47 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node532();
            _node47.SetClassNameString("Condition");
            _node47.SetId(0x214);
            _node46.AddChild(_node47);
            _node46.SetHasEvents(_node46.HasEvents() | _node47.HasEvents());
            sequence11.SetHasEvents(sequence11.HasEvents() | _node46.HasEvents());
            else4.SetHasEvents(else4.HasEvents() | sequence11.HasEvents());
            Sequence sequence12 = new Sequence();
            sequence12.SetClassNameString("Sequence");
            sequence12.SetId(0x215);
            else4.AddChild(sequence12);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node26 _node48 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node26();
            _node48.SetClassNameString("Action");
            _node48.SetId(0x1a);
            sequence12.AddChild(_node48);
            sequence12.SetHasEvents(sequence12.HasEvents() | _node48.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node535 _node49 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node535();
            _node49.SetClassNameString("Action");
            _node49.SetId(0x217);
            sequence12.AddChild(_node49);
            sequence12.SetHasEvents(sequence12.HasEvents() | _node49.HasEvents());
            DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node536 _node50 = new DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierSiege_node536();
            _node50.SetClassNameString("DecoratorLoopUntil");
            _node50.SetId(0x218);
            sequence12.AddChild(_node50);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node537 _node51 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node537();
            _node51.SetClassNameString("Condition");
            _node51.SetId(0x219);
            _node50.AddChild(_node51);
            _node50.SetHasEvents(_node50.HasEvents() | _node51.HasEvents());
            sequence12.SetHasEvents(sequence12.HasEvents() | _node50.HasEvents());
            else4.SetHasEvents(else4.HasEvents() | sequence12.HasEvents());
            sequence10.SetHasEvents(sequence10.HasEvents() | else4.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node538 _node52 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node538();
            _node52.SetClassNameString("Action");
            _node52.SetId(0x21a);
            sequence10.AddChild(_node52);
            sequence10.SetHasEvents(sequence10.HasEvents() | _node52.HasEvents());
            selector2.SetHasEvents(selector2.HasEvents() | sequence10.HasEvents());
            precondition2.SetHasEvents(precondition2.HasEvents() | selector2.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition2.HasEvents());
            WithPrecondition precondition3 = new WithPrecondition();
            precondition3.SetClassNameString("WithPrecondition");
            precondition3.SetId(0x15);
            loop.AddChild(precondition3);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node22 _node53 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node22();
            _node53.SetClassNameString("Condition");
            _node53.SetId(0x16);
            precondition3.AddChild(_node53);
            precondition3.SetHasEvents(precondition3.HasEvents() | _node53.HasEvents());
            Sequence sequence13 = new Sequence();
            sequence13.SetClassNameString("Sequence");
            sequence13.SetId(0x1b);
            precondition3.AddChild(sequence13);
            Selector selector3 = new Selector();
            selector3.SetClassNameString("Selector");
            selector3.SetId(0x1e7);
            sequence13.AddChild(selector3);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node488 _node54 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node488();
            _node54.SetClassNameString("Action");
            _node54.SetId(0x1e8);
            selector3.AddChild(_node54);
            selector3.SetHasEvents(selector3.HasEvents() | _node54.HasEvents());
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node35 _node55 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node35();
            _node55.SetClassNameString("Action");
            _node55.SetId(0x23);
            selector3.AddChild(_node55);
            selector3.SetHasEvents(selector3.HasEvents() | _node55.HasEvents());
            sequence13.SetHasEvents(sequence13.HasEvents() | selector3.HasEvents());
            DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierSiege_node65 _node56 = new DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierSiege_node65();
            _node56.SetClassNameString("DecoratorLoop");
            _node56.SetId(0x41);
            sequence13.AddChild(_node56);
            Noop noop = new Noop();
            noop.SetClassNameString("Noop");
            noop.SetId(0x42);
            _node56.AddChild(noop);
            _node56.SetHasEvents(_node56.HasEvents() | noop.HasEvents());
            sequence13.SetHasEvents(sequence13.HasEvents() | _node56.HasEvents());
            precondition3.SetHasEvents(precondition3.HasEvents() | sequence13.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition3.HasEvents());
            WithPrecondition precondition4 = new WithPrecondition();
            precondition4.SetClassNameString("WithPrecondition");
            precondition4.SetId(450);
            loop.AddChild(precondition4);
            Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node454 _node57 = new Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node454();
            _node57.SetClassNameString("Condition");
            _node57.SetId(0x1c6);
            precondition4.AddChild(_node57);
            precondition4.SetHasEvents(precondition4.HasEvents() | _node57.HasEvents());
            Sequence sequence14 = new Sequence();
            sequence14.SetClassNameString("Sequence");
            sequence14.SetId(0x1c7);
            precondition4.AddChild(sequence14);
            Action_bt_WrapperAI_Soldier_BTSoldierSiege_node456 _node58 = new Action_bt_WrapperAI_Soldier_BTSoldierSiege_node456();
            _node58.SetClassNameString("Action");
            _node58.SetId(0x1c8);
            sequence14.AddChild(_node58);
            sequence14.SetHasEvents(sequence14.HasEvents() | _node58.HasEvents());
            DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierSiege_node457 _node59 = new DecoratorLoop_bt_WrapperAI_Soldier_BTSoldierSiege_node457();
            _node59.SetClassNameString("DecoratorLoop");
            _node59.SetId(0x1c9);
            sequence14.AddChild(_node59);
            Noop noop2 = new Noop();
            noop2.SetClassNameString("Noop");
            noop2.SetId(0x1ca);
            _node59.AddChild(noop2);
            _node59.SetHasEvents(_node59.HasEvents() | noop2.HasEvents());
            sequence14.SetHasEvents(sequence14.HasEvents() | _node59.HasEvents());
            precondition4.SetHasEvents(precondition4.HasEvents() | sequence14.HasEvents());
            loop.SetHasEvents(loop.HasEvents() | precondition4.HasEvents());
            _node2.SetHasEvents(_node2.HasEvents() | loop.HasEvents());
            pChild.SetHasEvents(pChild.HasEvents() | _node2.HasEvents());
            bt.SetHasEvents(bt.HasEvents() | pChild.HasEvents());
            return true;
        }
    }
}
