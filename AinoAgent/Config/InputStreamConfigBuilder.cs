using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Aino.Agents.Core.Config
{
    public class InputStreamConfigBuilder : IAgentConfigBuilder
    {
        //Todo: Logitus jollain systeemillä päälle
        //private static readonly Log log = LogFactory.GetLog(InputStreamConfigBuilder.class);
        private static readonly string LOGGER_SCHEMA = "Logger.xsd";
        
        private const string CONFIG_ENABLED_ATT_Q = "enabled";
        private const string CONFIG_LOGGER_SERVICE_Q = "ainoLoggerService";
        private const string CONFIG_ADDRESS_Q = "address";
        private const string CONFIG_SEND_Q = "send";
        private const string CONFIG_URI_ATT_Q = "uri";
        private const string CONFIG_APIKEY_ATT_Q = "apiKey";
        private const string CONFIG_INTERVAL_ATT_Q = "interval";
        private const string CONFIG_SIZE_THRESHOLD_ATT_Q = "sizeThreshold";
        private const string CONFIG_GZIP_ENABLED_ATT_Q = "gzipEnabled";
        private const string CONFIG_PROXY_Q = "proxy";
        private const string CONFIG_HOST_ATT_Q = "host";
        private const string CONFIG_PORT_ATT_Q = "port";
                     
        private const string CONFIG_OPERATIONS_Q = "operations";
        private const string CONFIG_IDTYPES_Q = "idTypes";
        private const string CONFIG_PAYLOADTYPES_Q = "payloadTypes";
        private const string CONFIG_APPLICATIONS_Q = "applications";
        private const string CONFIG_KEY_ATT_Q = "key";
        private const string CONFIG_NAME_ATT_Q = "name";

        private readonly Stream stream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">InputStream to read the configuration from</param>
        public InputStreamConfigBuilder(Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("Stream cannot be null");
            }
            this.stream = stream;
        }

        public AgentConfig Build()
        {
            return ConfigFromInputStream();
        }

        private AgentConfig ConfigFromInputStream()
        {
            ValidateSchema(GetStream());

            XmlElement configElement = ParseConfigElement(GetStream());
            XmlNode xn = configElement.GetElementsByTagName(CONFIG_LOGGER_SERVICE_Q).Item(0);
            XmlElement serviceElement = configElement.GetElementsByTagName(CONFIG_LOGGER_SERVICE_Q).Item(0) as XmlElement;
            XmlNode operationsElement = configElement.GetElementsByTagName(CONFIG_OPERATIONS_Q).Item(0);
            XmlNode applicationsElement = configElement.GetElementsByTagName(CONFIG_APPLICATIONS_Q).Item(0);
            XmlNode idTypesElement = configElement.GetElementsByTagName(CONFIG_IDTYPES_Q).Item(0);
            XmlNode payloadTypesElement = configElement.GetElementsByTagName(CONFIG_PAYLOADTYPES_Q).Item(0);

            AgentConfig config = new AgentConfig();

            if (IsServiceEnabled(serviceElement))
            {
                ApplyServiceSettings(config, serviceElement);
                ApplyOperationSettings(config, operationsElement);
                ApplyApplicationSettings(config, applicationsElement);
                ApplyIdTypeSettings(config, idTypesElement);
                ApplyPayloadTypeSettings(config, payloadTypesElement);
            }

            config.SetEnabled(IsServiceEnabled(serviceElement));
            CloseStream();
            return config;
        }

        /// <summary>
        /// Validates the config file against XSD schema.
        /// </summary>
        /// <param name="stream">MemoryStream to config file.</param>
        private void ValidateSchema(Stream stream)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", LOGGER_SCHEMA); // Tämä vaatii paremman linkityksen tuohon loggerschemaan esim. resurssina.

            bool validationErrors = false;
            XDocument doc = XDocument.Load(stream);
            doc.Validate(schemas, (s, e) => {
                validationErrors = true;
            });

            if(validationErrors)
            {
                throw new InvalidAgentConfigException("Failed to validate logger config.");
            }
        }


        private bool IsServiceEnabled(XmlNode serviceElement)
        {
            if (null == serviceElement)
            {
                return false;
            }

            bool.TryParse(serviceElement.Attributes[CONFIG_ENABLED_ATT_Q].Value, out bool result);

            return result;
        }

        private void ApplyServiceSettings(AgentConfig config, XmlElement serviceElement)
        {

            XmlNode addressElement = serviceElement.GetElementsByTagName(CONFIG_ADDRESS_Q).Item(0);
            XmlNode proxyElement = serviceElement.GetElementsByTagName(CONFIG_PROXY_Q).Item(0);
            XmlNode sendElement = serviceElement.GetElementsByTagName(CONFIG_SEND_Q).Item(0);

            if (null == addressElement || null == sendElement)
            {
                throw new InvalidAgentConfigException("The logger config does not contain all of the required elements for the logger service configuration.");
            }

            config.SetLogServiceUri(addressElement.Attributes[CONFIG_URI_ATT_Q].Value);
            config.SetApiKey(addressElement.Attributes[CONFIG_APIKEY_ATT_Q].Value);
            config.SetSendInterval(int.Parse(sendElement.Attributes[CONFIG_INTERVAL_ATT_Q].Value));
            config.SetSizeThreshold(int.Parse(sendElement.Attributes[CONFIG_SIZE_THRESHOLD_ATT_Q].Value));
            config.SetGzipEnabled(bool.Parse(sendElement.Attributes[CONFIG_GZIP_ENABLED_ATT_Q].Value));

            if (null != proxyElement)
            {
                config.SetProxyHost(proxyElement.Attributes[CONFIG_HOST_ATT_Q].Value);
                config.SetProxyPort(int.Parse(proxyElement.Attributes[CONFIG_PORT_ATT_Q].Value));
            }
        }

        private void ApplyKeyNameElementSettings(AgentConfig config, XmlNode elementList, AgentConfig.KeyNameElementType type)
        {
            foreach (XmlNode node in elementList.ChildNodes)
            {
                string key = node.Attributes[CONFIG_KEY_ATT_Q].Value;
                string name = node.Attributes[CONFIG_NAME_ATT_Q].Value;

                if (config.Get(type).EntryExists(key))
                {
                    throw new InvalidAgentConfigException("Duplicate key: " + key + " for type: " + type.ToString()); //This type.ToString() might need something else.
                }
                config.Get(type).AddEntry(key, name);
            }
        }

        private void ApplyOperationSettings(AgentConfig config, XmlNode operationsElement)
        {
            ApplyKeyNameElementSettings(config, operationsElement, AgentConfig.KeyNameElementType.Operations);
        }

        private void ApplyPayloadTypeSettings(AgentConfig config, XmlNode payloadTypeElement)
        {
            ApplyKeyNameElementSettings(config, payloadTypeElement, AgentConfig.KeyNameElementType.Payloadtypes);
        }

        private void ApplyApplicationSettings(AgentConfig config, XmlNode applicationsElement)
        {
            ApplyKeyNameElementSettings(config, applicationsElement, AgentConfig.KeyNameElementType.Applications);
        }

        private void ApplyIdTypeSettings(AgentConfig config, XmlNode idTypesElement)
        {
            ApplyKeyNameElementSettings(config, idTypesElement, AgentConfig.KeyNameElementType.Idtypes);
        }
        
        private XmlElement ParseConfigElement(Stream stream)
        {
            try
            {
                var xmldoc = new XmlDocument();
                xmldoc.Load(stream);
                return xmldoc.DocumentElement;
            }
            catch (XmlException e)
            {
                throw new InvalidAgentConfigException("Unable to read logger config.", e);
            }
        }


        private Stream GetStream()
        {
            try
            {
                ClearStream();
                return stream;
            }
            catch (IOException e)
            {
                throw new InvalidAgentConfigException("Failed to reset() stream.", e);
            }
        }

        private void ClearStream()
        {

            StreamReader sr = new StreamReader(stream);
            sr.BaseStream.Seek(0, SeekOrigin.Begin);

            /* This would work with MemoryStream, but not with Stream
            byte[] buffer = stream.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            stream.Position = 0;
            stream.SetLength(0);*/
        }

        private void CloseStream()
        {
            try
            {
                GetStream().Close();
            }
            catch (IOException e)
            {
                //This should happen just about never
                throw new InvalidAgentConfigException("Could not close internal ByteArrayInputStream.", e);
            }
        }
    }
}
