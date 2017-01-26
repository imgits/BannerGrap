using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace bannergrap
{
    class MysqlHandshakeV9
    {
        public byte   protocol_version { get; set; }// (1) -- 0x09 protocol_version
        public string server_version { get; set; }// (string.NUL) -- human-readable server version
        public UInt32 connection_id { get; set; }// (4) -- connection id
        public string auth_plugin_data { get; set; }// (string.NUL) -- auth plugin data for Authentication::Old
    }

    class MysqlHandshakeV10
    {
        public byte protocol_version { get; set; }// (1) -- 0x0a protocol_version
        public string server_version { get; set; }// (string.NUL) -- human-readable server version
        public UInt32 connection_id { get; set; }// (4) -- connection id
        public byte[] auth_plugin_data_part_1 { get; set; }// (string.fix_len) -- [len=8] first 8 bytes of the auth-plugin data
        public byte filler_1 { get; set; }// (1) -- 0x00
        public UInt32 capability_flag_1 { get; set; }// (2) -- lower 2 bytes of the Protocol::CapabilityFlags(optional)
        public byte character_set { get; set; }// (1) -- default server character-set, only the lower 8-bits Protocol::CharacterSet(optional)
        public UInt32 status_flags { get; set; }// (2) -- Protocol::StatusFlags(optional)
        public UInt32 capability_flags_2 { get; set; }// (2) -- upper 2 bytes of the Protocol::CapabilityFlags
        public byte auth_plugin_data_len { get; set; }//(1) -- length of the combined auth_plugin_data, if auth_plugin_data_len is > 0
        public byte[] auth_plugin_data_part_2 { get; set; }
        public string auth_plugin_name { get; set; }// (string.NUL) -- name of the auth_method that the auth_plugin_data belongs to
    }

    class MysqlError40
    {
        public byte marker { get; set;}//1 byte 0xff
        public UInt32 error_code { get; set; }//2 bytes
        public string error_message { get; set; }//Zero-terminated text of the error message.
    }
    class MysqlError41
    {
        public byte marker { get; set; }//1 byte 0xff
        public UInt32 error_code { get; set; }//2 bytes
        public byte sharp { get; set; }//1 byte '#'
        public byte[] sql_state { get; set; }//5 bytes The value of the ODBC/JDBC SQL state.
        public string error_message { get; set; }//Zero-terminated text of the error message.
    }

    class MysqlScanner : TcpScanner
    {
        static public void Scan(UInt32 ip, UInt16 port, int timeout)
        {
            using (MysqlScanner scanner = new MysqlScanner())
            {
                MysqlBanner banner = scanner.GetBanner(ip, port, timeout);
                if (banner!=null)
                {

                }
            }
        }
        MysqlBanner banner = null;
        public new MysqlBanner GetBanner(UInt32 ip, UInt16 port, int timeout)
        {
            if (!Connect(ip, port, timeout)) return null;

            banner = new MysqlBanner(ip, port);

            this.ReceiveTimeout = timeout;
            using (NetworkStream ns = GetStream())
            {
                ns.ReadTimeout = timeout;
                try
                {
                    //包长度
                    int b0 = ns.ReadByte();
                    int b1 = ns.ReadByte();
                    int b2 = ns.ReadByte();
                    int pktdatalen = b0 + (b1 << 8) + b2;
                    //包序列号
                    int PacketNumber = ns.ReadByte();
                    //包数据
                    byte[] buffer = new byte[pktdatalen];
                    int read_bytes = 0;
                    while (read_bytes < pktdatalen)
                    {
                        int size = ns.Read(buffer, read_bytes, pktdatalen - read_bytes);
                        if (size <= 0) break;
                        read_bytes += size;
                    }
                    if (read_bytes > 0)
                    {
                        //banner.raw_data = new byte[read_bytes];
                        //Buffer.BlockCopy(buffer, 0, banner.raw_data, 0, read_bytes);
                    }
                    BytesReader br = new BytesReader(buffer, 0, read_bytes);
                    //协议版本
                    byte protocol_version = br.ReadByte();
                    switch(protocol_version)
                    {
                        case 9:     DecodeHandshakeV9(br);break;
                        case 10:    DecodeHandshakeV10(br);break;
                        case 0xff:  DecodeError40(br); break;
                        default:
                            break;
                    }
                    return banner;
                }
                catch (Exception ex)
                {
                }
            }
            return null;
        }

        //https://dev.mysql.com/doc/internals/en/connection-phase-packets.html#packet-Protocol::HandshakeV9
        void DecodeHandshakeV9(BytesReader br)
        {
            try
            {
                banner.protocol_version = 9;
                banner.server_version = br.ReadString();
                UInt32 connection_id = br.ReadUint32();
                banner.auth_plugin_name = br.ReadString();
            }                                                                                                                                                     
            catch(Exception ex)
            {
            }
        }

        //https://dev.mysql.com/doc/internals/en/connection-phase-packets.html#packet-Protocol::Handshake
        void DecodeHandshakeV10(BytesReader br)
        {
            const int CLIENT_PLUGIN_AUTH = 0x00080000;
            const int CLIENT_SECURE_CONNECTION = 0x00008000;
            try
            {
                banner.protocol_version = 10;
                banner.server_version = br.ReadString();
                UInt32 connection_id = br.ReadUint32();
                br.skip(8);//auth_plugin_data_part_1
                br.skip(1);//filler_1 = br.ReadByte();
                UInt32 capability_flag_1 = br.ReadUint16();
                br.skip(1);//packet.character_set = br.ReadByte();
                br.skip(2);//packet.status_flags = br.ReadUint16();
                UInt32 capability_flags_2 = br.ReadUint16();

                UInt32 capability = (capability_flags_2 << 16) + capability_flag_1;
                if ( (capability&CLIENT_PLUGIN_AUTH) !=0)
                {

                }
                int auth_plugin_data_len = br.ReadByte();
                br.skip(10);//filler
                if ((capability & CLIENT_SECURE_CONNECTION) != 0)
                {
                    int auth_plugin_data_2_len = Math.Max(13, auth_plugin_data_len - 8);
                    br.skip(auth_plugin_data_2_len);//packet.auth_plugin_data_part_2 = br.ReadBytes(auth_plugin_data_2_len);
                }
                banner.auth_plugin_name = br.ReadString();
            }
            catch(Exception ex)
            {

            }
        }

        //https://dev.mysql.com/doc/internals/en/packet-ERR_Packet.html
        void DecodeError40(BytesReader br)
        {
            try
            {
                banner.protocol_version = -1;
                banner.error_code = (int)br.ReadUint16();
                banner.error_message = br.ReadString();
            }
            catch (Exception ex) { }
        }

     }
}
