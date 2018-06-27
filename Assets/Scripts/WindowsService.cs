using System.Collections.Generic;
using UnityEngine;
using System;
using System.ServiceProcess;
using UnityEngine.UI;

//Part1
/// <summary>
/// Goal of the script is to show use of DLL System.Services in Unity3D to
/// 1) Check if service exist or not
/// 2) See all available services
/// 3) Check status of service
/// Ofcourse more service feature are avaialable in to access like you can start/stop/pause a service but for now we will keep this much only info
/// </summary>
public class WindowsService : MonoBehaviour
{
    public InputField nameInput;
    public ScrollRect nameList;
    public Text serviceStatus;
    public GameObject serviceNamePrefab;

    private ServiceController serviceController;

    private List<Text> serviceNamesPool = new List<Text>();

    void Start()
    {
        ShowAllServicesInSystem();
    }

    void Update() { }

    public void ShowAllServicesInSystem()
    {
        int i = 0;
        foreach (ServiceController sc in ServiceController.GetServices())
        {
            if (serviceNamesPool.Count <= i)
            {
                GameObject go = Instantiate(serviceNamePrefab);
                go.transform.SetParent(nameList.transform.GetChild(0).GetChild(0));
                serviceNamesPool.Add(go.GetComponent<Text>());
            }
            serviceNamesPool[i].GetComponent<Button>().onClick.RemoveAllListeners();
            serviceNamesPool[i].GetComponent<Button>().onClick.AddListener(() => OnTextClick(sc.ServiceName));
            serviceNamesPool[i].text = sc.ServiceName;
            i++;
        }
    }

    public void OnSearchButtonClick()
    {
        GetServiceControllerStatus(nameInput.text);
    }

    public void OnTextClick(string value)
    {
        nameInput.text = value;
        GetServiceControllerStatus(value);
    }

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
    private void GetServiceControllerStatus(string serviceName)
    {
        try
        {
            serviceController = new ServiceController(serviceName);
        }
        catch (ArgumentException)
        {
            //return "Invalid service name."; // Note that just because a name is valid does not mean the service exists.
            serviceStatus.text = "Invalid service name";
        }

        using (serviceController)
        {
            try
            {
                serviceController.Refresh();
                Debug.Log("Service status : " + serviceController.Status + " service name : " + serviceName);
                serviceStatus.text = serviceController.Status + "";
            }
            catch (Exception)
            {
                // A Win32Exception will be raised if the service-name does not exist or the running process has insufficient permissions to query service status.
                // See Win32 QueryServiceStatus()'s documentation.
                //return "Error: " + ex.Message;
                serviceStatus.text = "Service-name does not exist or the running process has insufficient permissions to query service status";
            }
        }
        serviceController.Close();
        serviceController.Dispose();
        serviceController = null;
    }
}