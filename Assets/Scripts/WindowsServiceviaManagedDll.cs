using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WindowsServiceDll;

public class WindowsServiceviaManagedDll : MonoBehaviour {
    enum ServiceControllerStatus
    {
        InvalidServiceName = -1,

        ServiceDoesNotExist = 0,

        Stopped = 1,
        
        StartPending = 2,
      
        StopPending = 3,
     
        Running = 4,
      
        ContinuePending = 5,
       
        PausePending = 6,
       
        Paused = 7
    }

    public InputField nameInput;
    public ScrollRect nameList;
    public Text serviceStatus;
    public GameObject serviceNamePrefab;

    // Use this for initialization
    private Processes serviceController;

    private List<Text> serviceNamesPool = new List<Text>();

    void Start()
    {
        serviceController = new Processes();
        ShowAllServicesInSystem();
    }

    public void ShowAllServicesInSystem()
    {
        int i = 0;
        List<string> servicesNames = serviceController.ShowAllServicesInSystem();

        foreach (string serviceName in servicesNames)
        {
            if (serviceNamesPool.Count <= i)
            {
                GameObject go = Instantiate(serviceNamePrefab);
                go.transform.SetParent(nameList.transform.GetChild(0).GetChild(0));
                serviceNamesPool.Add(go.GetComponent<Text>());
            }
            serviceNamesPool[i].GetComponent<Button>().onClick.RemoveAllListeners();
            serviceNamesPool[i].GetComponent<Button>().onClick.AddListener(() => OnTextClick(serviceName));
            serviceNamesPool[i].text = serviceName;
            i++;
        }
        servicesNames.Clear();
        servicesNames = null;
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
        return serviceController.isServicePresent(serviceName);
    }

    /// <summary>
    /// Gets status of service
    /// </summary>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    private void GetServiceControllerStatus(string serviceName)
    {
        if (serviceName == "" || serviceName == null) return;
        ServiceControllerStatus status  = (ServiceControllerStatus)serviceController.GetServiceControllerStatus(serviceName);
        serviceStatus.text = status + "";
    }
}
