using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionApplication.Model
{

    public class CameraParameter
    {
        public bool softwareTrigger { set; get; }
        public float exposureTime { set; get; }
        public float frameRate { set; get; }
        public float gain { set; get; }

        [Key]
        public int cameraID { get; set; } // Primary key property
        public DateTime dateChanged { set; get; }
    }
}
