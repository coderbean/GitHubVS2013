using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GmailQuickstart
{
    class Program
    {
        #region 这些我是不是不用看，直接用就好
        static string[] Scopes = { GmailService.Scope.GmailReadonly };//GmailReadOnly 申请具有读所有数据的权限，没有写权限，更多参照oneNote笔记
        static string ApplicationName = "Gmail API Quickstart";//???????????? AppName是什么鬼？


        static void Main(string[] args)
        {
            UserCredential credential;//用户令牌？

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);//获取本机的“文档”文件夹路径
                credPath = Path.Combine(credPath, ".credentials");//设置保存证书的路径

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    System.Threading.CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;//登陆获取授权oAuth2.0
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");
            #endregion
            // List labels.
            Console.WriteLine("标签：");
            ListLabels(service, "me");
            //IList<Label> labels = request.Execute().Labels;
            //Console.WriteLine("Labels:");
            //if (labels != null && labels.Count > 0)
            //{
            //    foreach (var labelItem in labels)
            //    {
            //        Console.WriteLine("{0}", labelItem.Name);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No labels found.");
            //}
            Console.WriteLine("================================================\n按下回车查看每个对话的每个消息的snippet");
            Console.Read();  

            //显示thread ID
            List<Google.Apis.Gmail.v1.Data.Thread> threads = ListThreads(service, "me");
            if(threads != null && threads.Count>0)
            {
                foreach(var threadItem in threads)
                {
                    Console.WriteLine("对话ID：{0}",threadItem.Id);
                    Thread thread = GetThread(service,"me",threadItem.Id);
                    foreach (var message in thread.Messages)
                    {
                        Console.WriteLine("内容提要：" + message.Snippet);
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("****************");
                }
            }
            else
            {
                Console.WriteLine("没有发现thread");
            }
            Console.WriteLine("================================================按下回车查看每个对话的每个消息的id");
            Console.Read();

            //message
            List<Message> messages = ListMessages(service, "me","");
            if (messages != null && messages.Count > 0)
            {
                foreach (var messageItem in messages)
                {
                    Console.WriteLine("{0}", messageItem.Id);
                }
            }
            else
            {
                Console.WriteLine("没有发现message");
            }
            Console.Read();

        }
        /// <summary>
        /// List all message threads of the user's mailbox.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        public static List<Google.Apis.Gmail.v1.Data.Thread> ListThreads(GmailService service, String userId)
        {
            List<Google.Apis.Gmail.v1.Data.Thread> result = new List<Google.Apis.Gmail.v1.Data.Thread>();
            UsersResource.ThreadsResource.ListRequest request = service.Users.Threads.List(userId);

            do
            {
                try
                {
                    ListThreadsResponse response = request.Execute();
                    result.AddRange(response.Threads);
                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }
        /// <summary>
        /// List all Messages of the user's mailbox matching the query.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="query">String used to filter Messages returned.</param>
        public static List<Message> ListMessages(GmailService service, String userId, String query)
        {
            List<Message> result = new List<Message>();
            UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List(userId);
            request.Q = query;

            do
            {
                try
                {
                    ListMessagesResponse response = request.Execute();
                    result.AddRange(response.Messages);
                    request.PageToken = response.NextPageToken;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            } while (!String.IsNullOrEmpty(request.PageToken));

            return result;
        }
        /// <summary>
        /// List the labels in the user's mailbox.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        public static void ListLabels(GmailService service, String userId)
        {
            try
            {
                ListLabelsResponse response = service.Users.Labels.List(userId).Execute();
                foreach (Label label in response.Labels)
                {
                    Console.WriteLine(label.Id + " - " + label.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }
        /// <summary>
        /// Get a Thread with given ID.
        /// </summary>
        /// <param name="service">Gmail API service instance.</param>
        /// <param name="userId">User's email address. The special value "me"
        /// can be used to indicate the authenticated user.</param>
        /// <param name="threadId">ID of Thread to retrieve.</param>
        public static Google.Apis.Gmail.v1.Data.Thread GetThread(GmailService service, String userId, String threadId)
        {
            try
            {
                return service.Users.Threads.Get(userId, threadId).Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }

            return null;
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
    }
}