using System;
using System.Collections.Generic;
using System.Text;
using gppg;

namespace COTES.ISTOK.Calc
{
    /// <summary>
    /// Адрес, использующийся в трехадресных командах
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Тип адреса
        /// </summary>
        public enum AddressType
        {
            /// <summary>
            /// Не определен
            /// </summary>
            None,

            /// <summary>
            /// Значение
            /// </summary>
            Value,

            /// <summary>
            /// Символ
            /// </summary>
            Symbol,

            ArrayElement,

            /// <summary>
            /// Парамтер
            /// </summary>
            Parameter,

            /// <summary>
            /// Метка
            /// </summary>
            Label
        }

        public bool SkipTopFrame { get; protected set; }

        public AddressType Type { get; protected set; }

        public SymbolValue Value { get; protected set; }

        public Address ArrayAddress { get; protected set; }

        public Address ArrayIndex { get; protected set; }

        public String SymbolName { get; set; }

        public int ReferenceIndex { get; set; }

        public Address(AddressType type) { Type = type; }

        public Address(SymbolValue value) : this(AddressType.Value) { Value = value; }

        public Address(Address arrayAddress, Address arrayIndex)
            : this(AddressType.ArrayElement)
        {
            this.ArrayAddress = arrayAddress;
            this.ArrayIndex = arrayIndex;
        }

        public Address(String name, bool skipTopFrame) : this(AddressType.Symbol) { SymbolName = name; SkipTopFrame = skipTopFrame; }

        public Address(String name) : this(name, false) { }

        public Address(int index) : this(AddressType.Label) { ReferenceIndex = index; }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Address address = obj as Address;

            if (address!=null)
            {
                if (Type != address.Type)
                    return false;

                switch (Type)
                {
                    case AddressType.Value:
                        return Value != null && Value.GetValue().Equals(address.Value.GetValue());
                    case AddressType.Symbol:
                    case AddressType.Parameter:
                        return SymbolName.Equals(address.SymbolName);
                    case AddressType.ArrayElement:
                        return ArrayAddress.Equals(address.ArrayAddress)
                            && ArrayIndex.Equals(address.ArrayIndex);
                    case AddressType.Label:
                        return false;
                    default:
                        return false;
                }
            }

            return base.Equals(obj);
        }
        public override string ToString()
        {
            switch (Type)
            {
                case AddressType.None:
                    break;
                case AddressType.ArrayElement:
                    return String.Format("{0}[{1}]", ArrayAddress, ArrayIndex);
                case AddressType.Value:
                    return Value.ToString();
                case AddressType.Parameter:
                    return String.Format("${0}$", SymbolName);
                case AddressType.Symbol:
                    return (SkipTopFrame ? "^" : String.Empty) + SymbolName;
                case AddressType.Label:
                    return String.Format("[{0}]", ReferenceIndex);
                default:
                    break;
            }
            return base.ToString();
        }
    }
}
