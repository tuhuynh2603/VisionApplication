
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace VisionApplication.Model
{
    [CategoryOrder(CategoryMappingOrder, 0)]
    [DisplayName("Mapping Setting")]
    public class CatergoryMappingParameters
    {

        public const string CategoryMappingOrder = "MAPPING";

        #region MAPPING
        [Browsable(true)]
        [Category(CategoryMappingOrder)]
        [DisplayName("Number Device X")]
        [Range(10, 100)]
        [DefaultValue(10)]
        [Description("")]
        [PropertyOrder(0)]
        public int M_NumberDeviceX { get; set; }
        [Browsable(true)]
        [Category(CategoryMappingOrder)]
        [DisplayName("Number Device Y")]
        [Range(1, 100)]
        [DefaultValue(10)]
        [Description("")]
        [PropertyOrder(1)]
        public int M_NumberDeviceY { get; set; }

        [Browsable(true)]
        [Category(CategoryMappingOrder)]
        [DisplayName("Number Device Per Lot")]
        [Range(1, 10000)]
        [DefaultValue(1000)]
        [Description("")]
        [PropertyOrder(2)]
        public int M_NumberDevicePerLot { get; set; }
        #endregion

    }

}
