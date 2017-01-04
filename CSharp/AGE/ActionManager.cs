namespace AGE
{
    using Assets.Scripts.Common;
    using Mono.Xml;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using UnityEngine;

    public class ActionManager : MonoSingleton<ActionManager>
    {
        public DictionaryView<string, ActionCommonData> actionCommonDataSet = new DictionaryView<string, ActionCommonData>();
        public ListView<AGE.Action> actionList = new ListView<AGE.Action>();
        public DictionaryView<string, AGE.Action> actionResourceSet = new DictionaryView<string, AGE.Action>();
        private ListView<AGE.Action> actionUpdatingList = new ListView<AGE.Action>();
        public string assetLoaderClass = string.Empty;
        private ListView<AGE.Action> conflictActionsToStop = new ListView<AGE.Action>();
        public bool debugMode;
        public bool frameMode;
        public bool isPrintLog;
        public DictionaryView<GameObject, ListView<AGE.Action>> objectReferenceSet = new DictionaryView<GameObject, ListView<AGE.Action>>();
        public bool preloadActionHelpers;
        public string[] preloadActions = new string[0];
        public bool preloadResources;
        private AssetLoader resLoader;
        private List<PoolObjHandle<AGE.Action>> waitReleaseList = new List<PoolObjHandle<AGE.Action>>();

        public void DeferReleaseAction(AGE.Action _action)
        {
            MonoSingleton<ActionManager>.instance.waitReleaseList.Add(new PoolObjHandle<AGE.Action>(_action));
        }

        public static void DestroyGameObject(UnityEngine.Object obj)
        {
            if ((Instance != null) && (Instance.ResLoader != null))
            {
                Instance.ResLoader.DestroyObject(obj);
            }
            else if (obj is GameObject)
            {
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(obj as GameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        public static void DestroyGameObjectFromAction(AGE.Action action, GameObject obj)
        {
            DestroyGameObject(obj);
            action.ClearGameObject(obj);
        }

        public void DestroyObject(GameObject _gameObject)
        {
            ListView<AGE.Action> view = null;
            if (this.objectReferenceSet.TryGetValue(_gameObject, out view) && (view != null))
            {
                for (int i = 0; i < view.Count; i++)
                {
                    view[i].ClearGameObject(_gameObject);
                }
            }
            DestroyGameObject(_gameObject);
        }

        public ActionSet FilterActionsByGameObject(GameObject obj, string nameInAction)
        {
            ActionSet set = new ActionSet();
            for (int i = 0; i < this.actionList.Count; i++)
            {
                AGE.Action action = this.actionList[i];
                int num2 = 0;
                action.templateObjectIds = this.LoadTemplateObjectList(action);
                if (action.TemplateObjectIds.TryGetValue(nameInAction, out num2) && (action.GetGameObject(num2) == obj))
                {
                    set.actionSet.Add(action, true);
                }
            }
            return set;
        }

        public void ForceReloadAction(string _actionName)
        {
        }

        public void ForceStart()
        {
            this.Initialize();
        }

        public void ForceStop()
        {
            if (MonoSingleton<ActionManager>.instance != null)
            {
                this.StopAllActions();
                this.actionList.Clear();
                this.actionResourceSet.Clear();
                this.objectReferenceSet.Clear();
                IEnumerator enumerator = base.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        DestroyGameObject(current.gameObject);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        }

        public bool GetActionTemplateObjectsAndPredefRefParams(string actionName, ref List<TemplateObject> objs, ref List<string> refnames)
        {
            ActionCommonData data;
            if (actionName == null)
            {
                return false;
            }
            if (this.actionCommonDataSet.ContainsKey(actionName))
            {
                data = this.actionCommonDataSet[actionName];
            }
            else
            {
                CBinaryObject obj2 = Instance.resLoader.LoadAge(actionName) as CBinaryObject;
                if (obj2 == null)
                {
                    return false;
                }
                data = new ActionCommonData();
                Mono.Xml.SecurityParser parser = new Mono.Xml.SecurityParser();
                parser.LoadXml(Encoding.UTF8.GetString(obj2.m_data));
                Singleton<CResourceManager>.GetInstance().RemoveCachedResource(actionName);
                SecurityElement element = parser.SelectSingleNode("Project");
                SecurityElement element2 = (element == null) ? null : element.SearchForChildByTag("TemplateObjectList");
                if (element2 != null)
                {
                    IEnumerator enumerator = element2.Children.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            SecurityElement current = (SecurityElement) enumerator.Current;
                            if (current.Tag == "TemplateObject")
                            {
                                TemplateObject item = new TemplateObject {
                                    name = current.Attribute("objectName"),
                                    id = int.Parse(current.Attribute("id")),
                                    isTemp = bool.Parse(current.Attribute("isTemp"))
                                };
                                data.templateObjects.Add(item);
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable = enumerator as IDisposable;
                        if (disposable == null)
                        {
                        }
                        disposable.Dispose();
                    }
                }
                SecurityElement element4 = (element == null) ? null : element.SearchForChildByTag("RefParamList");
                if (element4 != null)
                {
                    IEnumerator enumerator2 = element4.Children.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            string str = ((SecurityElement) enumerator2.Current).Attribute("name");
                            if (str.StartsWith("_"))
                            {
                                data.predefRefParamNames.Add(str);
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable2 = enumerator2 as IDisposable;
                        if (disposable2 == null)
                        {
                        }
                        disposable2.Dispose();
                    }
                }
                this.actionCommonDataSet.Add(actionName, data);
            }
            if (data != null)
            {
                objs.Clear();
                refnames.Clear();
                for (int i = 0; i < data.templateObjects.Count; i++)
                {
                    objs.Add(data.templateObjects[i]);
                }
                for (int j = 0; j < data.predefRefParamNames.Count; j++)
                {
                    refnames.Add(data.predefRefParamNames[j]);
                }
            }
            return true;
        }

        protected override void Init()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            IEnumerator enumerator = base.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    DestroyGameObject(current.gameObject);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (this.resLoader == null)
            {
                System.Type type = Utility.GetType(this.assetLoaderClass);
                if (type != null)
                {
                    this.resLoader = Activator.CreateInstance(type) as AssetLoader;
                }
                if (this.resLoader == null)
                {
                    this.resLoader = new AssetLoader();
                }
            }
            this.resLoader.preloading = true;
            foreach (string str in this.preloadActions)
            {
                this.LoadActionResource(str);
            }
            if (this.preloadActionHelpers)
            {
                foreach (ActionHelper helper in UnityEngine.Object.FindObjectsOfType<ActionHelper>())
                {
                    foreach (ActionHelperStorage storage in helper.actionHelpers)
                    {
                        this.LoadActionResource(storage.actionName);
                    }
                }
            }
            if (this.preloadResources)
            {
            }
            this.resLoader.preloading = false;
        }

        public static UnityEngine.Object InstantiateObject(UnityEngine.Object prefab)
        {
            if ((Instance != null) && (Instance.ResLoader != null))
            {
                return Instance.ResLoader.Instantiate(prefab);
            }
            return UnityEngine.Object.Instantiate(prefab);
        }

        public static UnityEngine.Object InstantiateObject(UnityEngine.Object prefab, Vector3 pos, Quaternion rot)
        {
            if ((Instance != null) && (Instance.ResLoader != null))
            {
                return Instance.ResLoader.Instantiate(prefab, pos, rot);
            }
            return UnityEngine.Object.Instantiate(prefab, pos, rot);
        }

        private AGE.Action InternalPlayAction(string _actionName, bool _autoPlay, bool _stopConflictAction, GameObject[] _gameObjects)
        {
            GameObject obj2 = null;
            for (int i = 0; i < _gameObjects.Length; i++)
            {
                GameObject key = _gameObjects[i];
                if ((key != null) && this.objectReferenceSet.ContainsKey(key))
                {
                    obj2 = key;
                    break;
                }
            }
            if ((obj2 != null) && _stopConflictAction)
            {
                this.conflictActionsToStop.Clear();
                ListView<AGE.Action>.Enumerator enumerator = this.objectReferenceSet[obj2].GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AGE.Action current = enumerator.Current;
                    if (!current.unstoppable)
                    {
                        this.conflictActionsToStop.Add(current);
                    }
                }
                ListView<AGE.Action>.Enumerator enumerator2 = this.conflictActionsToStop.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.Stop(false);
                }
            }
            AGE.Action action3 = this.LoadActionResource(_actionName);
            if (action3 == null)
            {
                Debug.LogError("Playing \"" + _actionName + "\" failed. Asset not found!");
                return null;
            }
            AGE.Action item = ClassObjPool<AGE.Action>.Get();
            item.enabled = _autoPlay;
            item.refGameObjectsCount = _gameObjects.Length;
            item.LoadAction(action3, _gameObjects);
            this.actionList.Add(item);
            for (int j = 0; j < _gameObjects.Length; j++)
            {
                GameObject obj4 = _gameObjects[j];
                if (obj4 != null)
                {
                    ListView<AGE.Action> view2 = null;
                    if (this.objectReferenceSet.TryGetValue(obj4, out view2))
                    {
                        view2.Add(item);
                    }
                    else
                    {
                        view2 = new ListView<AGE.Action>();
                        view2.Add(item);
                        this.objectReferenceSet.Add(obj4, view2);
                    }
                }
            }
            return item;
        }

        private AGE.Action InternalPlaySubAction(AGE.Action _parentAction, string _actionName, float _length, GameObject[] _gameObjects)
        {
            AGE.Action action = this.LoadActionResource(_actionName);
            if (action == null)
            {
                Debug.LogError("Playing \"" + _actionName + "\" failed. Asset not found!");
                return null;
            }
            AGE.Action item = ClassObjPool<AGE.Action>.Get();
            item.LoadAction(action, _gameObjects);
            item.loop = false;
            item.length = ActionUtility.SecToMs(_length);
            item.parentAction = _parentAction;
            this.actionList.Add(item);
            return item;
        }

        public bool IsActionValid(AGE.Action _action)
        {
            if (_action == null)
            {
                return false;
            }
            return this.actionList.Contains(_action);
        }

        public AGE.Action LoadActionResource(string _actionName)
        {
            AGE.Action action = null;
            if (_actionName == null)
            {
                DebugHelper.Assert(_actionName != null, "can't load action with name = null");
                return null;
            }
            if (this.actionResourceSet.TryGetValue(_actionName, out action))
            {
                if (action != null)
                {
                    return action;
                }
                this.actionResourceSet.Remove(_actionName);
            }
            CBinaryObject obj2 = Instance.resLoader.LoadAge(_actionName) as CBinaryObject;
            if (obj2 == null)
            {
                return null;
            }
            Mono.Xml.SecurityParser parser = new Mono.Xml.SecurityParser();
            try
            {
                parser.LoadXml(Encoding.UTF8.GetString(obj2.m_data));
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { _actionName, exception.Message };
                DebugHelper.Assert(false, "Load xml Exception for action name = {0}, exception = {1}", inParameters);
                return null;
            }
            action = new AGE.Action {
                name = _actionName,
                enabled = false,
                actionName = _actionName
            };
            this.actionResourceSet.Add(_actionName, action);
            Singleton<CResourceManager>.GetInstance().RemoveCachedResource(_actionName);
            SecurityElement element = parser.SelectSingleNode("Project");
            SecurityElement element2 = (element == null) ? null : element.SearchForChildByTag("TemplateObjectList");
            SecurityElement element3 = (element == null) ? null : element.SearchForChildByTag("Action");
            SecurityElement element4 = (element == null) ? null : element.SearchForChildByTag("RefParamList");
            DebugHelper.Assert(element3 != null, "actionNode!=null");
            if (element3 != null)
            {
                action.length = ActionUtility.SecToMs(float.Parse(element3.Attribute("length")));
                action.loop = bool.Parse(element3.Attribute("loop"));
            }
            if ((element2 != null) && (element2.Children != null))
            {
                for (int i = 0; i < element2.Children.Count; i++)
                {
                    SecurityElement element5 = element2.Children[i] as SecurityElement;
                    string str = element5.Attribute("objectName");
                    int id = int.Parse(element5.Attribute("id"));
                    action.AddTemplateObject(str, id);
                }
            }
            if ((element4 != null) && (element4.Children != null))
            {
                for (int j = 0; j < element4.Children.Count; j++)
                {
                    this.LoadRefParamNode(action, element4.Children[j] as SecurityElement);
                }
            }
            if ((element3 != null) && (element3.Children != null))
            {
                for (int k = 0; k < element3.Children.Count; k++)
                {
                    SecurityElement element6 = element3.Children[k] as SecurityElement;
                    string typeName = element6.Attribute("eventType");
                    if (!typeName.Contains(".") && (typeName.Length > 0))
                    {
                        typeName = "AGE." + typeName;
                    }
                    System.Type type = Utility.GetType(typeName);
                    if (type != null)
                    {
                        string name = string.Empty;
                        bool flag = false;
                        if (element6.Attribute("refParamName") != null)
                        {
                            name = element6.Attribute("refParamName");
                        }
                        if (element6.Attribute("useRefParam") != null)
                        {
                            flag = bool.Parse(element6.Attribute("useRefParam"));
                        }
                        bool flag2 = bool.Parse(element6.Attribute("enabled"));
                        if (flag)
                        {
                            action.refParams.GetRefParam(name, ref flag2);
                        }
                        Track data = action.AddTrack(type);
                        data.enabled = flag2;
                        data.trackName = element6.Attribute("trackName");
                        if (element6.Attribute("execOnActionCompleted") != null)
                        {
                            data.execOnActionCompleted = bool.Parse(element6.Attribute("execOnActionCompleted"));
                        }
                        if (element6.Attribute("execOnForceStopped") != null)
                        {
                            data.execOnForceStopped = bool.Parse(element6.Attribute("execOnForceStopped"));
                        }
                        if (flag)
                        {
                            FieldInfo field = type.GetField(element6.Attribute("enabled"));
                            action.refParams.AddRefData(name, field, data);
                        }
                        if (element6.Attribute("r") != null)
                        {
                            data.color.r = float.Parse(element6.Attribute("r"));
                        }
                        if (element6.Attribute("g") != null)
                        {
                            data.color.g = float.Parse(element6.Attribute("g"));
                        }
                        if (element6.Attribute("b") != null)
                        {
                            data.color.b = float.Parse(element6.Attribute("b"));
                        }
                        ListView<SecurityElement> view = new ListView<SecurityElement>();
                        if (element6.Children != null)
                        {
                            for (int m = 0; m < element6.Children.Count; m++)
                            {
                                SecurityElement item = element6.Children[m] as SecurityElement;
                                if ((item.Tag != "Event") && (item.Tag != "Condition"))
                                {
                                    view.Add(item);
                                }
                            }
                            for (int n = 0; n < element6.Children.Count; n++)
                            {
                                SecurityElement element8 = element6.Children[n] as SecurityElement;
                                if (element8.Tag == "Condition")
                                {
                                    SecurityElement element9 = element8;
                                    int key = int.Parse(element9.Attribute("id"));
                                    bool flag3 = bool.Parse(element9.Attribute("status"));
                                    if (data.waitForConditions == null)
                                    {
                                        data.waitForConditions = new Dictionary<int, bool>();
                                    }
                                    data.waitForConditions.Add(key, flag3);
                                }
                                else if (element8.Tag == "Event")
                                {
                                    int num8 = ActionUtility.SecToMs(float.Parse(element8.Attribute("time")));
                                    int num9 = 0;
                                    if (data.IsDurationEvent)
                                    {
                                        num9 = ActionUtility.SecToMs(float.Parse(element8.Attribute("length")));
                                    }
                                    BaseEvent event2 = data.AddEvent(num8, num9);
                                    for (int num10 = 0; num10 < view.Count; num10++)
                                    {
                                        this.SetEventField(action, event2, view[num10]);
                                    }
                                    if (element8.Children != null)
                                    {
                                        for (int num11 = 0; num11 < element8.Children.Count; num11++)
                                        {
                                            this.SetEventField(action, event2, element8.Children[num11] as SecurityElement);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Invalid event type \"" + element6.Attribute("eventType") + "\"!");
                    }
                }
            }
            return action;
        }

        private void LoadRefParamNode(AGE.Action result, SecurityElement paramNode)
        {
            string str = paramNode.Tag.ToLower();
            string name = paramNode.Attribute("name");
            switch (str)
            {
                case "float":
                {
                    float num = float.Parse(paramNode.Attribute("value"));
                    result.refParams.AddRefParam(name, num);
                    break;
                }
                case "int":
                {
                    int num2 = int.Parse(paramNode.Attribute("value"));
                    result.refParams.AddRefParam(name, num2);
                    break;
                }
                case "templateobject":
                {
                    int num3 = int.Parse(paramNode.Attribute("id"));
                    result.refParams.AddRefParam(name, num3);
                    break;
                }
                case "uint":
                {
                    uint num4 = uint.Parse(paramNode.Attribute("value"));
                    result.refParams.AddRefParam(name, num4);
                    break;
                }
                case "bool":
                {
                    bool flag = bool.Parse(paramNode.Attribute("value"));
                    result.refParams.AddRefParam(name, flag);
                    break;
                }
                case "string":
                {
                    string str3 = paramNode.Attribute("value");
                    result.refParams.AddRefParam(name, str3);
                    break;
                }
                case "vector3":
                {
                    float x = float.Parse(paramNode.Attribute("x"));
                    float y = float.Parse(paramNode.Attribute("y"));
                    float z = float.Parse(paramNode.Attribute("z"));
                    Vector3 vector = new Vector3(x, y, z);
                    result.refParams.AddRefParam(name, vector);
                    break;
                }
                case "vector3i":
                {
                    int num8 = int.Parse(paramNode.Attribute("x"));
                    int num9 = int.Parse(paramNode.Attribute("y"));
                    int num10 = int.Parse(paramNode.Attribute("z"));
                    VInt3 num11 = new VInt3(num8, num9, num10);
                    result.refParams.AddRefParam(name, num11);
                    break;
                }
                case "quaternion":
                {
                    float num12 = float.Parse(paramNode.Attribute("x"));
                    float num13 = float.Parse(paramNode.Attribute("y"));
                    float num14 = float.Parse(paramNode.Attribute("z"));
                    float w = float.Parse(paramNode.Attribute("w"));
                    Quaternion quaternion = new Quaternion(num12, num13, num14, w);
                    result.refParams.AddRefParam(name, quaternion);
                    break;
                }
                case "eulerangle":
                {
                    float num16 = float.Parse(paramNode.Attribute("x"));
                    float num17 = float.Parse(paramNode.Attribute("y"));
                    float num18 = float.Parse(paramNode.Attribute("z"));
                    Quaternion quaternion2 = Quaternion.Euler(num16, num17, num18);
                    result.refParams.AddRefParam(name, quaternion2);
                    break;
                }
                case "enum":
                {
                    int num19 = int.Parse(paramNode.Attribute("value"));
                    result.refParams.AddRefParam(name, num19);
                    break;
                }
            }
        }

        public Dictionary<string, int> LoadTemplateObjectList(AGE.Action action)
        {
            string actionName = action.actionName;
            CBinaryObject obj2 = Instance.resLoader.LoadAge(actionName) as CBinaryObject;
            if (obj2 == null)
            {
                return new Dictionary<string, int>();
            }
            Mono.Xml.SecurityParser parser = new Mono.Xml.SecurityParser();
            parser.LoadXml(Encoding.UTF8.GetString(obj2.m_data));
            Singleton<CResourceManager>.GetInstance().RemoveCachedResource(actionName);
            SecurityElement element = parser.SelectSingleNode("Project");
            SecurityElement element2 = (element == null) ? null : element.SearchForChildByTag("TemplateObjectList");
            if (element2 != null)
            {
                action.templateObjectIds.Clear();
                if (element2.Children != null)
                {
                    for (int i = 0; i < element2.Children.Count; i++)
                    {
                        SecurityElement element3 = element2.Children[i] as SecurityElement;
                        string key = element3.Attribute("objectName");
                        int num2 = int.Parse(element3.Attribute("id"));
                        if (element3.Attribute("isTemp") == "false")
                        {
                            action.templateObjectIds.Add(key, num2);
                        }
                    }
                }
            }
            return action.templateObjectIds;
        }

        public AGE.Action PlayAction(string _actionName, bool _autoPlay, bool _stopConflictAction, params GameObject[] _gameObjects)
        {
            return this.InternalPlayAction(_actionName, _autoPlay, _stopConflictAction, _gameObjects);
        }

        public AGE.Action PlaySubAction(AGE.Action _parentAction, string _actionName, float _length, params GameObject[] _gameObjects)
        {
            return this.InternalPlaySubAction(_parentAction, _actionName, _length, _gameObjects);
        }

        public void RemoveAction(AGE.Action _action)
        {
            if (_action != null)
            {
                ListLinqView<GameObject> gameObjectList = _action.GetGameObjectList();
                int count = gameObjectList.Count;
                for (int i = 0; i < count; i++)
                {
                    GameObject key = gameObjectList[i];
                    if (key != null)
                    {
                        if (this.objectReferenceSet.ContainsKey(key))
                        {
                            ListView<AGE.Action> view2 = this.objectReferenceSet[key];
                            view2.Remove(_action);
                            if (view2.Count == 0)
                            {
                                this.objectReferenceSet.Remove(key);
                            }
                        }
                        else if (gameObjectList.IndexOf(key) >= _action.refGameObjectsCount)
                        {
                            DestroyGameObject(key);
                        }
                    }
                }
                MonoSingleton<ActionManager>.instance.actionList.Remove(_action);
                _action.Release();
            }
        }

        private void SetEventField(AGE.Action action, BaseEvent _trackEvent, SecurityElement _fieldNode)
        {
            string str = _fieldNode.Tag.ToLower();
            FieldInfo field = _trackEvent.GetType().GetField(_fieldNode.Attribute("name"));
            if (field != null)
            {
                bool flag = false;
                string name = string.Empty;
                if (_fieldNode.Attribute("useRefParam") != null)
                {
                    flag = bool.Parse(_fieldNode.Attribute("useRefParam"));
                }
                if (flag && (_fieldNode.Attribute("refParamName") != null))
                {
                    name = _fieldNode.Attribute("refParamName");
                }
                switch (str)
                {
                    case "float":
                    {
                        float num = float.Parse(_fieldNode.Attribute("value"));
                        field.SetValue(_trackEvent, num);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref num))
                            {
                                field.SetValue(_trackEvent, num);
                            }
                        }
                        break;
                    }
                    case "int":
                    {
                        int num2 = int.Parse(_fieldNode.Attribute("value"));
                        field.SetValue(_trackEvent, num2);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref num2))
                            {
                                field.SetValue(_trackEvent, num2);
                            }
                        }
                        break;
                    }
                    case "templateobject":
                    case "trackobject":
                    {
                        int num3 = int.Parse(_fieldNode.Attribute("id"));
                        field.SetValue(_trackEvent, num3);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref num3))
                            {
                                field.SetValue(_trackEvent, num3);
                            }
                        }
                        break;
                    }
                    case "uint":
                    {
                        uint num4 = uint.Parse(_fieldNode.Attribute("value"));
                        field.SetValue(_trackEvent, num4);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref num4))
                            {
                                field.SetValue(_trackEvent, num4);
                            }
                        }
                        break;
                    }
                    case "bool":
                    {
                        bool flag2 = bool.Parse(_fieldNode.Attribute("value"));
                        field.SetValue(_trackEvent, flag2);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref flag2))
                            {
                                field.SetValue(_trackEvent, flag2);
                            }
                        }
                        break;
                    }
                    case "string":
                    {
                        string str3 = _fieldNode.Attribute("value");
                        field.SetValue(_trackEvent, str3);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            object obj2 = null;
                            if (action.refParams.GetRefParam(name, ref obj2))
                            {
                                field.SetValue(_trackEvent, str3);
                            }
                        }
                        break;
                    }
                    case "vector3":
                    {
                        float x = float.Parse(_fieldNode.Attribute("x"));
                        float y = float.Parse(_fieldNode.Attribute("y"));
                        float z = float.Parse(_fieldNode.Attribute("z"));
                        Vector3 vector = new Vector3(x, y, z);
                        field.SetValue(_trackEvent, vector);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref vector))
                            {
                                field.SetValue(_trackEvent, vector);
                            }
                        }
                        break;
                    }
                    case "vector3i":
                    {
                        int num8 = int.Parse(_fieldNode.Attribute("x"));
                        int num9 = int.Parse(_fieldNode.Attribute("y"));
                        int num10 = int.Parse(_fieldNode.Attribute("z"));
                        VInt3 num11 = new VInt3(num8, num9, num10);
                        field.SetValue(_trackEvent, num11);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref num11))
                            {
                                field.SetValue(_trackEvent, num11);
                            }
                        }
                        break;
                    }
                    case "quaternion":
                    {
                        float num12 = float.Parse(_fieldNode.Attribute("x"));
                        float num13 = float.Parse(_fieldNode.Attribute("y"));
                        float num14 = float.Parse(_fieldNode.Attribute("z"));
                        float w = float.Parse(_fieldNode.Attribute("w"));
                        Quaternion quaternion = new Quaternion(num12, num13, num14, w);
                        field.SetValue(_trackEvent, quaternion);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref quaternion))
                            {
                                field.SetValue(_trackEvent, quaternion);
                            }
                        }
                        break;
                    }
                    case "eulerangle":
                    {
                        float num16 = float.Parse(_fieldNode.Attribute("x"));
                        float num17 = float.Parse(_fieldNode.Attribute("y"));
                        float num18 = float.Parse(_fieldNode.Attribute("z"));
                        Quaternion quaternion2 = Quaternion.Euler(num16, num17, num18);
                        field.SetValue(_trackEvent, quaternion2);
                        if (flag)
                        {
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref quaternion2))
                            {
                                field.SetValue(_trackEvent, quaternion2);
                            }
                        }
                        break;
                    }
                    case "enum":
                        field.SetValue(_trackEvent, Enum.Parse(field.FieldType, _fieldNode.Attribute("value")));
                        if (flag)
                        {
                            int num19 = 0;
                            action.refParams.AddRefData(name, field, _trackEvent);
                            if (action.refParams.GetRefParam(name, ref num19))
                            {
                                field.SetValue(_trackEvent, num19);
                            }
                        }
                        break;

                    default:
                        if (!(str == "array"))
                        {
                            Debug.LogError("Invalid field type \"" + str + "\"!");
                            break;
                        }
                        if (_fieldNode.Children != null)
                        {
                            string str4 = _fieldNode.Attribute("type").ToLower();
                            switch (str4)
                            {
                                case "vector3i":
                                {
                                    VInt3[] numArray = new VInt3[_fieldNode.Children.Count];
                                    int num20 = 0;
                                    IEnumerator enumerator = _fieldNode.Children.GetEnumerator();
                                    try
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            SecurityElement current = (SecurityElement) enumerator.Current;
                                            int num21 = int.Parse(current.Attribute("x"));
                                            int num22 = int.Parse(current.Attribute("y"));
                                            int num23 = int.Parse(current.Attribute("z"));
                                            VInt3 num24 = new VInt3(num21, num22, num23);
                                            numArray[num20++] = num24;
                                        }
                                    }
                                    finally
                                    {
                                        IDisposable disposable = enumerator as IDisposable;
                                        if (disposable == null)
                                        {
                                        }
                                        disposable.Dispose();
                                    }
                                    field.SetValue(_trackEvent, numArray);
                                    return;
                                }
                                case "int":
                                {
                                    int[] numArray2 = new int[_fieldNode.Children.Count];
                                    int num25 = 0;
                                    IEnumerator enumerator2 = _fieldNode.Children.GetEnumerator();
                                    try
                                    {
                                        while (enumerator2.MoveNext())
                                        {
                                            SecurityElement element2 = (SecurityElement) enumerator2.Current;
                                            int num26 = int.Parse(element2.Attribute("value"));
                                            numArray2[num25++] = num26;
                                        }
                                    }
                                    finally
                                    {
                                        IDisposable disposable2 = enumerator2 as IDisposable;
                                        if (disposable2 == null)
                                        {
                                        }
                                        disposable2.Dispose();
                                    }
                                    field.SetValue(_trackEvent, numArray2);
                                    return;
                                }
                                case "templateobject":
                                case "trackobject":
                                {
                                    int[] numArray3 = new int[_fieldNode.Children.Count];
                                    int num27 = 0;
                                    IEnumerator enumerator3 = _fieldNode.Children.GetEnumerator();
                                    try
                                    {
                                        while (enumerator3.MoveNext())
                                        {
                                            SecurityElement element3 = (SecurityElement) enumerator3.Current;
                                            int num28 = int.Parse(element3.Attribute("id"));
                                            numArray3[num27++] = num28;
                                        }
                                    }
                                    finally
                                    {
                                        IDisposable disposable3 = enumerator3 as IDisposable;
                                        if (disposable3 == null)
                                        {
                                        }
                                        disposable3.Dispose();
                                    }
                                    field.SetValue(_trackEvent, numArray3);
                                    return;
                                }
                                case "uint":
                                {
                                    uint[] numArray4 = new uint[_fieldNode.Children.Count];
                                    int num29 = 0;
                                    IEnumerator enumerator4 = _fieldNode.Children.GetEnumerator();
                                    try
                                    {
                                        while (enumerator4.MoveNext())
                                        {
                                            SecurityElement element4 = (SecurityElement) enumerator4.Current;
                                            uint num30 = uint.Parse(element4.Attribute("value"));
                                            numArray4[num29++] = num30;
                                        }
                                    }
                                    finally
                                    {
                                        IDisposable disposable4 = enumerator4 as IDisposable;
                                        if (disposable4 == null)
                                        {
                                        }
                                        disposable4.Dispose();
                                    }
                                    field.SetValue(_trackEvent, numArray4);
                                    return;
                                }
                                case "bool":
                                {
                                    bool[] flagArray = new bool[_fieldNode.Children.Count];
                                    int num31 = 0;
                                    IEnumerator enumerator5 = _fieldNode.Children.GetEnumerator();
                                    try
                                    {
                                        while (enumerator5.MoveNext())
                                        {
                                            SecurityElement element5 = (SecurityElement) enumerator5.Current;
                                            bool flag3 = bool.Parse(element5.Attribute("value"));
                                            flagArray[num31++] = flag3;
                                        }
                                    }
                                    finally
                                    {
                                        IDisposable disposable5 = enumerator5 as IDisposable;
                                        if (disposable5 == null)
                                        {
                                        }
                                        disposable5.Dispose();
                                    }
                                    field.SetValue(_trackEvent, flagArray);
                                    return;
                                }
                                case "float":
                                {
                                    float[] numArray5 = new float[_fieldNode.Children.Count];
                                    int num32 = 0;
                                    IEnumerator enumerator6 = _fieldNode.Children.GetEnumerator();
                                    try
                                    {
                                        while (enumerator6.MoveNext())
                                        {
                                            SecurityElement element6 = (SecurityElement) enumerator6.Current;
                                            float num33 = float.Parse(element6.Attribute("value"));
                                            numArray5[num32++] = num33;
                                        }
                                    }
                                    finally
                                    {
                                        IDisposable disposable6 = enumerator6 as IDisposable;
                                        if (disposable6 == null)
                                        {
                                        }
                                        disposable6.Dispose();
                                    }
                                    field.SetValue(_trackEvent, numArray5);
                                    return;
                                }
                            }
                            if (!(str4 == "string"))
                            {
                                break;
                            }
                            string[] strArray = new string[_fieldNode.Children.Count];
                            int num34 = 0;
                            IEnumerator enumerator7 = _fieldNode.Children.GetEnumerator();
                            try
                            {
                                while (enumerator7.MoveNext())
                                {
                                    string str10 = ((SecurityElement) enumerator7.Current).Attribute("value");
                                    strArray[num34++] = str10;
                                }
                            }
                            finally
                            {
                                IDisposable disposable7 = enumerator7 as IDisposable;
                                if (disposable7 == null)
                                {
                                }
                                disposable7.Dispose();
                            }
                            field.SetValue(_trackEvent, strArray);
                        }
                        break;
                }
            }
        }

        public void SetResLoader(AssetLoader loader)
        {
            this.resLoader = loader;
        }

        public void StopAction(AGE.Action _action)
        {
            if (_action != null)
            {
                _action.Stop(true);
            }
        }

        public void StopAllActions()
        {
            if (this.actionList.Count != 0)
            {
                ListView<AGE.Action> view = new ListView<AGE.Action>(this.actionList);
                for (int i = 0; i < view.Count; i++)
                {
                    view[i].Stop(true);
                }
            }
        }

        private void Update()
        {
        }

        public void UpdateLogic(int nDelta)
        {
            if (this.frameMode)
            {
                for (int i = 0; i < this.waitReleaseList.Count; i++)
                {
                    if (this.waitReleaseList[i] != 0)
                    {
                        PoolObjHandle<AGE.Action> handle = this.waitReleaseList[i];
                        handle.handle.Stop(true);
                    }
                }
                this.waitReleaseList.Clear();
                this.actionUpdatingList.Clear();
                for (int j = 0; j < this.actionList.Count; j++)
                {
                    this.actionUpdatingList.Add(this.actionList[j]);
                }
                for (int k = 0; k < this.actionUpdatingList.Count; k++)
                {
                    if (!this.actionUpdatingList[k].nextDestroy)
                    {
                        this.actionUpdatingList[k].UpdateLogic(nDelta);
                    }
                }
                for (int m = 0; m < this.waitReleaseList.Count; m++)
                {
                    if (this.waitReleaseList[m] != 0)
                    {
                        PoolObjHandle<AGE.Action> handle2 = this.waitReleaseList[m];
                        handle2.handle.Stop(true);
                    }
                }
                this.waitReleaseList.Clear();
            }
        }

        public int ActionUpdatingCount
        {
            get
            {
                return this.actionUpdatingList.Count;
            }
        }

        public static ActionManager Instance
        {
            get
            {
                return MonoSingleton<ActionManager>.instance;
            }
        }

        public AssetLoader ResLoader
        {
            get
            {
                return this.resLoader;
            }
        }
    }
}

