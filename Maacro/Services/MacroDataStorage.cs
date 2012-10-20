using Maacro.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Maacro.Services
{
    public class MacroDataStorage : Maacro.Services.IMacroDataStorage
    {
        private static object _ioLock = new object();

        [Serializable]
        internal class MacroDataDto
        {
            public MacroDataDto()
            {
            }

            public static MacroDataDto FromMacroData(MacroData source)
            {
                MacroDataDto result = new MacroDataDto();

                result.Deployment = source.Deployment.Select(p =>
                    new Tuple<int, int>(p.PageNumber, p.SlotNumber)
                ).ToList();

                result.UIDelay = source.UIDelay;
                result.HeroPageCount = source.HeroPageCount;
                result.Length = source.Length;

                result.ScreenElements = source.ScreenElements.Select(p => 
                    new Tuple<ScreenElementType, string, int, int>(p.ElementType, p.Name, p.X, p.Y)
                ).ToList();

                return result;
            }
        
            public List<Tuple<int, int>> Deployment { get; set; }
            public List<Tuple<ScreenElementType, string, int, int>> ScreenElements { get; set; }
            public int UIDelay { get; set; }
            public int HeroPageCount { get; set; }
            public DeployLength Length { get; set; }

            internal MacroData ToMacroData()
            {
                var elementList = this.ScreenElements.Select(p => new ScreenElement()
                    {
                        ElementType = p.Item1,
                        Name = p.Item2,
                        X = p.Item3,
                        Y = p.Item4
                    }
                ).ToList();

                var deploymentList = this.Deployment.Select(p => new DeploymentSlot()
                    {
                        PageNumber = p.Item1,
                        SlotNumber = p.Item2
                    }
                ).ToList();

                return new MacroData(elementList, deploymentList, this.UIDelay, this.HeroPageCount, this.Length);            
            }
        }

        public MacroData Load()
        {
            MacroData result = null;

            string thisDir = Path.GetDirectoryName(this.GetType().Assembly.Location);
            string fileName = string.Concat(Environment.MachineName, ".maacro");
            string filePath = Path.Combine(thisDir, fileName);

            FileStream fs = null;            

            if (!File.Exists(filePath))
                return null;
            else
                fs = File.OpenRead(filePath);

            fs.Position = 0;                

            using (fs)
            {
                BinaryFormatter bf = new BinaryFormatter();
                var deserialized = bf.Deserialize(fs) as MacroDataDto;

                if (deserialized != null)
                    result = (MacroData)deserialized.ToMacroData();

                fs.Flush();
                fs.Close();
            }

            return result;
        }

        public void Save(MacroData macroData)
        {
            lock (_ioLock)
            {
                string thisDir = Path.GetDirectoryName(this.GetType().Assembly.Location);
                string fileName = string.Concat(Environment.MachineName, ".maacro");
                string filePath = Path.Combine(thisDir, fileName);

                Stream fs = null;

                if (File.Exists(filePath))
                {
                    fs = new FileStream(filePath, FileMode.Truncate, FileAccess.Write, FileShare.Read);
                    fs.Position = 0;
                }
                else
                    fs = File.Create(filePath);

                using (fs)
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    var toSerialize = MacroDataDto.FromMacroData(macroData);

                    bf.Serialize(fs, toSerialize);

                    fs.Flush();
                    fs.Close();
                }
            }
        }

        public Task SaveAsync(MacroData macroData)
        {
            return new Task(
                () => Save(macroData)
            );
        }
    }
}
