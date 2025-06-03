using System;

namespace SoulsFormats.Attributes
{
    /// <summary>
    /// Properties with this attribute are not used as a reference
    /// to obtain render groups from.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NoRenderGroupInheritance : Attribute
    {
    }
}
