using System;

namespace BinaryViewer
{
    /// <summary>
    /// The design time view model of <see cref="BinaryListItemViewModel"/>
    /// </summary>
    public class BinaryListItemDesignModel : BinaryListItemViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design time view model
        /// </summary>
        public static BinaryListItemDesignModel Instance => new BinaryListItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BinaryListItemDesignModel()
        {
            Byte = 0x41;
        }

        #endregion
    }
}
