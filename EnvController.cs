using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvController : MonoBehaviour
{
    public static EnvController instance;
    public EnvData[] envsData;
    public bool isAsyncLoading;


    public EnvData defaultEnv;
    EnvInfo currEnv_;
    public EnvInfo currEnv
    {
        get
        {
            return currEnv_;
        }
        set
        {
            currEnv_ = value;
            locationPresetData = currEnv_.envData.presetData;
            SetPreset();
        }
    }
    public string currSceneName;
    public List<EnvInfo> envs = new List<EnvInfo>();
    bool isLoaded;
    private void Awake()
    {
        if (EnvController.instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        EventsController.inventoryLoaded.AddListener(Setup);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetSceneByName("Env_1") != null)
            currSceneName = "Env_1";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Setup()
    {
        if (isLoaded)
            return;
        isLoaded = true;

        SetupEnv(currEnv, true);
    }
    public void Load(JSONObject dataLoaded)
    {
        envs = new List<EnvInfo>();

        if (dataLoaded != null)
        {
            
            JSONObject envsObj = dataLoaded.GetField("buyedEnvs");
            envs = new List<EnvInfo>();
            List<string> listIDs = new List<string>();
            if (envsObj != null)
            {
                for (int i = 0; i < envsData.Length; i++)
                {
                    EnvInfo envInfo = new EnvInfo();
                    envInfo.envData = envsData[i];

                    if(envsObj.HasField(envsData[i].id))
                    {
                        envInfo.isBuyed = bool.Parse(envsObj.GetField(envsData[i].id).ToString().Replace("\"", ""));
                    }

                    envs.Add(envInfo);
                }

                currEnv = GetEnvData(dataLoaded.GetField("currEnv").ToString().Replace("\"", ""));
            }
            else
            {
                SetDefaultEnv();
            }
        }
        else
        {
            SetDefaultEnv();
        }
    }

    public void SetDefaultEnv()
    {
        envs = new List<EnvInfo>();
        for (int i = 0; i < envsData.Length; i++)
        {
            EnvInfo envInfo = new EnvInfo();
            envInfo.envData = envsData[i];
            envInfo.isBuyed = i == 0;
            envs.Add(envInfo);
        }

        currEnv = GetEnvData(defaultEnv.id);
    }

    EnvInfo env_temp;
    public void SetupEnv(EnvInfo env, bool isApply)
    {
        if (IsAlreadyScene(env.envData.sceneName))
            return;

        isNeedAfterUnload = true;

        if (isApply && env.isBuyed)
            currEnv = env;

        env_temp = env;

        if (!Unload())
        {
            AfterUnLoad();
        }
    }
    public void SetupEnvDefault()
    {
        SetupEnv(envs[0], true);
    }
    void AfterUnLoad()
    {
        isNeedAfterUnload = false;

        locationPresetData = env_temp.envData.presetData;
        SetPreset();
        currSceneName = env_temp.envData.sceneName;

        Debug.LogError("AfterUnLoad: " + currSceneName);

        EventsController.envStartLoading.Invoke();

        this.Wait(0.3f, () => {
            /*if (isAsyncLoading)
                SceneManager.LoadSceneAsync(currSceneName, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(currSceneName, LoadSceneMode.Additive);*/
            
            if(PlayerInfoController.isAsync)
                StartCoroutine(LoadingSceneAsync());
            else
                SceneManager.LoadScene(currSceneName, LoadSceneMode.Additive);
        });
    }

    AsyncOperation sceneAsync;
    IEnumerator LoadingSceneAsync()
    {
        Debug.LogError("LoadingSceneAsync: " + currSceneName);
        AsyncOperation async = SceneManager.LoadSceneAsync(currSceneName, LoadSceneMode.Additive);
        sceneAsync = async;
        async.allowSceneActivation = false;
        yield return true;

        // ...

        while (async.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + async.progress);
            yield return null;
        }

        sceneAsync.allowSceneActivation = true;

        while (!async.isDone)
        {
            // wait until it is really finished
            yield return null;
        }

        Debug.LogError("LoadingSceneAsync OK: " + currSceneName);

        //...
    }
    IEnumerator UnLoadingSceneAsync()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(currSceneName, LoadSceneMode.Additive);
        sceneAsync = async;
        async.allowSceneActivation = false;
        yield return true;

        // ...

        while (async.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + async.progress);
            yield return null;
        }

        sceneAsync.allowSceneActivation = true;

        while (!async.isDone)
        {
            // wait until it is really finished
            yield return null;
        }

        //...
    }

    public bool Unload()
    {
        if (currSceneName != "")
        {
            for(int i = 0; i < SceneManager.sceneCount; i++)
            {
                if(SceneManager.GetSceneAt(i).name == currSceneName)
                {
                    Debug.LogError("UnloadSceneAsync: " + currSceneName);
                    SceneManager.UnloadSceneAsync(currSceneName);
                    return true;
                }
                    
            }
        }
        return false;
    }

    bool IsAlreadyScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                Debug.LogError("IsAlreadyScene: " + sceneName);
                return true;
            }
                
        }
        return false;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded: " + scene.name);

        if (currEnv != null && scene.name == "MainMenu")
            SetupEnv(currEnv, true);

        for (int i = 0; i < envsData.Length; i++)
        {
            if(scene.name == envsData[i].sceneName)
            {
                EventsController.envEndLoading.Invoke();
                break;
            }
        }

        LightProbes.Tetrahedralize();
    }
    bool isNeedAfterUnload;
    void OnSceneUnloaded(Scene scene)
    {
        Debug.LogError("UnLoaded: " + scene.name + ", isNeedAfterUnload: " + isNeedAfterUnload);
        if (scene.name == "Tutorial_CarControl_1")
        {
            this.Wait(0.1f, () => { UI_Loading.instance.gameObject.SetActive(false); });
        }

        Resources.UnloadUnusedAssets();

        if(isNeedAfterUnload)
            AfterUnLoad();
    }

    public LocationPresetData locationPresetData;
    void SetPreset()
    {
        
        RenderSettings.skybox = locationPresetData.skybox;

        RenderSettings.fogMode = locationPresetData.fogMode;
        RenderSettings.fogColor = locationPresetData.fogColor;
        RenderSettings.fogStartDistance = locationPresetData.fogStartDistance;
        RenderSettings.fogEndDistance = locationPresetData.fogEndDistance;
        RenderSettings.fogDensity = locationPresetData.fogDensity;

        RenderSettings.ambientLight = locationPresetData.ambientFlatColor;
        RenderSettings.customReflection = locationPresetData.ambientCubeMap;

        if (GameObject.Find("Directional Light"))
        {
            Light light = GameObject.Find("Directional Light").GetComponent<Light>();
            light.color = locationPresetData.lightColor;
            light.intensity = locationPresetData.lightIntensity;
            light.shadowStrength = locationPresetData.lightShadowIntensity;
            light.transform.eulerAngles = locationPresetData.lightRotation;
        }
        else
        {
            Debug.LogError("Помести объект с именем Directional Light в корень сцены");
        }

        if (CameraMy.instance && CameraMy.instance.amplifyColor != null)
        {
            CameraMy.instance.amplifyColor.BlendAmount = locationPresetData.blendAmount;
            CameraMy.instance.amplifyColor.Exposure = locationPresetData.exposure;
            CameraMy.instance.amplifyColor.LutTexture = locationPresetData.lut;
        }
    }

    public JSONObject GetJSON()
    {
        if (currEnv == null)
            SetDefaultEnv();

        JSONObject obj = new JSONObject(delegate (JSONObject info)
        {
            
            info.AddField("currEnv", currEnv.envData.id);

            info.AddField("buyedEnvs", new JSONObject(delegate (JSONObject envsObj)
            {
                for (int i = 0; i < envs.Count; i++)
                {
                    envsObj.AddField(envs[i].envData.id, envs[i].isBuyed);
                }
            }));
        });
        return obj;
    }

    public EnvInfo GetEnvData(string id)
    {
        for (int i = 0; i < envs.Count; i++)
        {
            if (envs[i].envData.id == id)
                return envs[i];
        }

        return null;
    }
}
