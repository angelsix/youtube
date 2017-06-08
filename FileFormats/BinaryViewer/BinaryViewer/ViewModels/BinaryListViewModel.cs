using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BinaryViewer
{
    /// <summary>
    /// The view model for a list of bytes in the binary list
    /// </summary>
    public class BinaryListViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The byte to display
        /// </summary>
        public ObservableCollection<BinaryListItemViewModel> Bytes { get; set; }

        /// <summary>
        /// The direct text to bind to that will convert to the Bytes as it changes
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The format of the string when converting it to binary
        /// </summary>
        public StringToBinaryFormat StringFormat { get; set; } = StringToBinaryFormat.UTF8;

        /// <summary>
        /// A list of all string to binary formats
        /// </summary>
        public List<StringToBinaryFormat> StringFormats => Enum.GetValues(typeof(StringToBinaryFormat)).Cast<StringToBinaryFormat>().ToList();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BinaryListViewModel()
        {
            PropertyChanged += BinaryListViewModel_PropertyChanged;
        }

        /// <summary>
        /// Construct from a byte array
        /// </summary>
        /// <param name="bytes">The byte array to use</param>
        public BinaryListViewModel(byte[] bytes)
        {
            FromBytes(bytes);
        }

        /// <summary>
        /// Construct from a string
        /// </summary>
        /// <param name="string">The string to use</param>
        public BinaryListViewModel(string @string)
        {
            FromString(@string);
        }

        #endregion
    
        #region Public Methods 

        /// <summary>
        /// Set bytes from byte[]
        /// </summary>
        public void FromBytes(byte[] bytes)
        {
            // Create list from bytes
            Bytes = new ObservableCollection<BinaryListItemViewModel>(bytes.Select(b => new BinaryListItemViewModel { Byte = b }).ToArray());
        }

        /// <summary>
        /// Set bytes from a string
        /// </summary>
        public void FromString(string @string)
        {
            switch (StringFormat)
            {
                // ASCII string 
                case StringToBinaryFormat.ASCII:
                    FromBytes(Encoding.ASCII.GetBytes(@string));
                    break;

                // UTF7 string 
                case StringToBinaryFormat.UTF7:
                    FromBytes(Encoding.UTF7.GetBytes(@string));
                    break;

                // UTF8 string 
                case StringToBinaryFormat.UTF8:
                    FromBytes(Encoding.UTF8.GetBytes(@string));
                    break;

                // UTF32 string 
                case StringToBinaryFormat.UTF32:
                    FromBytes(Encoding.UTF32.GetBytes(@string));
                    break;

                // Unicode string 
                case StringToBinaryFormat.Unicode:
                    FromBytes(Encoding.Unicode.GetBytes(@string));
                    break;

            }
        }

        #endregion

        private void BinaryListViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the text changes, process it
            if (string.Equals(e.PropertyName, nameof(Text)))
                DetectSpecialText();
            // If the format of the string changes, process it
            else if (string.Equals(e.PropertyName, nameof(StringFormat)))
                DetectSpecialText();
        }

        private void DetectSpecialText()
        {
            if (string.IsNullOrEmpty(Text))
                return;

            // When text changes, update the bytes

            // If the bytes are all 0's and 1's and exactly 8 long
            // presume the user wants to create a byte from it
            if (Text.Length == 8 && !Text.Any(f => f != '0' && f != '1'))
            {
                // Convert the 8 characters into a single byte
                var result = 0;
                for (var i = 7; i > 0; i--)
                {
                    if (Text[i] == '1')
                        result |= (1 << (7 - i));
                }

                FromBytes(new[] { (byte)result });
            }
            // Hex value (i.e. 0x45)
            else if (Text.Length == 4 && Text.StartsWith("0x") && !Text.Skip(2).Any(f => !IsHex(f)))
            {
                FromBytes(new[] { Convert.ToByte(Convert.ToInt16(Text, 16)) });
            }
            // Otherwise, normal string
            else
                FromString(Text);
        }

        /// <summary>
        /// Checks if this character is a hexadecimal value (between 0-9 or A-F)
        /// </summary>
        /// <param name="character">The character to check</param>
        /// <returns></returns>
        private bool IsHex(char character)
        {
            return (character >= '0' && character <= '9') ||
                   (character >= 'a' && character <= 'f') ||
                   (character >= 'A' && character <= 'F');
        }
    }
}
