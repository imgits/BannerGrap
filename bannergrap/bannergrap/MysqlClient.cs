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

    class MysqlClient : TcpScanner
    {
        override public string GetBanner(int timeout)
        {
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
                    BytesReader br = new BytesReader(buffer, 0, read_bytes);
                    //协议版本
                    byte protocol_version = br.ReadByte();
                    string banner = null;
                    switch(protocol_version)
                    {
                        case 9:     banner = DecodeHandshakeV9(br);break;
                        case 10:    banner = DecodeHandshakeV10(br);break;
                        case 0xff:  banner = DecodeError40(br); break;
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
        string DecodeHandshakeV9(BytesReader br)
        {
            StringBuilder banner = new StringBuilder();
            banner.Append("mysql protocol v9;");
            try
            {
                MysqlHandshakeV9 packet = new MysqlHandshakeV9();
                packet.protocol_version = 9;
                packet.server_version = br.ReadString();
                banner.Append("server_version:" + packet.server_version + ";");
                packet.connection_id = br.ReadUint32();
                packet.auth_plugin_data = br.ReadString();
                banner.Append("auth_plugin_data:" + packet.auth_plugin_data +";");
            }                                                                                                                                                     
            catch(Exception ex)
            {
            }
            return banner.ToString();
        }

        //https://dev.mysql.com/doc/internals/en/connection-phase-packets.html#packet-Protocol::Handshake
        string DecodeHandshakeV10(BytesReader br)
        {
            const int CLIENT_PLUGIN_AUTH = 0x00080000;
            const int CLIENT_SECURE_CONNECTION = 0x00008000;
            StringBuilder banner = new StringBuilder();
            banner.Append("mysql protocol v10;");
            try
            {
                MysqlHandshakeV10 packet = new MysqlHandshakeV10();
                packet.protocol_version = 10;
                packet.server_version = br.ReadString();
                banner.Append("server_version:" + packet.server_version + ";");
                packet.connection_id = br.ReadUint32();
                packet.auth_plugin_data_part_1 = br.ReadBytes(8);
                packet.filler_1 = br.ReadByte();
                packet.capability_flag_1 = br.ReadUint16();

                packet.character_set = br.ReadByte();
                packet.status_flags = br.ReadUint16();
                packet.capability_flags_2 = br.ReadUint16();

                UInt32 capability = (packet.capability_flags_2 << 16) + packet.capability_flag_1;
                if ( (capability&CLIENT_PLUGIN_AUTH) !=0)
                {

                }
                packet.auth_plugin_data_len = br.ReadByte();
                br.ReadBytes(10);
                if ((capability & CLIENT_SECURE_CONNECTION) != 0)
                {
                    int auth_plugin_data_2_len = Math.Max(13, packet.auth_plugin_data_len - 8);
                    packet.auth_plugin_data_part_2 = br.ReadBytes(auth_plugin_data_2_len);
                }
                packet.auth_plugin_name = br.ReadString();
            }
            catch(Exception ex)
            {

            }
            return banner.ToString();
        }

        //https://dev.mysql.com/doc/internals/en/packet-ERR_Packet.html
        string DecodeError40(BytesReader br)
        {
            StringBuilder banner = new StringBuilder();
            banner.Append("mysql error;");
            try
            {
                MysqlError40 packet = new MysqlError40();
                packet.marker = 0xff;
                packet.error_code = br.ReadUint16();
                packet.error_message = br.ReadString();
                banner.Append(packet.error_message);
            }
            catch (Exception ex) { }
            return banner.ToString();
        }

     }
}
