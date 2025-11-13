using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Core;
using WrathCombo.Services;
namespace WrathCombo.CustomComboNS.Functions;

internal abstract class UserData(string v)
{
    public string pName = v;

    public static implicit operator string(UserData o) => (o.pName);

    public static Dictionary<string, UserData> MasterList = new();

    public abstract void ResetToDefault();
}

internal class UserFloat : UserData
{
    // Constructor with only the string parameter
    public UserFloat(string v) : this(v, 0.0f) { }

    public float Default;

    // Constructor with both string and float parameters
    public UserFloat(string v, float defaults) : base(v) // Overload constructor to preload data
    {
        if (!Configuration.CustomFloatValues.ContainsKey(this.pName)) // if it isn't there, set
        {
            Configuration.SetCustomFloatValue(this.pName, defaults);
            Service.Configuration.Save();
        }

        Default = defaults;
        MasterList.Add(this.pName, this);
    }

    // Implicit conversion to float
    public static implicit operator float(UserFloat o) => Configuration.GetCustomFloatValue(o.pName);

    public override void ResetToDefault()
    {
        Configuration.SetCustomFloatValue(this.pName, Default);
        Service.Configuration.Save();
    }
}

internal class UserInt : UserData
{
    // Constructor with only the string parameter
    public UserInt(string v) : this(v, 0) { } // Chaining to the other constructor with a default value

    public int Default;
    // Constructor with both string and int parameters
    public UserInt(string v, int defaults) : base(v) // Overload constructor to preload data
    {
        if (!Configuration.CustomIntValues.ContainsKey(this.pName)) // if it isn't there, set
        {
            Configuration.SetCustomIntValue(this.pName, defaults);
            Service.Configuration.Save();
        }

        Default = defaults;
        MasterList.Add(this.pName, this);
    }

    // Implicit conversion to int
    public static implicit operator int(UserInt o) => Configuration.GetCustomIntValue(o.pName);

    public int Value { get { return this; } set { Configuration.SetCustomIntValue(this.pName, value); Service.Configuration.Save(); } }

    public override void ResetToDefault()
    {
        Configuration.SetCustomIntValue(this.pName, Default);
        Service.Configuration.Save();
    }
}

internal class UserBool : UserData
{
    // Constructor with only the string parameter
    public UserBool(string v) : this(v, false) { }

    public bool Default;

    // Constructor with both string and bool parameters
    public UserBool(string v, bool defaults) : base(v) // Overload constructor to preload data
    {
        if (!Configuration.CustomBoolValues.ContainsKey(this.pName)) // if it isn't there, set
        {
            Configuration.SetCustomBoolValue(this.pName, defaults);
            Service.Configuration.Save();
        }

        Default = defaults;
        MasterList.Add(this.pName, this);
    }

    // Implicit conversion to bool
    public static implicit operator bool(UserBool o) => Configuration.GetCustomBoolValue(o.pName);

    public override void ResetToDefault()
    {
        Configuration.SetCustomBoolValue(this.pName, Default);
        Service.Configuration.Save();
    }
}

internal class UserIntArray : UserData
{
    public string Name => pName;

    public int[] Default;
    public int Count => Configuration.GetCustomIntArrayValue(this.pName).Length;
    public bool Any(Func<int, bool> func) => Configuration.GetCustomIntArrayValue(this.pName).Any(func);
    public int[] Items => Configuration.GetCustomIntArrayValue(this.pName);
    public int IndexOf(int item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (Items[i] == item)
                return i;
        }
        return -1;
    }

    public void Clear(int maxValues)
    {
        var array = Configuration.GetCustomIntArrayValue(this.pName);
        Array.Resize(ref array, maxValues);
        Configuration.SetCustomIntArrayValue(this.pName, array);
        Service.Configuration.Save();
    }

    public UserIntArray(string v, int[] defaults) : base(v)
    {
        if (!Configuration.CustomIntArrayValues.ContainsKey(this.pName))
        {
            Configuration.SetCustomIntArrayValue(this.pName, defaults);
            Service.Configuration.Save();
        }

        Default = defaults;
        MasterList.Add(this.pName, this);
    }

    public UserIntArray(string v) : base(v)
    {
        if (!Configuration.CustomIntArrayValues.ContainsKey(this.pName))
        {
            Configuration.SetCustomIntArrayValue(this.pName, []);
            Service.Configuration.Save();
        }

        Default = [];
        MasterList.Add(this.pName, this);
    }

    public static implicit operator int[](UserIntArray o) => Configuration.GetCustomIntArrayValue(o.pName);

    public int this[int index]
    {
        get
        {
            if (index >= this.Count)
            {
                var array = Configuration.GetCustomIntArrayValue(this.pName);
                Array.Resize(ref array, index + 1);
                array[index] = 0;
                Configuration.SetCustomIntArrayValue(this.pName, array);
                Service.Configuration.Save();
            }
            return Configuration.GetCustomIntArrayValue(this.pName)[index];
        }
        set
        {
            if (index < this.Count)
            {
                var array = Configuration.GetCustomIntArrayValue(this.pName);
                array[index] = value;
                Service.Configuration.Save();
            }
        }
    }

    public override void ResetToDefault()
    {
        Configuration.SetCustomIntArrayValue(this.pName, (int[])Default.Clone());
        Service.Configuration.Save();
    }
}

internal class UserBoolArray : UserData
{
    // Constructor with only the string parameter
    public UserBoolArray(string v) : this(v, []) { }

    public bool[] Default;

    // Constructor with both string and bool array parameters
    public UserBoolArray(string v, bool[] defaults) : base(v)
    {
        if (!Configuration.CustomBoolArrayValues.ContainsKey(this.pName))
        {
            Configuration.SetCustomBoolArrayValue(this.pName, defaults);
            Service.Configuration.Save();
        }

        Default = defaults;
        MasterList.Add(this.pName, this);
    }

    public int Count => Configuration.GetCustomBoolArrayValue(this.pName).Length;
    public static implicit operator bool[](UserBoolArray o) => Configuration.GetCustomBoolArrayValue(o.pName);
    public bool this[int index]
    {
        get
        {
            if (index >= this.Count)
            {
                var array = Configuration.GetCustomBoolArrayValue(this.pName);
                Array.Resize(ref array, index + 1);
                array[index] = false;
                Configuration.SetCustomBoolArrayValue(this.pName, array);
                Service.Configuration.Save();
            }
            return Configuration.GetCustomBoolArrayValue(this.pName)[index];
        }
    }

    public bool All(Func<bool, bool> predicate)
    {
        var array = Configuration.GetCustomBoolArrayValue(this.pName);
        return array.All(predicate);
    }

    public override void ResetToDefault()
    {
        Configuration.SetCustomBoolArrayValue(this.pName, Default);
        Service.Configuration.Save();
    }
}