using Newtonsoft.Json;
using OpenAi_Assistant.Models;
using System.Text;
using OpenAi_Assistant.Interfaces;
using OpenAi_Assistant.OpenAiAssistant.Services;
using System.Xml.Linq;

namespace OpenAi_Assistant.Services
{
    
    public partial class OpenAiAssistantService : IOpenAiAssistantService ,IDisposable
    {


       
        private string apiKey { get; set; }
        private readonly HttpClient httpClient;
        private readonly bool _disposeHttpClient;


        private AssistantModel assistantModel { get; set; } 
        public  AssistantService assistant { get; set; }

        public ThreadModel currentThread { get; set; }



      
        public OpenAiAssistantService(string ApiKey)
        {
            
            try
            {
                apiKey = ApiKey;
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                // During beta stage of the Assistant API, we need to add OpenAI-Beta as request header.
                httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
            }
            catch(Exception AssistantConstructorException) 
            {
                Console.WriteLine("Error:" + AssistantConstructorException.Message); 
            }
        }

       
        public async Task<AssistantModel> CreateAssistant(string apimodel,string name,string tool, string instructions) 
        {
           



            assistantModel = new AssistantModel()
            {
                name = name,
                tool = tool,
                instructions = instructions,
                apimodel = apimodel,

            };
            assistant = new AssistantService(httpClient);
            assistantModel = await assistant.CreateAssistant(assistantModel);
            return assistantModel;
        }

       
        public async Task<AssistantModel> GetAssistantById(string assistant_id)
        {
            assistant = new AssistantService(httpClient);
            assistantModel = new AssistantModel();
            assistantModel = await assistant.GetAssistantById(assistant_id); // get the assistant by id
            return assistantModel;

        }


      
        public async Task<string> CreateThread()
        {

         
            try
            {        
                ThreadService threadService = new ThreadService(httpClient);
                currentThread =  await  threadService.CreateThread();
                return currentThread.thread_id;
               
            } catch (Exception SendAndRecieveException)
            {
                return SendAndRecieveException.Message;
            }

        }

     
        public async Task<string> SendMsgToThread(string msg,string role, ThreadModel threadModel)
        {
            MessageModel messageModel= new MessageModel();
            MessageService msgService = new MessageService(httpClient);
            currentThread = threadModel;
            messageModel = await msgService.CreateMsg(msg,role, threadModel);
            if(messageModel != null) 
            {
                return "Added new user message to thread.";
            }
            return "Error: Could not add new user message to thread";
        }
      
        public async Task<string> RunAssistant()
        {
         
            try
            {
                RunService runService = new RunService(httpClient);
                var response = await runService.CreateRun(currentThread, assistantModel.id); // Run the assistant with given thread & assistant id.
                if (response == true)
                {
                    return "Successfully completed run operation.";

                }else
                {
                    return "Could not complete run operation...";
                }
            }catch (Exception RunAssistantError)
            {
                return "Error:" + RunAssistantError.Message; // If assistant exception return the exception message. 
            }
        }
       
        public async Task<string> GetResponseFromAssistant()
        {
       
            MessageService messageService = new MessageService(httpClient);
            var response = await messageService.GetLatestResponse(currentThread);
            return response;
        }

       
        public void Dispose()
        {
        
            httpClient.Dispose();
        }
    }
}
