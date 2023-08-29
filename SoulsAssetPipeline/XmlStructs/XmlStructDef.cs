using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoulsAssetPipeline.XmlStructs
{
    public class XmlStructDef : IList<XmlStructDefField>
    {
        private List<XmlStructDefField> _fields = new List<XmlStructDefField>();
        //public IReadOnlyList<XmlStructDefField> Fields => _fields;

        public XmlStructDef()
        {

        }

        protected virtual void XmlReadCustomAttributes(XmlNode node)
        {

        }

        protected virtual void XmlReadCustomNodesBeforeFields(XmlNode node)
        {

        }

        protected virtual void XmlWriteCustomAttributes(XmlWriter writer)
        {

        }

        protected virtual void XmlWriteCustomNodesBeforeFields(XmlWriter writer)
        {

        }

        public XmlStructDef(XmlNode xmlNode)
        {
            XmlReadCustomAttributes(xmlNode);
            XmlReadCustomNodesBeforeFields(xmlNode);
            foreach (XmlNode fieldNode in xmlNode.ChildNodes)
            {
                if (fieldNode.Name == "#comment")
                    continue;
                var field = new XmlStructDefField(fieldNode);
                if (field.Name == null)
                {
                    field.Name = $"_{field.ValueType}_Unknown{Count}";
                }
                _fields.Add(field);
            }
        }

        public void XmlWriteWithinElement(XmlWriter writer)
        {
            XmlWriteCustomAttributes(writer);
            XmlWriteCustomNodesBeforeFields(writer);
            foreach (var f in _fields)
            {
                f.WriteWithinSerializedFfxXml(writer);
            }
        }

        public int IndexOf(XmlStructDefField item)
        {
            return ((IList<XmlStructDefField>)_fields).IndexOf(item);
        }

        public void Insert(int index, XmlStructDefField item)
        {
            ((IList<XmlStructDefField>)_fields).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<XmlStructDefField>)_fields).RemoveAt(index);
        }

        public XmlStructDefField this[int index] { get => ((IList<XmlStructDefField>)_fields)[index]; set => ((IList<XmlStructDefField>)_fields)[index] = value; }

        public void Add(XmlStructDefField item)
        {
            ((IList<XmlStructDefField>)_fields).Add(item);
        }

        public void Clear()
        {
            ((IList<XmlStructDefField>)_fields).Clear();
        }

        public bool Contains(XmlStructDefField item)
        {
            return ((IList<XmlStructDefField>)_fields).Contains(item);
        }

        public void CopyTo(XmlStructDefField[] array, int arrayIndex)
        {
            ((IList<XmlStructDefField>)_fields).CopyTo(array, arrayIndex);
        }

        public bool Remove(XmlStructDefField item)
        {
            return ((IList<XmlStructDefField>)_fields).Remove(item);
        }

        public int Count => ((IList<XmlStructDefField>)_fields).Count;

        public bool IsReadOnly => ((IList<XmlStructDefField>)_fields).IsReadOnly;

        public IEnumerator<XmlStructDefField> GetEnumerator()
        {
            return ((IList<XmlStructDefField>)_fields).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<XmlStructDefField>)_fields).GetEnumerator();
        }
    }
}
