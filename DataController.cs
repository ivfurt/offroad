using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public static DataController instance;

    public IAPData subscribeData;

    public bool isCanCheats;

    public string sceneName_GetCarOutBox;

    public int baseReward = 1000;

    public CarData carDefault;
    public CarData[] cars;
    public CarData[] carsFirstOpenForAD;
    public CarVisualData[] colors;
    public CarVisualData[] tires;
    public CarVisualData[] discs;
    public CarVisualData[] armors;
    public CarVisualData[] lights;
    public ModeData[] modes;
    public ModeData[] modesAdvanced;
    public LocationData[] dailyModes;
    

    

    public LocationPresetData[] locationPresets;
    public LocationPresetData locationGetCarBoxPresets;

    public LeagueData[] leagues;

    public PolygonData polygonData;

    [Space(20)]
    public AntennaData[] antenns;
    public GameObject prefabAntennCollectEffect;

    [Space(20)]
    public Shader carPaintShader;
    public Sprite icon_Silver;
    public Sprite icon_Gold;
    public Color color_Rarity_Regular;
    public Color color_Rarity_Rare;
    public Color color_Rarity_Legendary;

    public int[] rangePower = new int[] { 0, 1000 };
    public int[] rangeSpeed = new int[] { 0, 150 };
    public int[] rangeHealth = new int[] { 0, 25000 };

    public GameObject[] botsPrefab;
    public GameObject prefabNeedFuel;
    public GameObject prefabVFXController;

    public string[] tagsDeform = { "DirtRoad", "SnowRoad" };

    public Texture dirtTexture;


    private void Awake()
    {
        if (DataController.instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);
    }

    /*public BuildData GetBuildData(string id)
    {
        for (int i = 0; i < builds.Length; i++)
        {
            if (builds[i].id == id)
                return builds[i];
        }
        return null;
    }*/

    public bool ContainsTag(string tag)
    {
        for (int i = 0; i < tagsDeform.Length; i++)
            if (tagsDeform[i] == tag)
                return true;
        return false;
    }

    public Color GetColorByRarity(CarRarity carRarity)
    {
        if (carRarity == CarRarity.Rare)
            return color_Rarity_Rare;
        else if (carRarity == CarRarity.Legendary)
            return color_Rarity_Legendary;
        else
            return color_Rarity_Regular;
    }
    public string GetRarityName(CarRarity carRarity)
    {
        if (carRarity == CarRarity.Rare)
            return I2.Loc.LocalizationManager.GetTranslation("rarity_rare").ToUpper();
        else if (carRarity == CarRarity.Legendary)
            return I2.Loc.LocalizationManager.GetTranslation("rarity_legendary").ToUpper();
        else
            return I2.Loc.LocalizationManager.GetTranslation("rarity_usual").ToUpper();
    }

    public LeagueData GetLeagueData(string id)
    {
        for (int i = 0; i < leagues.Length; i++)
        {
            if (leagues[i].id == id)
                return leagues[i];
        }
        return null;
    }
    public LeagueData GetNextLeagueData(string idCurrent)
    {
        for (int i = 0; i < leagues.Length; i++)
        {
            Debug.LogError("GetNextLeagueData: " + idCurrent+", "+ leagues[i].id+", "+i+", "+(leagues.Length - 1)+", "+(leagues[i].id == idCurrent && i < leagues.Length - 1));
            if (leagues[i].id == idCurrent && i < leagues.Length - 1)
                return leagues[i+1];
        }
        Debug.LogError("GetNextLeagueData is first");
        return leagues[0];
    }

    public CarData GetFirstCarOpenForAD()
    {
        for(int i = 0; i < carsFirstOpenForAD.Length; i++)
        {
            if (!InventoryController.instance.isHasCar(carsFirstOpenForAD[i].id))
                return carsFirstOpenForAD[i];
        }
        return null;
    }

    public CarData GetCarData(string id)
    {
        for(int i = 0; i < cars.Length; i++)
        {
            if (cars[i].id == id)
                return cars[i];
        }
        Debug.LogError("GetCarData " + id + " is null!");
        return null;
    }
    public CarData GetCarData(GameObject prefab)
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].prefab == prefab)
                return cars[i];
        }
        Debug.LogError("GetCarData " + prefab.name + " is null!");
        return null;
    }
    public CarData GetCarDataRandomClosedForAD()
    {
        List<CarData> carsClosed = new List<CarData>();
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carPriceType == CarPriceType.AD && !InventoryController.instance.isHasCar(cars[i].id))
                carsClosed.Add(cars[i]);
        }
        if (carsClosed.Count > 0)
            return carsClosed[Random.Range(0, carsClosed.Count)];
        return null;
    }
    public List<CarData> GetCarsByClass(CarClass carClass)
    {
        List<CarData> list = new List<CarData>();

        for(int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carClass == carClass)
                list.Add(cars[i]);
        }

        return list;
    }
    public AntennaData GetAntenna(string id)
    {
        for(int i = 0; i < antenns.Length; i++)
        {
            if (antenns[i].id == id)
                return antenns[i];
        }

        return null;
    }
    public AntennaData GetAntennaRandomExcept(List<AntennaData> antennsExcept)
    {
        List<AntennaData> antenns_return = new List<AntennaData>();
        for(int i = 0; i < antenns.Length; i++)
        {
            bool isCan = true;
            if(antennsExcept != null && antennsExcept.Count > 0)
            for (int ii = 0; ii < antennsExcept.Count; ii++)
            {
                if (antennsExcept[ii] != null && antenns[i] != null && antennsExcept[ii].id == antenns[i].id)
                    isCan = false;
            }
            if (isCan)
                antenns_return.Add(antenns[i]);
        }
        return antenns_return[Random.Range(0, antenns_return.Count - 1)];
    }
    /* СЕЙЧАС ЭТО НАХОДИТСЯ ОТНОСИТЕЛЬНО CARDATA
        public CarVisualData[] GetCarVisualDataCategory(CarVisualDataType category)
        {
            switch (category)
            {
                case CarVisualDataType.Color:
                    return colors;
                case CarVisualDataType.Disc:
                    return discs;
                case CarVisualDataType.Tire:
                    return tires;
                case CarVisualDataType.Armor:
                    return armors;
                case CarVisualDataType.Light:
                    return lights;
            }
            return null;
        }

        public CarVisualData GetCarVisualData(CarVisualDataType category, string id)
        {
            CarVisualData[] list = GetCarVisualDataCategory(category);

            if(list != null)
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].category == category && list[i].id == id)
                        return list[i];
                }

            return null;
        }
        public CarVisualData GetCarVisualDataRandom(CarVisualDataType category)
        {
            CarVisualData[] list = GetCarVisualDataCategory(category);

            if (list != null)
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].category == category)
                        return list[Random.Range(0, list.Length)];
                }

            return null;
        }*/

    public ModeData GetModeData(string id)
    {
        for(int i = 0; i < modes.Length; i++)
        {
            if (modes[i].id == id)
                return modes[i];
        }
        return null;
    }
    public LocationData GetDailyMode(string id)
    {
        for (int i = 0; i < dailyModes.Length; i++)
        {
            if (dailyModes[i].id == id)
                return dailyModes[i];
        }
        return null;
    }
    public LocationData GetDailyModeRandom(LocationData except = null)
    {
        if(except != null)
        {
            List<LocationData> dailyModesForRand = new List<LocationData>();
            for (int i = 0; i < dailyModes.Length; i++)
            {
                if (dailyModes[i].id != except.id)
                    dailyModesForRand.Add(dailyModes[i]);
            }
            if(dailyModesForRand.Count > 0)
                return dailyModesForRand[UnityEngine.Random.Range(0, dailyModesForRand.Count)];
        }
        return dailyModes[UnityEngine.Random.Range(0, dailyModes.Length)];
    }

    public LocationData GetDailyModeNext(LocationData curr)
    {
        int idCurr = 0;

        if(curr != null)
        {
            for (int i = 0; i < dailyModes.Length; i++)
            {
                if (dailyModes[i].id == curr.id)
                    idCurr = i;
            }

            if (idCurr >= dailyModes.Length - 1)
                idCurr = 0;
            else
                idCurr++;
        }
        

        return dailyModes[idCurr];
    }

    public CarData GetRandomClosedCarData(CarPriceType carPriceType)
    {
        List<CarData> list = new List<CarData>();
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carPriceType == carPriceType && !cars[i].isIgnoreGetRandomClosedCar && !InventoryController.instance.isHasCar(cars[i].id) &&
                (InventoryController.instance.autoCrateCar == null || cars[i].id != InventoryController.instance.autoCrateCar.id) &&
                cars[i].carClass == DataController.instance.carDefault.carClass)
                list.Add(cars[i]);
        }
        if (list.Count > 0)
        {
            int rand = Random.Range(0, list.Count);
            return list[rand];
        }
            
        return null;
    }
    public CarData GetRandomCarDataByCarClass(CarClass carClass)
    {
        List<CarData> list = new List<CarData>();
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].carClass == carClass)
                list.Add(cars[i]);
        }
        if (list.Count > 0)
        {
            int rand = Random.Range(0, list.Count);
            return list[rand];
        }

        return null;
    }

    public LocationPresetData GetRandomLocationPreset()
    {
        //if (InventoryController.instance != null && (InventoryController.instance.locationCurr.locationData.id_ == "Location_Trial_1"
        //    || InventoryController.instance.locationCurr.locationData.id_ == "Location_Trial_2"
        //    || InventoryController.instance.locationCurr.locationData.id_ == "Location_Trial_3"))
        //{
        //    return locationPresets[0];
        //}
        //else
        //{ 
        //    return locationPresets[Random.Range(0, locationPresets.Length)];

        //}

        return locationPresets[Random.Range(0, locationPresets.Length)];
    }

    public List<CarClass> GetCarClasses(CarClass carClass, bool isPrev, bool isCurr, bool isNext, bool isOther, List<CarClass> except = null)
    {
        if (except == null) except = new List<CarClass>();

        List<CarClass> carClassesForReturn = new List<CarClass>();
        List<CarClass> carClasses = new List<CarClass>();

        carClasses.Add(CarClass.A);
        carClasses.Add(CarClass.B);
        carClasses.Add(CarClass.C);
        carClasses.Add(CarClass.D);
        carClasses.Add(CarClass.E);

        int id = 0;

        for(int i = 0; i < carClasses.Count; i++)
        {
            if (carClasses[i] == carClass)
            {
                id = i;
                break;
            }
        }
        if(!isOther)
        {
            if(isPrev)
            {
                id--;
            }
            if (isNext)
            {
                id++;
            }

            if (id < 0) id = 0;
            if (id > carClasses.Count-1) id = carClasses.Count-1;

            carClassesForReturn.Add(carClasses[id]);
        }
        if(isOther)
        {
            carClassesForReturn = new List<CarClass>();
            for (int i = 0; i < carClasses.Count; i++)
            {
                if (!except.Contains(carClasses[i]) && carClasses[i].ToString() != carClass.ToString())
                {
                    carClassesForReturn.Add(carClasses[i]);
                }
            }
        }

        return carClassesForReturn;
    }
}

public interface IData
{
    string displayName
    {
        get;
        set;
    }
    string id
    {
        get;
        set;
    }
    Sprite icon
    {
        get;
        set;
    }
}
public interface IInfo
{
    JSONObject GetJSON();
}
public abstract class BaseInfo : IInfo
{
    public BaseData data;
    public abstract JSONObject GetJSON();
}

[System.Serializable]
//Класс-родитель для всех данных о сущности, реализует интерфейс для указания стандартных настроек
public class BaseData : ScriptableObject, IData
{
    #region Standart Settings
    [Header("Стандартные настройки")]

    public string id_;
    public string id
    {
        get
        {
            return id_;
        }
        set
        {
            id_ = value;
        }
    }

    public string displayName_;
    public string displayName
    {
        get
        {
            return displayName_;
        }
        set
        {
            displayName_ = value;
        }
    }
    public string desc_;
    public string desc
    {
        get
        {
            return desc_;
        }
        set
        {
            desc_ = value;
        }
    }

    public Sprite icon_;
    public Sprite icon
    {
        get
        {
            return icon_;
        }
        set
        {
            icon_ = value;
        }
    }
    #endregion
}

[System.Serializable]
public class Reward
{
    public string displayName;
    public string desc;
    public RewardType rewardType;
    public int num;
    public CarData carData;
    public CrateData crateData;

    public void Apply()
    {
        Transaction transaction = new Transaction();

        if (rewardType == RewardType.Silver)
            transaction.needSilver = num;

        if (rewardType == RewardType.Gold)
            transaction.needGold = num;

        if (rewardType == RewardType.Car)
        {
            transaction.carData = carData;
            if (num > 0)
                transaction.needCrafts = num;
        }
            

        transaction.isFree = true;
        TransactionController.instance.StartTransaction(transaction);
    }
}
public enum RewardType
{
    Silver,
    Gold,
    Car,
    Crate
}

[System.Serializable]
public class DailyRewardInfo
{
    public bool isReceived;
}