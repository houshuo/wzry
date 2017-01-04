namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class ProgressWidget : ActivityWidget
    {
        private Image _back;
        private Cursor[] _cursorArray;
        private Cursor _cursorTemplate;
        private Image _fore;
        private Text _tips;

        public ProgressWidget(GameObject node, ActivityView view) : base(node, view)
        {
            this._tips = Utility.GetComponetInChild<Text>(node, "Tips");
            this._back = Utility.GetComponetInChild<Image>(node, "Bar");
            this._fore = Utility.GetComponetInChild<Image>(node, "Bar/Fore");
            this._cursorTemplate = new Cursor(Utility.FindChild(node, "Bar/Cursor"));
            ListView<ActivityPhase> phaseList = view.activity.PhaseList;
            if (phaseList.Count > 0)
            {
                this._cursorArray = new Cursor[phaseList.Count];
                this._cursorTemplate.root.SetActive(true);
                this._cursorArray[0] = this._cursorTemplate;
                for (int i = 1; i < phaseList.Count; i++)
                {
                    Cursor cursor = new Cursor((GameObject) UnityEngine.Object.Instantiate(this._cursorTemplate.root));
                    cursor.root.transform.SetParent(this._cursorTemplate.root.transform.parent);
                    this._cursorArray[i] = cursor;
                }
            }
            else
            {
                this._cursorTemplate.root.SetActive(false);
            }
            view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
            view.activity.OnMaskStateChange += new Activity.ActivityEvent(this.OnStateChange);
            this.Validate();
        }

        public override void Clear()
        {
            base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            base.view.activity.OnMaskStateChange -= new Activity.ActivityEvent(this.OnStateChange);
            if (this._cursorArray != null)
            {
                for (int i = 1; i < this._cursorArray.Length; i++)
                {
                    UnityEngine.Object.Destroy(this._cursorArray[i].root);
                }
                this._cursorArray = null;
            }
        }

        private void OnStateChange(Activity actv)
        {
            this.Validate();
        }

        public override void Validate()
        {
            ListView<ActivityPhase> phaseList = base.view.activity.PhaseList;
            if (phaseList.Count > 0)
            {
                this._tips.text = base.view.activity.Tips;
                int target = base.view.activity.Target;
                int current = base.view.activity.Current;
                if (current > target)
                {
                    current = target;
                }
                float width = this._back.GetComponent<RectTransform>().rect.width;
                float num4 = ((float) current) / ((float) target);
                RectTransform component = this._fore.GetComponent<RectTransform>();
                component.sizeDelta = new Vector2(num4 * width, component.rect.height);
                for (int i = 0; i < phaseList.Count; i++)
                {
                    ActivityPhase phase = phaseList[i];
                    float num6 = ((float) phase.Target) / ((float) target);
                    Cursor cursor = this._cursorArray[i];
                    cursor.root.transform.localPosition = new Vector3(num6 * width, 0f, 0f);
                    cursor.root.transform.localScale = Vector3.one;
                    cursor.root.transform.localRotation = Quaternion.identity;
                    bool flag = current >= phase.Target;
                    cursor.valTxt.text = phase.Target.ToString();
                    cursor.valTxt.color = !flag ? Color.gray : Color.white;
                    cursor.arrow.color = !flag ? Color.gray : Color.white;
                }
            }
        }

        public class Cursor
        {
            public Image arrow;
            public GameObject root;
            public Text valTxt;

            public Cursor(GameObject node)
            {
                this.root = node;
                this.valTxt = Utility.GetComponetInChild<Text>(node, "Value");
                this.arrow = Utility.GetComponetInChild<Image>(node, "Arrow");
            }
        }
    }
}

