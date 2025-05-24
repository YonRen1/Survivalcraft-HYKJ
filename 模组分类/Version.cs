using System.Linq;
using Game;
using Engine.Serialization;
using System.Xml.Linq;
using Engine;
using System;
using Engine.Media;
using Random = Game.Random;
using Engine.Graphics;
using GameEntitySystem;
using System.Collections.Generic;
using System.Globalization;
using TemplatesDatabase;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using XmlUtilities;
using Engine.Input;
using System.Text;
using System.Threading.Tasks;
//———————————————————————荒野科技——————————————————————— 
namespace HYKJ
{
    public class HYKJVersion
    {
        public string Name;
        public string MOD;
        public string Version;
        public string FullVersion;

        public HYKJVersion()
        {
            Name = "HYKJMOD";

            MOD = " 0.0.4";

            Version = " beta";

            FullVersion = Name +
                           MOD +
                           Version;
        }
    }
}