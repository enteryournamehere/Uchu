using System.Collections.Generic;
using System.Xml.Serialization;

namespace Uchu.Core
{
    [XmlRoot("Uchu")]
    public class Configuration
    {
        [XmlElement]
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration
        {
            Provider = "postgres",
            Database = "uchu",
            Host = "localhost",
            Username = "postgres",
            Password = "postgres"
        };

        [XmlElement]
        public LoggingConfiguration ConsoleLogging { get; set; } = new LoggingConfiguration
        {
            Level = LogLevel.Debug.ToString()
        };

        [XmlElement]
        public LoggingConfiguration FileLogging { get; set; } = new LoggingConfiguration
        {
            Level = LogLevel.None.ToString(),
            File = "uchu.log"
        };

        [XmlElement] public ServerDllSource DllSource { get; set; } = new ServerDllSource();

        [XmlElement] public ManagedScriptSources ManagedScriptSources { get; set; } = new ManagedScriptSources();
        
        [XmlElement]
        public ResourcesConfiguration ResourcesConfiguration { get; set; } =
            new ResourcesConfiguration {GameResourceFolder = "/res"};
        
        [XmlElement] public Networking Networking { get; set; } = new Networking();
        
        [XmlElement] public GamePlay GamePlay { get; set; } = new GamePlay();
        
        [XmlElement("Api")] public ApiConfig ApiConfig { get; set; } = new ApiConfig();
        
        [XmlElement("Sso")] public SsoConfig SsoConfig { get; set; } = new SsoConfig();
    }

    public class ManagedScriptSources
    {
        [XmlElement("Script")] public List<string> Scripts { get; set; } = new List<string>();
        
        [XmlElement("Library")] public List<string> Paths { get; set; } = new List<string>();
    }

    public class ServerDllSource
    {
        [XmlElement] public string DotNetPath { get; set; } = "dotnet";

        [XmlElement] public string Instance { get; set; } = "Uchu.Instance.dll";
        
        [XmlElement]
        public List<string> ScriptDllSource { get; set; } = new List<string>
        {
            "Uchu.StandardScripts"
        };
    }

    public class Networking
    {
        [XmlElement] public string Certificate { get; set; } = "";

        [XmlElement] public string Hostname { get; set; } = "";
        
        [XmlElement] public int CharacterPort { get; set; } = 2002;

        [XmlElement] public bool HostAuthentication { get; set; } = true;

        [XmlElement] public bool HostCharacter { get; set; } = true;

        [XmlElement] public int MaxWorldServers { get; set; } = 100;
        
        [XmlElement("WorldPort")] public List<int> WorldPorts { get; set; }
    }

    public class SsoConfig
    {
        [XmlElement] public string Domain { get; set; } = "";
    }

    public class ApiConfig
    {
        [XmlElement] public string Protocol { get; set; } = "http";
        
        [XmlElement] public string Domain { get; set; } = "localhost";

        [XmlElement] public int Port { get; set; } = 10000;
    }

    public class ResourcesConfiguration
    {
        [XmlElement] public string GameResourceFolder { get; set; }
    }

    public class LoggingConfiguration
    {
        [XmlElement] public string Level { get; set; }

        [XmlElement] public string File { get; set; }
        
        [XmlElement] public bool Timestamp { get; set; }
    }

    public class DatabaseConfiguration
    {
        [XmlElement] public string Provider { get; set; }
        
        [XmlElement] public string Database { get; set; }

        [XmlElement] public string Host { get; set; }

        [XmlElement] public string Username { get; set; }

        [XmlElement] public string Password { get; set; }
    }

    public class GamePlay
    {
        [XmlElement] public bool PathFinding { get; set; }
        
        [XmlElement] public bool AiWander { get; set; }
    }
}