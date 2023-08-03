using NEventSocket;
using NEventSocket.FreeSwitch;
using System;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace FreewitchESLTesting
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var prg = new Program();
            try
            {
                await prg.TestFs();
            }
            catch (Exception ex)
            {

            }
            Console.WriteLine("TestFs finished!");
        }

        private async Task TestFs()
        {
            NEventSocket.Logging.Logger.Configure(new LoggerFactory());

            using (var client = await InboundSocket.Connect("172.17.13.3", 8021, "ClueCon"))
            {
                var apiResponse = await client.SendApi("show registrations as json");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(apiResponse.BodyText);



                //FreeSwitch dagi eventga ulanish
                await client.SubscribeEvents(EventName.ChannelAnswer);

                //ChannelAnswer eventni handle qilish
                client.ChannelEvents.Where(x => x.EventName == EventName.ChannelAnswer)
                      .Subscribe(async x =>
                      {
                          Console.ForegroundColor = ConsoleColor.Green;
                          Console.WriteLine("Channel ID: " + x.UUID);

                          //endi bizda chanel ID bor, uni kontrol qila olamiz:
                          await client.Play(x.UUID, "misc/8000/misc-freeswitch_is_state_of_the_art.wav");
                      });
            }
        }
        private static string GetAgentListJson => @"json {""command"": ""callcenter_config"", ""format"": ""pretty"", ""data"": {""arguments"":""agent list""}}";
    }
}
