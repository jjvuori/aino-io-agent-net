using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aino.Agents.Core.Config
{
    /// <summary>
    /// Class for reading configuration file from classpath resource.
    /// </summary>
    public class ClassPathResourceConfigBuilder : InputStreamConfigBuilder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceName">XML resource in classpath</param>
        public ClassPathResourceConfigBuilder(string resourceName) : base(GetResourceTextFile(resourceName))
        {
        }

        // This could be a better way to do it.
        InputStreamConfigBuilder ConfigBuilderFromResourceName(string resourceName)
        {
            var data = GetResourceTextFile(resourceName);
            return new InputStreamConfigBuilder(data);
        }

        public static MemoryStream GetResourceTextFile(string filename)
        {
            MemoryStream streamresult;

            using (Stream stream = typeof(ClassPathResourceConfigBuilder).Assembly.GetManifestResourceStream("assembly.folder." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    streamresult = new MemoryStream(Encoding.ASCII.GetBytes(sr.ReadToEnd()));
                }
            }
            return streamresult;
        }
    }
}
