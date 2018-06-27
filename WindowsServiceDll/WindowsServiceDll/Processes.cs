using System;
using System.ServiceProcess;
using System.Collections.Generic;

namespace WindowsServiceDll
{
    public class Processes
    {
        /// <summary>
        /// Check if Service is present or not in system
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public bool isServicePresent(string serviceName)
        {
            bool serviceExists = false;
            foreach (ServiceController sc in ServiceController.GetServices())
            {
                if (sc.ServiceName == serviceName)
                {
                    //service is found
                    serviceExists = true;
                    break;
                }
            }
            return serviceExists;
        }

        /// <summary>
        /// Gets status of service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public int GetServiceControllerStatus(string serviceName)
        {
            ServiceController serviceController = null;
            try
            {
                serviceController = new ServiceController(serviceName);
            }
            catch (ArgumentException)
            {
                //return "Invalid service name."; // Note that just because a name is valid does not mean the service exists.
                //serviceStatus.text = "Invalid service name";
                serviceController.Close();
                serviceController.Dispose();
                return -1;
            }

            using (serviceController)
            {
                try
                {
                    serviceController.Refresh();
                    int returnType = (int)serviceController.Status;
                    serviceController.Close();
                    serviceController.Dispose();
                    return returnType;
                }
                catch (Exception)
                {
                    // A Win32Exception will be raised if the service-name does not exist or the running process has insufficient permissions to query service status.
                    // See Win32 QueryServiceStatus()'s documentation.
                    //return "Error: " + ex.Message;
                    serviceController.Close();
                    serviceController.Dispose();
                    return (int)0;
                }
            }
        }

        public List<string> ShowAllServicesInSystem()
        {
            List<string> Names = new List<string>();
            foreach (ServiceController sc in ServiceController.GetServices())
            {
                Names.Add(sc.DisplayName);
            }

            return Names;
        }

    }
}
