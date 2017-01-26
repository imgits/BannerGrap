using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace bannergrap
{
    class BannerBase
    {
        public ObjectId _id;//BsonType.ObjectId 这个对应了 MongoDB.Bson.ObjectId 
        public DateTime grap_time;//扫描时间
        public DateTime task_id;//扫描任务ID/启动时间
        public UInt32   ip;
        public UInt16   port;
        public string   ip_str;
        public string server_name;
        public byte[]   raw_data;
        public string   text;
        
        public BannerBase(UInt32 ip, UInt16 port, string server_name)
        {
            this.ip = ip;
            this.port = port;
            this.grap_time = DateTime.Now;
            this.ip_str = IPHelper.ntoa(ip);
            this.server_name = server_name;
        }

        virtual public string ToString()
        {
            return grap_time.ToLongDateString() + " " + ip_str + ":" + port + "\n" + text;
        }
    }
}
