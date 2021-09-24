using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf.ServiceConfigurators;

using Microsoft.Owin.Hosting;
using System.Configuration;

namespace GD_API
{
    public class TopShelfService
    {
        public string endpoint = ConfigurationSettings.AppSettings["Endp"];
        private IDisposable moDisposable = null;

        public void Start()
        {
            this.moDisposable = WebApp.Start<Startup>(endpoint);
        }

        public void Stop()
        {
            this.moDisposable?.Dispose();
        }
    }
}
