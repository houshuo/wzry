namespace AGE
{
    using System;
    using UnityEngine;

    public class ActionHelper : MonoBehaviour
    {
        private DictionaryView<string, ActionHelperStorage> actionHelperMap = new DictionaryView<string, ActionHelperStorage>();
        public ActionHelperStorage[] actionHelpers = new ActionHelperStorage[0];
        private Animator animator;

        public void BeginAction(string _actionHelperName)
        {
            ActionHelperStorage storage = null;
            if (this.actionHelperMap.TryGetValue(_actionHelperName, out storage) && storage.waitForEvents)
            {
                Animator component = base.gameObject.GetComponent<Animator>();
                if (component != null)
                {
                    if (storage.detectStatePath.Length <= 0)
                    {
                        storage.PlayAction();
                    }
                    else
                    {
                        bool flag2 = false;
                        for (int i = 0; i < component.layerCount; i++)
                        {
                            if (component.GetCurrentAnimatorStateInfo(i).nameHash == storage.GetDetectStatePathHash())
                            {
                                flag2 = true;
                                break;
                            }
                            AnimatorStateInfo nextAnimatorStateInfo = component.GetNextAnimatorStateInfo(i);
                            if (component.IsInTransition(i) && (nextAnimatorStateInfo.nameHash == storage.GetDetectStatePathHash()))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (flag2)
                        {
                            storage.PlayAction();
                        }
                    }
                }
            }
        }

        public void EndAction(string _actionHelperName)
        {
            ActionHelperStorage storage = null;
            if ((this.actionHelperMap.TryGetValue(_actionHelperName, out storage) && storage.waitForEvents) && (base.gameObject.GetComponent<Animator>() != null))
            {
                storage.StopLastAction();
            }
        }

        public void ForceStart()
        {
            this.actionHelperMap.Clear();
            foreach (ActionHelperStorage storage in this.actionHelpers)
            {
                this.actionHelperMap.Add(storage.helperName, storage);
            }
        }

        public ActionHelperStorage GetAction(int index)
        {
            if ((index >= 0) && (index <= this.actionHelpers.Length))
            {
                return this.actionHelpers[index];
            }
            return null;
        }

        public ActionHelperStorage GetAction(string _actionHelperName)
        {
            ActionHelperStorage storage = null;
            if (!this.actionHelperMap.TryGetValue(_actionHelperName, out storage))
            {
                return null;
            }
            return storage;
        }

        public AGE.Action PlayAction(int index)
        {
            if ((index < 0) || (index > this.actionHelpers.Length))
            {
                return null;
            }
            ActionHelperStorage storage = this.actionHelpers[index];
            if (storage == null)
            {
                return null;
            }
            storage.autoPlay = true;
            return storage.PlayAction();
        }

        public AGE.Action PlayAction(string _actionHelperName)
        {
            ActionHelperStorage storage = null;
            if (!this.actionHelperMap.TryGetValue(_actionHelperName, out storage))
            {
                return null;
            }
            storage.autoPlay = true;
            return storage.PlayAction();
        }

        public AGE.Action PlayAction(string _actionHelperName, DictionaryView<string, GameObject> dictionary)
        {
            ActionHelperStorage storage = null;
            if (!this.actionHelperMap.TryGetValue(_actionHelperName, out storage))
            {
                return null;
            }
            storage.autoPlay = true;
            return storage.PlayAction(dictionary);
        }

        public void Restart()
        {
            foreach (ActionHelperStorage storage in this.actionHelpers)
            {
                if (storage.autoPlay)
                {
                    ActionManager.Instance.PlayAction(storage.actionName, storage.autoPlay, storage.stopConflictActions, storage.targets);
                }
            }
        }

        private void Start()
        {
            this.animator = base.gameObject.GetComponent<Animator>();
            this.actionHelperMap.Clear();
            foreach (ActionHelperStorage storage in this.actionHelpers)
            {
                this.actionHelperMap.Add(storage.helperName, storage);
                storage.autoPlay = true;
                if (storage.playOnStart)
                {
                    storage.PlayAction();
                }
            }
        }

        private void Update()
        {
            if (this.animator != null)
            {
                foreach (ActionHelperStorage storage in this.actionHelpers)
                {
                    if (storage.detectStatePath.Length <= 0)
                    {
                        continue;
                    }
                    bool flag = false;
                    for (int i = 0; i < this.animator.layerCount; i++)
                    {
                        if (this.animator.GetCurrentAnimatorStateInfo(i).nameHash == storage.GetDetectStatePathHash())
                        {
                            flag = true;
                            break;
                        }
                        AnimatorStateInfo nextAnimatorStateInfo = this.animator.GetNextAnimatorStateInfo(i);
                        if (this.animator.IsInTransition(i) && (nextAnimatorStateInfo.nameHash == storage.GetDetectStatePathHash()))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        if (!storage.waitForEvents && !storage.IsLastActionActive())
                        {
                            storage.PlayAction();
                        }
                    }
                    else
                    {
                        storage.StopLastAction();
                    }
                }
            }
        }
    }
}

