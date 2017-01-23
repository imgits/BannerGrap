using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace bannergrap
{
    class MysqlHandshakeV9
    {
        byte protocol_version;// (1) -- 0x09 protocol_version
        string server_version;// (string.NUL) -- human-readable server version
        UInt32 connection_id;// (4) -- connection id
        string auth_plugin_data;// (string.NUL) -- auth plugin data for Authentication::Old
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
        public string auth_plugin_name { get; set; }// (string.NUL) -- name of the auth_method that the auth_plugin_data belongs to
    }

    class MysqlClient : TcpScanner
    {
        MySqlConnection connection = null;
        public bool Connect1(UInt32 ip, UInt16 port, int timeout)
        {
            string server = IPHelper.ntoa(ip);
            server = "localhost";
            string database = "sys";
            string uid = "root";
            string password = "root";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";";
            try
            {
                 connection = new MySqlConnection(connectionString);
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        override public string GetBanner(int timeout)
        {
            this.ReceiveTimeout = timeout;
            
            using (NetworkStream ns = GetStream())
            {
                try
                {
                    int b0 = ns.ReadByte();
                    int b1 = ns.ReadByte();
                    int b2 = ns.ReadByte();
                    int packet_len = b2 * 16 + b1 * 8 + b0;
                    int read_bytes = 0;
                    byte[] buffer = new byte[packet_len];
                    while (read_bytes < packet_len)
                    {
                        int size = ns.Read(buffer, read_bytes, buffer.Length- read_bytes);
                        if (size < 0) break;
                        read_bytes += size;
                    }
                    string banner = Encoding.ASCII.GetString(buffer, 0, read_bytes);

                    int i = 0;
                    int PacketNumber = buffer[i++];
                    int protocol_version = buffer[i++];
                    if (protocol_version==9)
                    {
                        banner = DecodeHandshakeV9(buffer, read_bytes);
                    }
                    else if (protocol_version == 10)
                    {
                        banner = DecodeHandshakeV10(buffer, read_bytes);
                    }
                    else if (protocol_version == 0xff)
                    {
                        banner = DecodeError(buffer, read_bytes);
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
        string DecodeHandshakeV9(byte[] buffer, int size)
        {
            return null;
        }

        //https://dev.mysql.com/doc/internals/en/connection-phase-packets.html#packet-Protocol::Handshake
        string DecodeHandshakeV10(byte[] buffer, int size)
        {
            MysqlHandshakeV10 MysqlHandshake = new MysqlHandshakeV10();
            int i = 1;
            MysqlHandshake.protocol_version = buffer[i++];
            for (; i < size; i++)
            {
                if (buffer[i] == 0)
                {
                    MysqlHandshake.server_version = Encoding.ASCII.GetString(buffer, 2, i - 2);
                    i++;
                    break;
                }
            }
            MysqlHandshake.connection_id = ((UInt32)buffer[i] << 24) + ((UInt32)buffer[i + 1] << 16) + ((UInt32)buffer[i + 2] << 8) + ((UInt32)buffer[i + 3]);
            i += 4;
            MysqlHandshake.auth_plugin_data_part_1 = new byte[8];
            Buffer.BlockCopy(buffer, i, MysqlHandshake.auth_plugin_data_part_1, 0, 8);
            i += 8;
            MysqlHandshake.filler_1 = buffer[i++];
            MysqlHandshake.capability_flag_1 = ((UInt32)buffer[i++] << 8) + ((UInt32)buffer[i++]);
            MysqlHandshake.character_set = buffer[i++];
            MysqlHandshake.status_flags = ((UInt32)buffer[i++] << 8) + ((UInt32)buffer[i++]);
            MysqlHandshake.capability_flags_2 = ((UInt32)buffer[i++] << 8) + ((UInt32)buffer[i++]);
            MysqlHandshake.auth_plugin_data_len = buffer[i++];
            for (; i < size; i++)
            {
                if (buffer[i] == 0)
                {
                    MysqlHandshake.auth_plugin_name = Encoding.ASCII.GetString(buffer, 1, i - 1);
                    i++;
                    break;
                }
            }


            return null;
        }

        //https://dev.mysql.com/doc/internals/en/packet-ERR_Packet.html
        string DecodeError(byte[] buffer, int size)
        {
            return null;
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection = null;
            }
        }
    }
}
