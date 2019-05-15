using Microsoft.Win32.TaskScheduler;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using WpfApp1.Models;

namespace WpfApp1.Core
{
    public class ProxyController
    {
        private static readonly Lazy<ProxyController> _instance = new Lazy<ProxyController>(true);
        private ProxyRequest _request;

        public static ProxyController Instance
        {
            get
            {
                return _instance.Value;
            }
        }


        public void TaskScheduler(ProxyRequest request)
        {
            _request = request;
            var arguments = CreateFiles();
            using (TaskService ts = new TaskService())
            {
                //Create a new task definition and assign properties
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Git Config proxy controller.";
                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.Priority = ProcessPriorityClass.High;
                td.Settings.Enabled = true;
                td.Settings.Hidden = true;

                // Create a trigger that will fire the task at this time every other day
                EventTrigger Etrigger = new EventTrigger("Microsoft-Windows-NetworkProfile/Operational", "Microsoft-Windows-NetworkProfile", 10000)
                {
                    Enabled = true
                };

                td.Triggers.Add(Etrigger);

                // Create an action that will launch Notepad whenever the trigger fires
                td.Actions.Add(new ExecAction("wscript.exe", arguments));
                //td.Actions.Add(new ExecAction($@"{BasePath}\proxy.bat"));
                
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                // Register the task in the root folder
                ts.RootFolder.RegisterTaskDefinition("GitProxyController", td);


                //run task
                var task = ts.FindTask("GitProxyController");
                task.Run();
            }
        }

        public string BasePath
        {
            get
            {
                var assemblypath = Assembly.GetExecutingAssembly().Location;
                return assemblypath.Substring(0, assemblypath.LastIndexOf("\\"));
            }
        }

        private string CreateFiles()
        {
            try
            {

                var batFilePath = Path.Combine(BasePath, "constants", "first.bat");
                var batsourcePath = $@"{BasePath}\proxy.bat";
                CopyFiles(batFilePath, batsourcePath, true);

                var vbsFilePath = Path.Combine(BasePath, "constants", "invi.vbs");
                var vbssourcePath = $@"{BasePath}\invisible.vbs";
                CopyFiles(vbsFilePath, vbssourcePath, false);

                string pathArray = $"\"{vbssourcePath}\" \"{batsourcePath}\"";

                return pathArray;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
                throw;
            }
        }

        private void CopyFiles(string source, string destination, bool isReplaceString)
        {
            using (StreamReader reader = new StreamReader(source))
            {
                var fileContent = reader.ReadToEnd();
                //"C://GitproxyController//proxy.bat"
                using (FileStream fileStream = new FileStream(destination, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter sr = new StreamWriter(fileStream))
                    {
                        if (isReplaceString)
                        {
                            fileContent = ReplaceString(fileContent);
                        }
                        sr.WriteLine(fileContent);
                        sr.Flush();
                        sr.Close();
                    }
                    fileStream.Close();
                }
                reader.Close();
            }
        }

        private string ReplaceString(string oldString)
        {
            var str = oldString.Replace("<%NetworkName%>", _request.Proxy_Network);
            str = str.Replace("<%ProxyUrl%>", GetUrl());
            return str;
        }

        private string GetUrl()
        {

            var encodedPassword = HttpUtility.UrlEncode(_request.Password, Encoding.UTF8);

            var keys = Common.Instance.GetKeys();

            foreach (var key in keys)
            {
                encodedPassword = encodedPassword.Replace(key, Common.Instance.EscapeCharacter(key));
            }

            //string url = "http://proxy-bdc.petronas.com:8080";
            var urlArr = _request.Proxy_Url.Split(("//").ToCharArray());
            var newCode = $@"{_request.Username}:{encodedPassword}";
            var str = $"{urlArr[0]}//{newCode}@{urlArr[2]}";
            return str;
        }



    }
}
