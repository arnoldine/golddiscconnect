using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;

namespace GD_SOAP
{
    class Program
    {
        public static string Endpnt = ConfigurationSettings.AppSettings["Endp"];
        static void Main(string[] args)
        {
            //Uri baseAddress = new Uri("http://localhost:8181/Golddiskconnect");
            Uri baseAddress = new Uri(Endpnt);
           
            using (ServiceHost host = new ServiceHost(typeof(GD_Transact), baseAddress))
            {

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                Console.WriteLine("The Gold Disc Connect service is ready at {0}", baseAddress);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();

            }  
        }
        
        
    }
}
