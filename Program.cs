using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Oraculo
{
    
    class Program
    {
        private static string chave;
        private const string RedisConnectionString = "191.232.234.20:6379";
        private static ConnectionMultiplexer connection;

        private const string ChatChannel = "perguntas";
        private static string oraculo_group = string.Empty;
       

        static void Main()
        {
            connection = ConnectionMultiplexer.Connect(RedisConnectionString);
            var db = connection.GetDatabase();
             
            oraculo_group = "ConsultNet13";

            var pubsub = connection.GetSubscriber();

            pubsub.Subscribe(ChatChannel, (channel, message) => MessageAction(message));

            

            pubsub.Publish(ChatChannel, $": {oraculo_group} esta participando do grupo de oraculos.");

            while (true)
            {
                var resposta = Console.ReadLine();
                if (resposta != "")
                {
                    //pubsub.Publish(ChatChannel, $"{resposta}");  
                    db.HashSet(chave, oraculo_group,resposta);
                }
                
            }
        }

        static void MessageAction(string message)
        {
            var teste = message.Split(':');
            var db = connection.GetDatabase();

            if (teste.Count() == 2)
            {
                #region Controle de cursor
                int initialCursorTop = Console.CursorTop;
                int initialCursorLeft = Console.CursorLeft;

                Console.MoveBufferArea(0, initialCursorTop, Console.WindowWidth,
                    1, 0, initialCursorTop + 1);
                Console.CursorTop = initialCursorTop;
                Console.CursorLeft = 0;
                #endregion

                var PerguntaNumero_GroupName = teste[0];
                chave = teste[0];
                var Pergunta_Resposta = teste[1];

                if (PerguntaNumero_GroupName != oraculo_group)
                {
                    if(PerguntaNumero_GroupName != "" && PerguntaNumero_GroupName.Substring(0,1).Equals("P"))
                    {
                        #region Tratamento da pergunta que virá do bot
                        Console.WriteLine($"*****************************{PerguntaNumero_GroupName}*****************************************");
                        Console.WriteLine(Pergunta_Resposta);
                        Console.WriteLine("************************************************************************");
                        Console.WriteLine("Resposta: ");
                        Console.CursorTop = initialCursorTop + 3;                        
                        Console.CursorLeft = 10;

                        //db.HashSet(PerguntaNumero_GroupName, oraculo_group, resposta);
                      
                        #endregion
                    }
                    else
                    {
                        #region Tratamento das respostas dos outros oraculos
                        Console.WriteLine(Pergunta_Resposta);
                        Console.CursorTop = initialCursorTop + 3;
                        Console.CursorLeft = 0;
                        #endregion
                    }                                        
                }
            }
        }
    }
}
