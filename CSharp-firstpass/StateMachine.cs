using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

public class StateMachine
{
    private DictionaryView<string, IState> _registedState = new DictionaryView<string, IState>();
    private Stack<IState> _stateStack = new Stack<IState>();

    public IState ChangeState(IState state)
    {
        if (state == null)
        {
            return null;
        }
        this.tarState = state;
        IState state2 = null;
        if (this._stateStack.Count > 0)
        {
            state2 = this._stateStack.Pop();
            state2.OnStateLeave();
        }
        this._stateStack.Push(state);
        state.OnStateEnter();
        return state2;
    }

    public IState ChangeState(string name)
    {
        IState state;
        if (name == null)
        {
            return null;
        }
        if (!this._registedState.TryGetValue(name, out state))
        {
            return null;
        }
        return this.ChangeState(state);
    }

    public void Clear()
    {
        while (this._stateStack.Count > 0)
        {
            this._stateStack.Pop().OnStateLeave();
        }
    }

    public IState GetState(string name)
    {
        IState state;
        if (name == null)
        {
            return null;
        }
        return (!this._registedState.TryGetValue(name, out state) ? null : state);
    }

    public string GetStateName(IState state)
    {
        if (state != null)
        {
            DictionaryView<string, IState>.Enumerator enumerator = this._registedState.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, IState> current = enumerator.Current;
                if (current.Value == state)
                {
                    return current.Key;
                }
            }
        }
        return null;
    }

    public IState PopState()
    {
        if (this._stateStack.Count <= 0)
        {
            return null;
        }
        IState state = this._stateStack.Pop();
        state.OnStateLeave();
        if (this._stateStack.Count > 0)
        {
            this._stateStack.Peek().OnStateResume();
        }
        return state;
    }

    public void Push(IState state)
    {
        if (state != null)
        {
            if (this._stateStack.Count > 0)
            {
                this._stateStack.Peek().OnStateOverride();
            }
            this._stateStack.Push(state);
            state.OnStateEnter();
        }
    }

    public void Push(string name)
    {
        IState state;
        if ((name != null) && this._registedState.TryGetValue(name, out state))
        {
            this.Push(state);
        }
    }

    public void RegisterState(string name, IState state)
    {
        if (((name != null) && (state != null)) && !this._registedState.ContainsKey(name))
        {
            this._registedState.Add(name, state);
        }
    }

    public void RegisterState<TStateImplType>(TStateImplType State, string name) where TStateImplType: IState
    {
        this.RegisterState(name, State);
    }

    public ClassEnumerator RegisterStateByAttributes<TAttributeType>(Assembly InAssembly) where TAttributeType: AutoRegisterAttribute
    {
        ClassEnumerator enumerator = new ClassEnumerator(typeof(TAttributeType), typeof(IState), InAssembly, true, false, false);
        ListView<Type>.Enumerator enumerator2 = enumerator.results.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Type current = enumerator2.Current;
            IState state = (IState) Activator.CreateInstance(current);
            this.RegisterState<IState>(state, state.name);
        }
        return enumerator;
    }

    public ClassEnumerator RegisterStateByAttributes<TAttributeType>(Assembly InAssembly, params object[] args) where TAttributeType: AutoRegisterAttribute
    {
        ClassEnumerator enumerator = new ClassEnumerator(typeof(TAttributeType), typeof(IState), InAssembly, true, false, false);
        ListView<Type>.Enumerator enumerator2 = enumerator.results.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Type current = enumerator2.Current;
            IState state = (IState) Activator.CreateInstance(current, args);
            this.RegisterState<IState>(state, state.name);
        }
        return enumerator;
    }

    public IState TopState()
    {
        if (this._stateStack.Count <= 0)
        {
            return null;
        }
        return this._stateStack.Peek();
    }

    public string TopStateName()
    {
        if (this._stateStack.Count <= 0)
        {
            return null;
        }
        IState state = this._stateStack.Peek();
        return this.GetStateName(state);
    }

    public IState UnregisterState(string name)
    {
        IState state;
        if (name == null)
        {
            return null;
        }
        if (!this._registedState.TryGetValue(name, out state))
        {
            return null;
        }
        this._registedState.Remove(name);
        return state;
    }

    public int Count
    {
        get
        {
            return this._stateStack.Count;
        }
    }

    public IState tarState { get; private set; }
}

