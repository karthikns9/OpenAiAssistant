﻿using Newtonsoft.Json;
using OpenAi_Assistant.Interfaces;
using OpenAi_Assistant.Models;
using System.Reflection;
using System.Text;


namespace OpenAi_Assistant.Services
{
    public class AssistantService : IAssistantService
    {
        private readonly HttpClient httpClient;
        public AssistantService(HttpClient _httpClient) 
        {
          httpClient = _httpClient;
        }

        
        public async Task<AssistantModel> CreateAssistant(AssistantModel model)
        {
           
            var requestUri = "https://api.openai.com/v1/assistants";
                var requestBody = new
                {
                    name = model.name,
                    instructions = model.instructions,
                    tools = new[] { new { type = model.tool } },
                    model = model.apimodel
                };

                var response = await httpClient.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response to get the assistants id
                    model.id = responseObject?.id;
                    model.created_at = responseObject?.created_at;


                }
                else
                {
                    return null;
                }

                return model;     
        }

       
        public async Task<AssistantModel> ModifyAssistant(AssistantModel model)
        {

        

            var requestUri = $"https://api.openai.com/v1/assistants/{model.id}";
            var requestBody = new
            {
                name = model.name,
                instructions = model.instructions,
                tools = new[] { new { type = model.tool } },
                model = model.apimodel
            };

            var response = await httpClient.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
            if (response.IsSuccessStatusCode)
            {
                return model;
            }
            else
            {
                return null;
            }

        }

        public async Task<AssistantModel> DeleteAssistant(AssistantModel model)
        {

          

            var requestUri = $"https://api.openai.com/v1/assistants/{model.id}";
            var response = await httpClient.DeleteAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                model.id = null;
                model.created_at = null;
                model.tool = null;
                model.name  = null;
                model.instructions = null;
                model.apimodel = null;
                model.description  = null;
                return model;
            }
            else
            {
                return model;
            }

        }

        public async Task<AssistantModel> GetAssistantById(string assistant_id)
        {
            var requestUri = $"https://api.openai.com/v1/assistants/{assistant_id}";
            var response = await httpClient.GetAsync(requestUri);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (response.IsSuccessStatusCode)
            {
                AssistantModel model = new AssistantModel()
                {
                    id = responseObject?.id,
                    created_at = responseObject?.created_at,
                    instructions = responseObject?.instructions,
                    name = responseObject?.name,
                    apimodel = responseObject?.model,
                    tool = responseObject?.tools[0]?.type,
                };
                return model;
            } else
            {
                return null;
            }

        }
    }
}
