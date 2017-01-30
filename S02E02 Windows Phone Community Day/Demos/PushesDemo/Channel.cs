using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushesDemo
{
    public class Channel
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string Uri { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
