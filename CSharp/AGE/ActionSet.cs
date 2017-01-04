namespace AGE
{
    using System;
    using System.Collections.Generic;

    public class ActionSet
    {
        public Dictionary<AGE.Action, bool> actionSet;

        public ActionSet()
        {
            this.actionSet = new Dictionary<AGE.Action, bool>();
            this.actionSet = new Dictionary<AGE.Action, bool>();
        }

        public ActionSet(Dictionary<AGE.Action, bool> _actionSet)
        {
            this.actionSet = new Dictionary<AGE.Action, bool>();
            this.actionSet = new Dictionary<AGE.Action, bool>();
            foreach (KeyValuePair<AGE.Action, bool> pair in _actionSet)
            {
                this.actionSet.Add(pair.Key, pair.Value);
            }
        }

        public static ActionSet AndSet(ActionSet src1, ActionSet src2)
        {
            ActionSet set = new ActionSet();
            foreach (AGE.Action action in src1.actionSet.Keys)
            {
                if (src2.actionSet.ContainsKey(action))
                {
                    set.actionSet.Add(action, true);
                }
            }
            return set;
        }

        public static ActionSet InvertSet(ActionSet all, ActionSet exclusion)
        {
            ActionSet set = new ActionSet();
            foreach (AGE.Action action in all.actionSet.Keys)
            {
                if (!exclusion.actionSet.ContainsKey(action))
                {
                    set.actionSet.Add(action, true);
                }
            }
            return set;
        }

        public static ActionSet OrSet(ActionSet src1, ActionSet src2)
        {
            ActionSet set = new ActionSet();
            foreach (AGE.Action action in src1.actionSet.Keys)
            {
                set.actionSet.Add(action, true);
            }
            foreach (AGE.Action action2 in src2.actionSet.Keys)
            {
                if (!set.actionSet.ContainsKey(action2))
                {
                    set.actionSet.Add(action2, true);
                }
            }
            return set;
        }
    }
}

