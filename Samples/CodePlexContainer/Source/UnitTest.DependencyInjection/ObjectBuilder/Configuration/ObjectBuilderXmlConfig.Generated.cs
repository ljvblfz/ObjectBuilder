//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50215.44
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50215.44.
// 
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    [XmlRoot("object-builder-config", Namespace = "pag-object-builder", IsNullable = false)]
    public partial class ObjectBuilderXmlConfig
    {
        Strategies strategiesField;

        BuildRule[] buildrulesField;

        /// <remarks/>
        public Strategies strategies
        {
            get { return strategiesField; }
            set { strategiesField = value; }
        }

        /// <remarks/>
        [XmlArray("build-rules")]
        [XmlArrayItem("build-rule", IsNullable = false)]
        public BuildRule[] buildrules
        {
            get { return buildrulesField; }
            set { buildrulesField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class Strategies
    {
        Strategy[] strategyField;

        bool includedefaultField;

        public Strategies()
        {
            includedefaultField = true;
        }

        /// <remarks/>
        [XmlElement("strategy")]
        public Strategy[] strategy
        {
            get { return strategyField; }
            set { strategyField = value; }
        }

        /// <remarks/>
        [XmlAttribute("include-default")]
        [DefaultValue(true)]
        public bool includedefault
        {
            get { return includedefaultField; }
            set { includedefaultField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class Strategy
    {
        string typeField;

        /// <remarks/>
        [XmlAttribute()]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class MappedType
    {
        string typeField;

        string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class Property
    {
        object itemField;

        string nameField;

        /// <remarks/>
        [XmlElement("value-param", typeof(ValueParam))]
        [XmlElement("ref-param", typeof(RefParam))]
        public object Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class ValueParam
    {
        string typeField;

        string valueField;

        /// <remarks/>
        [XmlAttribute()]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlText()]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class RefParam
    {
        string typeField;

        string nameField;

        /// <remarks/>
        [XmlAttribute()]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class ConstructorParams
    {
        object[] itemsField;

        /// <remarks/>
        [XmlElement("value-param", typeof(ValueParam))]
        [XmlElement("ref-param", typeof(RefParam))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public partial class BuildRule
    {
        ConstructorParams constructorparamsField;

        Property[] propertyField;

        MappedType mappedtypeField;

        string typeField;

        Mode modeField;

        string nameField;

        /// <remarks/>
        [XmlElement("constructor-params")]
        public ConstructorParams constructorparams
        {
            get { return constructorparamsField; }
            set { constructorparamsField = value; }
        }

        /// <remarks/>
        [XmlElement("property")]
        public Property[] property
        {
            get { return propertyField; }
            set { propertyField = value; }
        }

        /// <remarks/>
        [XmlElement("mapped-type")]
        public MappedType mappedtype
        {
            get { return mappedtypeField; }
            set { mappedtypeField = value; }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute()]
        public Mode mode
        {
            get { return modeField; }
            set { modeField = value; }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(Namespace = "pag-object-builder")]
    public enum Mode
    {
        /// <remarks/>
        Singleton,

        /// <remarks/>
        Instance,
    }
}