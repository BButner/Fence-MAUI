using fence_maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fence_maui.Services
{
    public class ConfigService
    {
        /// <summary>
        /// Creates a new instance of the ConfigService class,
        /// and loads the <see cref="Config"/>. (Creates and Saves if not exists)
        /// </summary>
        public ConfigService( string fileName )
        {
            if( !string.IsNullOrWhiteSpace( fileName ) )
            {
                mFileName = fileName;
            }

            if( File.Exists( mFileName ) )
            {
                var jsonString = File.ReadAllText( mFileName );
                Config = JsonSerializer.Deserialize<Config>( jsonString );
            }
            else
            {
                Config = new Config();
                Save();
            }
        }

        /// <summary>
        /// Saves the Config
        /// TODO: This should be async
        /// </summary>
        public void Save()
        {
            File.WriteAllText(
                mFileName,
                JsonSerializer.Serialize( Config,
                    new JsonSerializerOptions { WriteIndented = true } )
            );
        }

        /// <summary>
        /// Gets the Config
        /// </summary>
        public Config Config { get; }

        private string mFileName = "config.json";
    }
}