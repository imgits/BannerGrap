using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bannergrap
{
    class MysqlBanner : BannerBase
    {
        public int protocol_version;
        public string server_version;
        public string auth_plugin_name;
        public int error_code;
        public string error_message;
        public MysqlBanner(UInt32 ip, UInt16 port) : base(ip, port,"MySQL")
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
