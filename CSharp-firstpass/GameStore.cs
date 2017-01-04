using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class GameStore
{
    private BinaryReader _br;
    private BinaryWriter _bw;
    private Dictionary<ushort, ClassDelegate[]> _classDlgsDict = new Dictionary<ushort, ClassDelegate[]>();
    private Queue<EStore> _esQueue;
    private static GameStore _instance;
    private Queue<IStore> _isQueue;
    private MemoryStream _ms = new MemoryStream(0x400000);
    private HashSet<uint> _objMask;
    private uint _refKeySeed;
    private Dictionary<object, uint> _refObjDict = new Dictionary<object, uint>();
    private GameTypeAttr[] _typeArray;
    private ushort _typeCounter;
    private ushort _typeKeySeed;

    private GameStore()
    {
        this._bw = new BinaryWriter(this._ms);
        this._br = null;
        this._objMask = new HashSet<uint>();
        this._isQueue = new Queue<IStore>();
        this._esQueue = new Queue<EStore>();
    }

    public void DestroyTypeList()
    {
        this._typeArray = null;
    }

    public GameTypeAttr FindRegisterType(Type type)
    {
        for (int i = 1; i <= this._typeCounter; i++)
        {
            if ((this._typeArray[i] != null) && (this._typeArray[i].type == type))
            {
                return this._typeArray[i];
            }
        }
        return null;
    }

    public uint GenRefKey()
    {
        return ++this._refKeySeed;
    }

    public ushort GenTypeKey()
    {
        if (this._typeKeySeed > this._typeCounter)
        {
            this._typeKeySeed = (ushort) (this._typeKeySeed + 1);
        }
        else
        {
            while (((this._typeKeySeed = (ushort) (this._typeKeySeed + 1)) <= this._typeCounter) && (this._typeArray[this._typeKeySeed] != null))
            {
            }
        }
        return this._typeKeySeed;
    }

    public uint GetRefKey(object obj)
    {
        if (obj == null)
        {
            return 0;
        }
        if (this._refObjDict.ContainsKey(obj))
        {
            return this._refObjDict[obj];
        }
        uint num = this.GenRefKey();
        this._refObjDict.Add(obj, num);
        return num;
    }

    public GameTypeAttr GetRegisterType(ushort key)
    {
        return this._typeArray[key];
    }

    public void InitGameType(Assembly[] asmbs)
    {
        this._typeCounter = 0;
        this._typeKeySeed = 0;
        this._typeArray = new GameTypeAttr[0x65];
        for (int i = 0; i < asmbs.Length; i++)
        {
            Type[] types = asmbs[i].GetTypes();
            for (int j = 0; j < types.Length; j++)
            {
                MethodInfo[] methods = types[j].GetMethods(BindingFlags.Public | BindingFlags.Static);
                for (int k = 0; k < methods.Length; k++)
                {
                    object[] customAttributes = methods[k].GetCustomAttributes(typeof(GameTypeAttr), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        GameTypeAttr attr = customAttributes[0] as GameTypeAttr;
                        if (attr.key >= this._typeArray.Length)
                        {
                            GameTypeAttr[] destinationArray = new GameTypeAttr[attr.key + 100];
                            Array.Copy(this._typeArray, destinationArray, this._typeArray.Length);
                            this._typeArray = destinationArray;
                        }
                        if (this._typeArray[attr.key] == null)
                        {
                            this._typeArray[attr.key] = attr;
                        }
                        if (attr.key > this._typeCounter)
                        {
                            this._typeCounter = attr.key;
                        }
                    }
                }
            }
        }
    }

    public byte[] ReadArray()
    {
        int count = this._br.ReadInt32();
        if (count > 0)
        {
            return this._br.ReadBytes(count);
        }
        return new byte[0];
    }

    public bool ReadBoolean()
    {
        return this._br.ReadBoolean();
    }

    public byte ReadByte()
    {
        return this._br.ReadByte();
    }

    public char ReadChar()
    {
        return this._br.ReadChar();
    }

    public decimal ReadDecimal()
    {
        return this._br.ReadDecimal();
    }

    public double ReadDouble()
    {
        return this._br.ReadDouble();
    }

    public object ReadEStore()
    {
        this.ReadUInt32();
        return null;
    }

    public short ReadInt16()
    {
        return this._br.ReadInt16();
    }

    public int ReadInt32()
    {
        return this._br.ReadInt32();
    }

    public long ReadInt64()
    {
        return this._br.ReadInt64();
    }

    public object ReadIStore()
    {
        uint num = this.ReadUInt32();
        if ((num != 0) && (num < uint.MaxValue))
        {
        }
        return null;
    }

    public object ReadObject()
    {
        return this.ReadIStore();
    }

    public sbyte ReadSByte()
    {
        return this._br.ReadSByte();
    }

    public float ReadSingle()
    {
        return this._br.ReadSingle();
    }

    public string ReadString()
    {
        return this._br.ReadString();
    }

    public ushort ReadUInt16()
    {
        return this._br.ReadUInt16();
    }

    public uint ReadUInt32()
    {
        return this._br.ReadUInt32();
    }

    public ulong ReadUInt64()
    {
        return this._br.ReadUInt64();
    }

    public void RegisterClassDelegate(ushort key, ClassDelegate sd, ClassDelegate rd)
    {
        ClassDelegate[] delegateArray = new ClassDelegate[] { sd, rd };
        if (!this._classDlgsDict.ContainsKey(key))
        {
            this._classDlgsDict.Add(key, delegateArray);
        }
    }

    private void RunRestore()
    {
        ClosureType type;
        while ((type = (ClosureType) this._br.ReadByte()) != ClosureType.END)
        {
            switch (type)
            {
                case ClosureType.Class:
                {
                    ushort index = this._br.ReadUInt16();
                    GameTypeAttr attr = this._typeArray[index];
                    continue;
                }
                case ClosureType.Instance:
                {
                    ushort num2 = this._br.ReadUInt16();
                    uint num3 = this._br.ReadUInt32();
                    GameTypeAttr attr2 = this._typeArray[num2];
                    break;
                }
            }
        }
    }

    public void StartStore(string savePath)
    {
        this._objMask.Clear();
        this._isQueue.Clear();
        this._esQueue.Clear();
        this._ms.Position = 0L;
        this._bw.Seek(0, SeekOrigin.Begin);
        Dictionary<ushort, ClassDelegate[]>.Enumerator enumerator = this._classDlgsDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            this.WriteByte(0);
            KeyValuePair<ushort, ClassDelegate[]> current = enumerator.Current;
            this.WriteUInt16(current.Key);
            KeyValuePair<ushort, ClassDelegate[]> pair2 = enumerator.Current;
            pair2.Value[0](this);
        }
        while ((this._isQueue.Count > 0) || (this._esQueue.Count > 0))
        {
            while (this._isQueue.Count > 0)
            {
                IStore store = this._isQueue.Dequeue();
                this.WriteByte(1);
                this.WriteUInt16(store.__TypKey);
                this.WriteUInt32(store.__RefKey);
                store.__Store(this);
            }
            while (this._esQueue.Count > 0)
            {
                EStore store2 = this._esQueue.Dequeue();
                this.WriteByte(1);
                this.WriteUInt16(store2.typKey);
                this.WriteUInt32(store2.refKey);
                store2.storeDelegate(store2.val, this);
            }
        }
        try
        {
            this._bw.Flush();
            FileStream stream = new FileStream(savePath, FileMode.Create);
            stream.Write(this._ms.GetBuffer(), 0, (int) this._ms.Position);
            stream.WriteByte(0xff);
            stream.Flush();
            stream.Close();
        }
        catch (Exception)
        {
        }
        finally
        {
            this._refObjDict.Clear();
        }
    }

    public void WriteArray(byte[] val)
    {
        if (val != null)
        {
            this._bw.Write(val.Length);
            this._bw.Write(val);
        }
        else
        {
            this._bw.Write(0);
        }
    }

    public void WriteBoolean(bool val)
    {
        this._bw.Write(val);
    }

    public void WriteByte(byte val)
    {
        this._bw.Write(val);
    }

    public void WriteChar(char val)
    {
        this._bw.Write(val);
    }

    public void WriteDecimal(decimal val)
    {
        this._bw.Write(val);
    }

    public void WriteDouble(double val)
    {
        this._bw.Write(val);
    }

    public void WriteEStore(object val, ushort typKey, StoreDelegate sd, RestoreDelegate rd)
    {
        if (val != null)
        {
            uint refKey = this.GetRefKey(val);
            this.WriteUInt32(refKey);
            if (refKey < uint.MaxValue)
            {
                if (this._objMask.Add(refKey))
                {
                    EStore item = new EStore(val, typKey, refKey, sd, rd);
                    this._esQueue.Enqueue(item);
                }
            }
            else
            {
                sd(val, this);
            }
        }
        else
        {
            this.WriteUInt32(0);
        }
    }

    public void WriteInt16(short val)
    {
        this._bw.Write(val);
    }

    public void WriteInt32(int val)
    {
        this._bw.Write(val);
    }

    public void WriteInt64(long val)
    {
        this._bw.Write(val);
    }

    public void WriteIStore(IStore val)
    {
        if (val != null)
        {
            uint num = val.__RefKey;
            this.WriteUInt32(num);
            if (num < uint.MaxValue)
            {
                if (this._objMask.Add(num))
                {
                    this._isQueue.Enqueue(val);
                }
            }
            else
            {
                val.__Store(this);
            }
        }
        else
        {
            this.WriteUInt32(0);
        }
    }

    public void WriteObject(object val)
    {
        IStore store = val as IStore;
        if ((store != null) || (val == null))
        {
            this.WriteIStore(store);
        }
    }

    public void WriteSByte(sbyte val)
    {
        this._bw.Write(val);
    }

    public void WriteSingle(float val)
    {
        this._bw.Write(val);
    }

    public void WriteString(string val)
    {
        this._bw.Write((val == null) ? string.Empty : val);
    }

    public void WriteUInt16(ushort val)
    {
        this._bw.Write(val);
    }

    public void WriteUInt32(uint val)
    {
        this._bw.Write(val);
    }

    public void WriteUInt64(ulong val)
    {
        this._bw.Write(val);
    }

    public static GameStore Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStore();
            }
            return _instance;
        }
    }
}

