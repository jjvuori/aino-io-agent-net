using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aino.Agents.Core.Config
{
    public class FileConfigBuilder : InputStreamConfigBuilder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">Configuration File</param>
        /// <exception cref="FileNotFoundException">Thrown when file is not found</exception>
        public FileConfigBuilder(Stream filestream) : base(filestream)
        {
            if (filestream == null) throw new FileNotFoundException();
        }
    }
}
