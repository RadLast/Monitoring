using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Monitoring.Model
{
    public class Production
    {
        [Key]
        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [DisplayName("Final Target")]
        public int FinalTarget { get; set; }

        [DisplayName("Now target")]
        public int NowTarget { get; set; }

        [DisplayName("Result")]
        public int Result { get; set; }
    }
}
