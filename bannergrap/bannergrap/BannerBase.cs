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
        public UInt32 ip;
        public UInt16 port;
        public string protocol;
        public string ip_str;
        public string service;
        public byte[] banner_data;
        public string banner_text;

        virtual public string Text
        {
            get { return ToString(); }
            set { }
        }

        public BannerBase(UInt32 ip, UInt16 port, string service, string protocol="TCP")
        {
            this.ip = ip;
            this.port = port;
            this.grap_time = DateTime.Now;
            this.ip_str = IPHelper.ntoa(ip);
            this.service = service;
            this.protocol = protocol;
        }

        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("host:" + ip_str);
            sb.AppendLine("port:" + port);
            sb.AppendLine("protocol:" + protocol);
            sb.AppendLine("service:" + service);
            sb.AppendLine("banner:" + banner_text);
            return sb.ToString();
        }

    }
}
