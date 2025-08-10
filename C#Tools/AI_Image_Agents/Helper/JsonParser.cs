using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AI_Image_Agents.Helper
{
    public class JsonParser
    {
        private readonly string _filePath;

        public JsonParser(string filePath)
        {
            _filePath = filePath;
        }

        public Dictionary<string, string> GetSeleniumConfig()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Datei nicht gefunden: {_filePath}");

            string jsonContent = File.ReadAllText(_filePath);
            using JsonDocument doc = JsonDocument.Parse(jsonContent);

            var root = doc.RootElement;

            if (!root.TryGetProperty("seleniumConf", out JsonElement seleniumConfElement))
                throw new Exception("Property 'seleniumConf' nicht gefunden");

            var seleniumConfig = new Dictionary<string, string>();

            foreach (JsonProperty prop in seleniumConfElement.EnumerateObject())
            {
                seleniumConfig[prop.Name] = prop.Value.GetString() ?? "";
            }

            return seleniumConfig;
        }

        public Dictionary<string, Dictionary<string, object>> GetAgents()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Datei nicht gefunden: {_filePath}");

            string jsonContent = File.ReadAllText(_filePath);
            using JsonDocument doc = JsonDocument.Parse(jsonContent);

            var root = doc.RootElement;

            if (!root.TryGetProperty("agents", out JsonElement agentsElement))
                throw new Exception("Property 'agents' nicht gefunden");

            var result = new Dictionary<string, Dictionary<string, object>>();

            foreach (JsonProperty agentProperty in agentsElement.EnumerateObject())
            {
                string agentId = agentProperty.Name;
                var credentials = new Dictionary<string, object>();

                foreach (JsonProperty credentialProperty in agentProperty.Value.EnumerateObject())
                {
                    if (credentialProperty.Value.ValueKind == JsonValueKind.String)
                        credentials[credentialProperty.Name] = credentialProperty.Value.GetString();
                    else if (credentialProperty.Value.ValueKind == JsonValueKind.True ||
                             credentialProperty.Value.ValueKind == JsonValueKind.False)
                        credentials[credentialProperty.Name] = credentialProperty.Value.GetBoolean();
                }

                result[agentId] = credentials;
            }

            return result;
        }

        public void SetAgentSlow(string agentId)
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Datei nicht gefunden: {_filePath}");

            string jsonContent = File.ReadAllText(_filePath);
            var jsonData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent);

            if (jsonData == null || !jsonData.ContainsKey("agents"))
                throw new Exception("Property 'agents' nicht gefunden");

            var agentsJson = jsonData["agents"].ToString();
            var agentsDict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(agentsJson);

            if (agentsDict == null || !agentsDict.ContainsKey(agentId))
                throw new Exception($"Agent mit ID '{agentId}' nicht gefunden");

            agentsDict[agentId]["slowAgent"] = true;

            jsonData["agents"] = agentsDict;

            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(jsonData, options);
            File.WriteAllText(_filePath, updatedJson);
        }
    }
}