// credit: DMGregory♦'s Answer - https://gamedev.stackexchange.com/questions/154696/picking-multiple-choices-from-an-enum/154698

using UnityEngine;

public class EnumFlagsAttribute : PropertyAttribute
{
    public string enumName;

    public EnumFlagsAttribute() { }

    public EnumFlagsAttribute(string name)
    {
        enumName = name;
    }
}