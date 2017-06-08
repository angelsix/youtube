using System;

namespace BinaryViewer
{
    /// <summary>
    /// The design time view model of <see cref="BinaryListViewModel"/>
    /// </summary>
    public class BinaryListDesignModel : BinaryListViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design time view model
        /// </summary>
        public static BinaryListDesignModel Instance => new BinaryListDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BinaryListDesignModel()
        {
            Text = "Hello world";
        }

        #endregion
    }
}
