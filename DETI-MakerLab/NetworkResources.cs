using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DETI_MakerLab
{
    [Serializable()]
    class NetworkResources
    {
        private int _resourceID;
        private Project _reqProject;

        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }

        public Project ReqProject
        {
            get { return _reqProject; }
            set
            {
                if (value == null)
                    throw new Exception("Invalid Project");
                _reqProject = value;
            }
        }

        public override string ToString()
        {
            return "NetworkResource " + ResourceID.ToString();
        }

        public NetworkResources() { }

        public NetworkResources(int ResourceID, Project ReqProject)
        {
            this.ResourceID = ResourceID;
            this.ReqProject = ReqProject;
        }
    }

    [Serializable()]
    class OS
    {
        private int _osID;
        private String _osName;

        public int OSID
        {
            get { return _osID; }
            set { _osID = value; }
        }

        public String OSName
        {
            get { return _osName; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid OS Name");
                _osName = value;
            }
        }

        public override string ToString()
        {
            return "OS " + OSName;
        }

        public OS(int OSID, String OSName)
        {
            this.OSID = OSID;
            this.OSName = OSName;
        }
    }

    [Serializable()]
    class VirtualMachine : NetworkResources
    {
        private String _ip;
        private String _passwordHash;
        private String _dockerID;
        private OS _usedOS;

        public String IP
        {
            get { return _ip; }
            set
            {
                System.Net.IPAddress ipAddress = null;
                if (!System.Net.IPAddress.TryParse(value, out ipAddress))
                    throw new Exception("Invalid IP Address");
                _ip = value;
            }
        }

        public String PasswordHash
        {
            get { return _passwordHash; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid password");
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                var pbkdf2 = new Rfc2898DeriveBytes(value, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                _passwordHash = Convert.ToBase64String(hashBytes);
            }
        }

        public String DockerID
        {
            get { return _dockerID; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid DockerID");
                _dockerID = value;
            }
        }

        public VirtualMachine(int ResourceID, Project ReqProject, String IP, 
            String PasswordHash, String DockerID)
        {
            this.ResourceID = ResourceID;
            this.ReqProject = ReqProject;
            this.IP = IP;
            this.PasswordHash = PasswordHash;
            this.DockerID = DockerID;
        }
    }

    [Serializable()]
    class EthernetSocket : NetworkResources
    {
        private int _socketNum;

        public int SocketNum
        {
            get { return _socketNum; }
            set
            {
                if (value > 99999)
                    throw new Exception("Invalid SocketNum");
                _socketNum = value;
            }
        }

        public EthernetSocket(int ResourceID, Project ReqProject, int SocketNum)
        {
            this.ResourceID = ResourceID;
            this.ReqProject = ReqProject;
            this.SocketNum = SocketNum;
        }
    }

    [Serializable()]
    class WirelessLAN : NetworkResources
    {
        private String _ssid;
        private String _passwordHash;

        public String SSID
        {
            get { return _ssid; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid SSID");
                _ssid = value;
            }
        }

        public String PasswordHash
        {
            get { return _passwordHash; }
            set
            {
                if (value == null | String.IsNullOrEmpty(value))
                    throw new Exception("Invalid password");
                byte[] salt;
                new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
                var pbkdf2 = new Rfc2898DeriveBytes(value, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                _passwordHash = Convert.ToBase64String(hashBytes);
            }
        }

        public WirelessLAN(int ResourceID, Project ReqProject, String SSID,
            String PasswordHash)
        {
            this.ResourceID = ResourceID;
            this.ReqProject = ReqProject;
            this.SSID = SSID;
            this.PasswordHash = PasswordHash;
        }
    }
}
