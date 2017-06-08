using System;
using System.Text;

namespace BinaryViewer
{
    /// <summary>
    /// The view model for a single binary item in the binary list
    /// </summary>
    public class BinaryListItemViewModel : BaseViewModel
    {
        /// <summary>
        /// The byte to display
        /// </summary>
        public byte Byte { get; set; }

        /// <summary>
        /// The hexadecimal display of the byte (e.g. 0x41)
        /// </summary>
        public string HexString => "0x" + Byte.ToString("X2");

        /// <summary>
        /// The binary display of the byte (e.g 01000001)
        /// </summary>
        public string BinaryString => Convert.ToString(Byte, 2).PadLeft(8, '0');

        /// <summary>
        /// The decimal display of the byte (e.g. 65)
        /// </summary>
        public string DecimalString => ((int)Byte).ToString();

        /// <summary>
        /// The UTF8 (Unicode) character representation of the byte
        /// </summary>
        public string UTF8String => new string((char)Byte, 1);
    }
}
